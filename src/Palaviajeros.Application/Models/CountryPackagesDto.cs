namespace Palaviajeros.Application.Models;

public class CountryPackagesDto
{
    public CountryPackagesDto(string countryCode, string countryName, string description,
        IEnumerable<TravelPackageDto> packages)
    {
        CountryCode = countryCode;
        CountryName = countryName;
        Description = description;
        Packages = packages;
    }

    public string CountryCode { get; }
    public string CountryName { get; }
    public string Description { get; }
    public IEnumerable<TravelPackageDto> Packages { get; }
}