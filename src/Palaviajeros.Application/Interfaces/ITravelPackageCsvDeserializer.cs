using Palaviajeros.Application.Models;

namespace Palaviajeros.Application.Interfaces;

public interface ITravelPackageCsvDeserializer
{
    Task<TravelPackageCsvModel[]> Deserialize(string csvFileContents);
}

public class TravelPackageCsvDeserializer : ITravelPackageCsvDeserializer
{
    public Task<TravelPackageCsvModel[]> Deserialize(string csvFileContents)
    {
        // Todo: Parse here using CSV parser
        // domain objects will be generated
        // Implement a serializer somewhere that converts to TravelPackageDto
        throw new NotImplementedException();
    }
}