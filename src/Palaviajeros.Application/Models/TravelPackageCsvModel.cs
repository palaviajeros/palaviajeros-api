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

    // # of days are defined by Itinerary length
    public int Days => Itinerary.Length;
    public DateOnly[] TravelDates { get; set; }
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
            TravelDates = travelPackage.TravelDates.Select(td => td).ToArray(),
            Itinerary = travelPackage.Itinerary.ToArray()
        };
    }
}