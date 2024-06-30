using CsvHelper.Configuration;

namespace Palaviajeros.Application.Models;

public class CountryPackagesCsvModel
{
    public CountryPackagesCsvModel()
    {
    }

    public CountryPackagesCsvModel(string countryCode, string countryName, string description,
        List<TravelPackageCsvModel> packages)
    {
        CountryCode = countryCode;
        CountryName = countryName;
        Description = description;
        Packages = packages;
    }

    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string Description { get; set; }
    public IEnumerable<TravelPackageCsvModel> Packages { get; set; }
}

public sealed class CountryPackagesMap : ClassMap<CountryPackagesCsvModel>
{
    public CountryPackagesMap()
    {
        Map(m => m.CountryCode).Name("Code");
        Map(m => m.CountryName).Name("Name");
        Map(m => m.Description).Name("Description");
    }
}