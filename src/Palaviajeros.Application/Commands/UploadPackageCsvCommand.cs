using MediatR;
using Palaviajeros.Application.Models;

namespace Palaviajeros.Application.Commands;

public class UploadPackageCsvCommand : IRequest<CountryPackagesCsvModel>
{
    public Stream FileStream { get; set; }
}