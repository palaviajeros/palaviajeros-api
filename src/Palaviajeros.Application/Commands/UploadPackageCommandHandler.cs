using MediatR;
using Palaviajeros.Application.Interfaces;
using Palaviajeros.Application.Models;

namespace Palaviajeros.Application.Commands;

public class UploadPackageCommandHandler(ITravelPackageCsvDeserializer travelPackageCsvDeserializer)
    : IRequestHandler<UploadPackageCsvCommand, CountryPackagesDto>
{
    public async Task<CountryPackagesDto> Handle(UploadPackageCsvCommand request,
        CancellationToken cancellationToken)
    {
        var file = request.FileStream;
        if (file.Length == 0) await Task.FromResult<CountryPackagesDto>(null);

        using var reader = new StreamReader(file);
        var fileContent = await reader.ReadToEndAsync(cancellationToken);

        var travelPackages = await travelPackageCsvDeserializer.Deserialize(fileContent);
        return new CountryPackagesDto("", "", "", travelPackages.Select(tp => TravelPackageDto.FromDomain(tp)));
    }
}