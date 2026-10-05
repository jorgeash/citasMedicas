using Notifications.Api;
using Notifications.Core;
using Notifications.Infraestructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// La librería NugetPackage_Rest escribe en el logger estático de Serilog (Log.Logger).
// Si no se configura Serilog, esos registros se descartan en silencio.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers()
    // Newtonsoft: la librería serializa con Newtonsoft, así que la API debe respetar
    // los mismos atributos [JsonProperty] de los DTOs que viajan por HTTP.
    .AddNewtonsoftJson();
builder.Services.AddOpenApi();
builder.Services.AddCore(builder.Configuration);
builder.Services.AddInfraestructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<DomainExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
