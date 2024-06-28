using Palaviajeros.Domain.Entities;

namespace Palaviajeros.Application.Models;

public class TravelPackageDto
{
    public static TravelPackageDto FromDomain(TravelPackage travelPackage)
    {
        return new TravelPackageDto();
    }
}