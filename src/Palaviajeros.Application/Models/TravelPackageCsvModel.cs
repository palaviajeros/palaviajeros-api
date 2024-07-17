using CsvHelper.Configuration;
using Palaviajeros.Domain.Entities;
using Palaviajeros.Domain.ValueObjects;

namespace Palaviajeros.Application.Models;

public class TravelPackageCsvModel
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string[] Description { get; set; }
    public string[] Inclusions { get; set; }

    public string[] Exclusions => Enum.GetValues(typeof(Services)).Cast<Services>().Select(a => a.ToString())
        .Except(Inclusions.Select(i => i.ToUpper())).ToArray();

    public DateRange[] TravelDates { get; set; }
    public DayPlan[] Itinerary { get; set; }
    public float Price { get; set; }
    public bool? IsFlexible { get; set; }

    public static TravelPackageCsvModel FromDomain(TravelPackage travelPackage)
    {
        return new TravelPackageCsvModel
        {
            Code = travelPackage.PackageCode,
            Name = travelPackage.Name,
            Description = travelPackage.Description,
            Inclusions = travelPackage.Inclusions.Select(i => i.ToString()).ToArray(),
            TravelDates = travelPackage.DataRanges.ToArray(),
            Itinerary = travelPackage.Itinerary.ToArray()
        };
    }
}

public sealed class TravelPackagesMap : ClassMap<TravelPackageCsvModel>
{
    public TravelPackagesMap()
    {
        Map(m => m.Code);
        Map(m => m.Name);
        Map(m => m.Description).Convert(args =>
        {
            var flattenMissingArray = args.Value.Description;
            return string.Join(",", flattenMissingArray);
        });
        Map(m => m.Inclusions);
        Map(m => m.TravelDates).Name("Dates");
        Map(m => m.Itinerary);
        Map(m => m.Price);
        Map(m => m.IsFlexible);
    }
}

public sealed class ItineraryMap : ClassMap<DayPlan>
{
    public ItineraryMap()
    {
        Map(m => m.dayNo);
        Map(m => m.activities);
    }
}