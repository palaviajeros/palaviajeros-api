using Palaviajeros.Domain.Entities;

namespace Palaviajeros.Domain.Models;

public class Country : Entity
{
    public string CountryCode { get; }
    public string Name { get; }
    public string Description { get; }
}