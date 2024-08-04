using MediatR;
using Palaviajeros.Application;
using Palaviajeros.Application.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationDependencies();
builder.Services.AddAntiforgery();

var app = builder.Build();

app.UseAntiforgery();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/parser/upload",
        async (IFormFile formFile, IMediator mediator) => await mediator.Send(new UploadPackageCsvCommand
        {
            FileStream = formFile.OpenReadStream()
        }))
    .DisableAntiforgery()
    .WithName("PostParserUpload");

app.Run();