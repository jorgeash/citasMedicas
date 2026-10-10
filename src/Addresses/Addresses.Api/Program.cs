using Addresses.Api;
using Addresses.Core;
using Addresses.Persistence;
using Addresses.Persistence.Extensions;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// La libreria NugetPackage_Rest escribe en el logger estatico de Serilog (Log.Logger).
// Si no se configura Serilog, esos registros se descartan en silencio.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers()
    // Newtonsoft: el microservicio de Customers consume esta API con la libreria
    // NugetPackage_Rest, que serializa con Newtonsoft.
    .AddNewtonsoftJson();
builder.Services.AddOpenApi();
builder.Services.AddCore();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<DomainExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();