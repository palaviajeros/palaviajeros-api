using Palaviajeros.Domain.Models;

namespace Palaviajeros.Application.Interfaces;

public interface ICountryPackageCsvDeserializer
{
    Task<Country> Deserialize(string countryCsvContent);
}

public class CountryPackageCsvDeserializer : ICountryPackageCsvDeserializer
{
    public Task<Country> Deserialize(string countryCsvContent)
    {
        throw new NotImplementedException();
    }
}