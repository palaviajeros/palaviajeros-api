using MediatR;
using Palaviajeros.Application.Interfaces;
using Palaviajeros.Application.Models;

namespace Palaviajeros.Application.Commands;

public class UploadPackageCommandHandler(ICountryPackageCsvDeserializer countryPackageCsvDeserializer)
    : IRequestHandler<UploadPackageCsvCommand, CountryPackagesCsvModel>
{
    public async Task<CountryPackagesCsvModel?> Handle(UploadPackageCsvCommand request,
        CancellationToken cancellationToken)
    {
        var file = request.FileStream;
        if (file.Length == 0) await Task.FromResult<CountryPackagesCsvModel>(null);

        return await countryPackageCsvDeserializer.Deserialize(file);
    }
}