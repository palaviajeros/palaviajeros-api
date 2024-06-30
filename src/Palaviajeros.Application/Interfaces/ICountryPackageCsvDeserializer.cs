using System.Dynamic;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
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
            ProcessPackagesData(unprocessedPackagesData.Select(upd => TransformItinerariesToOneField(upd)).ToList())
                .ToList();

        return Task.FromResult(country);
    }

    private static IEnumerable<TravelPackageCsvModel> ProcessPackagesData(List<dynamic> rotatedData)
    {
        // Write the new list to memory
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        csvWriter.WriteRecords(rotatedData);
        writer.Flush();
        stream.Position = 0;

        // Read in the person records using a ClassMap.
        csvReader.Context.RegisterClassMap<TravelPackagesMap>();
        csvReader.Context.RegisterClassMap<ItineraryMap>();
        return csvReader.GetRecords<TravelPackageCsvModel>().ToList();
    }

    private static CountryPackagesCsvModel ProcessCountryData(List<dynamic> rotatedData)
    {
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Write the new list to memory
        csvWriter.WriteRecords(rotatedData);
        writer.Flush();
        stream.Position = 0;

        // Read in the person records using a ClassMap.
        csvReader.Context.RegisterClassMap<CountryPackagesMap>();
        var cp = csvReader.GetRecords<CountryPackagesCsvModel>();
        return cp.First();
    }

    private static dynamic TransformItinerariesToOneField(dynamic rotatedData)
    {
        IDictionary<string, object> dict = rotatedData;
        var unparsedItineraries = dict.Where(d => d.Key.Contains("Itinerary")).ToList();
        dict.Add("Itinerary", unparsedItineraries.Select(r => new DayPlan(int.Parse(r.Key.Split('-')[1]),
            ((List<object>)r.Value).Select(v => v.ToString() ?? "").ToArray())));

        foreach (var (key, _) in unparsedItineraries) dict.Remove(key);

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