using Palaviajeros.Domain.Models;
using Palaviajeros.Domain.ValueObjects;

namespace Palaviajeros.Domain.Entities;

public class TravelPackage
{
    public Country Country { get; }
    public string Id { get; }
    public string Name { get; }
    public int Description { get; }
    public List<Amenity> Inclusions { get; }
    public List<Amenity> Exclusions { get; }
    public List<DateRange> DataRanges { get; }
    public List<DayPlan> Itinerary { get; }
}