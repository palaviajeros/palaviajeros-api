using System.Dynamic;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Palaviajeros.Application.Models;
using Palaviajeros.Domain.ValueObjects;

namespace Palaviajeros.Application.Interfaces;

public interface ICountryPackageCsvDeserializer
{
    Task<CountryPackagesCsvModel> Deserialize(Stream fileStream);
}

public class CountryPackageCsvDeserializer : ICountryPackageCsvDeserializer
{
    public enum ReadMode
    {
        Country,
        Package,
        Pending
    }

    public Task<CountryPackagesCsvModel> Deserialize(Stream fileStream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null
        };
        dynamic unprocessedCountryData = null;
        var unprocessedPackagesData = new List<dynamic>();

        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, config);
        {
            var readMode = ReadMode.Pending;
            var dynamicObjects = new List<dynamic>();
            while (csv.Read())
                switch (readMode)
                {
                    case ReadMode.Country:
                    case ReadMode.Package:
                        if (csv.GetField(0) == "")
                        {
                            var rotatedData = RotateData(dynamicObjects);
                            switch (readMode)
                            {
                                case ReadMode.Country:
                                    unprocessedCountryData = rotatedData;
                                    break;
                                case ReadMode.Package:
                                    unprocessedPackagesData.Add(rotatedData);
                                    break;
                                case ReadMode.Pending:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            readMode = ReadMode.Pending;
                            dynamicObjects.Clear();
                        }
                        else
                        {
                            dynamicObjects.Add(csv.GetRecord<dynamic>());
                        }

                        break;
                    case ReadMode.Pending:
                        readMode = csv.GetField(0) switch
                        {
                            "COUNTRY" => ReadMode.Country,
                            "PACKAGE" => ReadMode.Package,
                            "" => ReadMode.Pending,
                            _ => readMode
                        };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
        var country = ProcessCountryData([unprocessedCountryData]);
        country.Packages =
            ProcessPackagesData(unprocessedPackagesData.Select(upd => TransformFields(upd)).ToList())
                .ToList();

        return Task.FromResult(country);
    }

    private static List<TravelPackageCsvModel> ProcessPackagesData(List<dynamic> rotatedData)
    {
        return JsonConvert.DeserializeObject<List<TravelPackageCsvModel>>(JsonConvert.SerializeObject(rotatedData)) ??
               [];
    }

    private static CountryPackagesCsvModel ProcessCountryData(List<dynamic> rotatedData)
    {
        return (JsonConvert.DeserializeObject<CountryPackagesCsvModel[]>(JsonConvert.SerializeObject(rotatedData)) ??
                Array.Empty<CountryPackagesCsvModel>())
            .FirstOrDefault() ?? new CountryPackagesCsvModel();
    }

    private static dynamic TransformFields(dynamic rotatedData)
    {
        IDictionary<string, object> dict = rotatedData;
        var unparsedItineraries = dict.Where(d => d.Key.Contains("Itinerary")).ToList();
        dict.Add("Itinerary",
            unparsedItineraries.Select(kv =>
                    new DayPlan(int.Parse(kv.Key.Split('-')[1]),
                        (kv.Value is not List<object> list
                            ? [kv.Value as string ?? ""]
                            : list.Select(o => o.ToString()).ToArray())!))
                .ToArray());

        foreach (var (key, _) in unparsedItineraries) dict.Remove(key);

        if (dict.TryGetValue("Description", out var descValue))
            dict["Description"] = descValue is not List<object> ? new List<object> { descValue } : descValue;

        if (dict.TryGetValue("Dates", out var dates))
        {
            var datesCollection = dates is not List<object> list
                ? new List<string> { dates.ToString() ?? "" }
                : list.Cast<string>();
            dict["TravelDates"] = datesCollection.Select(d =>
            {
                var dateRanges = d.Split(',').Select(s => DateTime.Parse(s.Trim())).ToArray();
                return new DateRange(dateRanges[0], dateRanges[1]);
            });
            dict.Remove("Dates");
        }

        return rotatedData;
    }

    private static dynamic RotateData(IEnumerable<dynamic> records)
    {
        var rows = records.Select(row => (row as IDictionary<string, object>)!).ToList();

        var flippedRecord = new ExpandoObject() as IDictionary<string, object>;
        for (var i = 2; i <= rows[0].Count; i++)
            foreach (var row in rows.Where(row => !string.IsNullOrEmpty(row["Field" + i].ToString())))
            {
                var fieldName = (string)row["Field1"];
                var cellValue = row["Field" + i];
                if (flippedRecord.TryGetValue(fieldName, out var existingValue))
                {
                    if (existingValue is not List<object>)
                        flippedRecord[fieldName] = new List<object> { existingValue };
                    (flippedRecord[fieldName] as List<object>)?.Add(cellValue);
                    continue;
                }

                flippedRecord.Add(fieldName, cellValue);
            }

        return flippedRecord;
    }
}