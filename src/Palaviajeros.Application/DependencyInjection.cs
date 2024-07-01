using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Palaviajeros.Application.Interfaces;

namespace Palaviajeros.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
        services.AddTransient<ITravelPackageCsvDeserializer, TravelPackageCsvDeserializer>();
        services.AddTransient<ICountryPackageCsvDeserializer, CountryPackageCsvDeserializer>();
        return services;
    }
}