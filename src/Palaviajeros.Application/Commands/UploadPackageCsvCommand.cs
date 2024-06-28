using MediatR;
using Palaviajeros.Application.Models;

namespace Palaviajeros.Application.Commands;

public class UploadPackageCsvCommand : IRequest<CountryPackagesDto>
{
    public Stream FileStream { get; set; }
}