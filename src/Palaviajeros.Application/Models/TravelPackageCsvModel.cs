using CsvHelper.Configuration;
using Palaviajeros.Domain.Entities;
using Palaviajeros.Domain.ValueObjects;

namespace Palaviajeros.Application.Models;

public class TravelPackageCsvModel
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string[] Inclusions { get; set; }
    public string[] Exclusions { get; set; }
    public DateRange[] TravelDates { get; set; }
    public DayPlan[] Itinerary { get; set; }

    public static TravelPackageCsvModel FromDomain(TravelPackage travelPackage)
    {
        return new TravelPackageCsvModel
        {
            Code = travelPackage.PackageCode,
            Name = travelPackage.Name,
            Description = travelPackage.Description,
            Inclusions = travelPackage.Inclusions.Select(i => i.ToString()).ToArray(),
            Exclusions = Enum.GetValues(typeof(Amenity)).Cast<Amenity>().Except(travelPackage.Inclusions)
                .Select(e => e.ToString()).ToArray(),
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
        Map(m => m.Description);
        Map(m => m.Inclusions);
        Map(m => m.TravelDates).Name("Dates");
        Map(m => m.Itinerary);
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