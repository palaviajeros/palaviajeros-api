using Palaviajeros.Domain.Entities;

namespace Palaviajeros.Application.Interfaces;

public interface ITravelPackageCsvDeserializer
{
    Task<TravelPackage[]> Deserialize(string csvFileContents);
}

public class TravelPackageCsvDeserializer : ITravelPackageCsvDeserializer
{
    public Task<TravelPackage[]> Deserialize(string csvFileContents)
    {
        // Todo: Parse here using CSV parser
        // domain objects will be generated
        // Implement a serializer somewhere that converts to TravelPackageDto
        throw new NotImplementedException();
    }
}