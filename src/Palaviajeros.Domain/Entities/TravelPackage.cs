using Palaviajeros.Domain.Models;
using Palaviajeros.Domain.ValueObjects;

namespace Palaviajeros.Domain.Entities;

public class TravelPackage : Entity
{
    public Country Country { get; }
    public string PackageCode { get; }
    public string Name { get; }
    public string[] Description { get; }
    public IEnumerable<Services> Inclusions { get; }
    public IEnumerable<DateRange> DataRanges { get; }
    public IEnumerable<DayPlan> Itinerary { get; }
}