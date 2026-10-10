using Business.RestClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;
using Serilog;

// Direcciones base de los dos microservicios. Se pueden pasar como argumentos:
//   dotnet run --project Business.RestClient -- http://localhost:5310 http://localhost:5320
var addressesBaseUrl = args.Length > 0 ? args[0] : "http://localhost:5310";
var customersBaseUrl = args.Length > 1 ? args[1] : "http://localhost:5320";

// La libreria NugetPackage_Rest escribe en el logger estatico de Serilog. Sin Serilog, los
// registros de cada request se perderian.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

Console.WriteLine("=====================================================================");
Console.WriteLine(" CLIENTE DE LA ACTIVIDAD - Todos los request usan la libreria");
Console.WriteLine(" NugetPackage_Rest (IRest / RestBuilder)");
Console.WriteLine("=====================================================================");
Console.WriteLine($" Addresses.Api : {addressesBaseUrl}");
Console.WriteLine($" Business.Api  : {customersBaseUrl}");
Console.WriteLine();

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        // RestSettings: activa el log de cada request de la libreria (metodo, URL y body)
        ["RestSettings:EnableRequestLogs"] = "true",
        ["RestSettings:JsonSerializer"] = "Newtonsoft"
    })
    .Build();

var services = new ServiceCollection();
services.AddLogging(builder => builder.ClearProviders());
services.AddRequestLogging(configuration);
services.AddHttpClient<IRest, RestBuilder>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("X-Origen", "RestClient");
});

await using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var rest = scope.ServiceProvider.GetRequiredService<IRest>();
var client = new RestDemoClient(rest, addressesBaseUrl, customersBaseUrl);

// ---------------------------------------------------------------------------
// Paso 1. Catalogo de direcciones del microservicio de Addresses (IRest.Get)
// ---------------------------------------------------------------------------
Console.WriteLine("PASO 1  GET  {Addresses}/api/positions?pageNumber=1&pageSize=50&orderBy=Id");
var direcciones = await client.ListarDireccionesAsync(50);

Console.WriteLine($"        Tabla Addresses_Position: {direcciones.Count} direcciones de prueba");
foreach (var d in direcciones)
{
    Console.WriteLine($"        [LocationId {d.Id}] {d.Address} | {d.CityName}, {d.CountryName} | CP {d.ZipCode} | " +
                      $"Lat {d.Latitude:F4} Lon {d.Longitude:F4} | CustomerId {d.CustomerId?.ToString() ?? "null"}");
}

if (direcciones.Count == 0)
{
    Console.WriteLine("ERROR: no hay direcciones en el catalogo. Ejecuta primero las migraciones de Addresses.");
    return 1;
}

// ---------------------------------------------------------------------------
// Paso 2. Crear dos clientes con IRest.Post, cada uno con su LocationId
// ---------------------------------------------------------------------------
var nuevosClientes = new List<NewCustomerRequest>
{
    new()
    {
        Code = "CLI-0002",
        Name = "Clinica Vida Salud S.A.",
        Email = "facturacion@clinicavidasalud.example",
        CustomerTypeId = 1,
        CompanyId = 1,
        LocationId = direcciones[0].Id
    },
    new()
    {
        Code = "CLI-0003",
        Name = "EPS-total Salud S.A.",
        Email = "pagos@epstotal.example",
        CustomerTypeId = 3,
        CompanyId = 3,
        LocationId = direcciones.Count > 2 ? direcciones[2].Id : direcciones.Last().Id
    }
};

var creados = new List<CustomerContract>();

foreach (var nuevo in nuevosClientes)
{
    var locationPrevia = await client.ObtenerDireccionAsync(nuevo.LocationId);

    Console.WriteLine();
    Console.WriteLine($"PASO 2  POST {customersBaseUrl}/api/customers   (IRest.Post.WithBody)");
    Console.WriteLine($"        Body: {{ \"Code\": \"{nuevo.Code}\", \"Name\": \"{nuevo.Name}\", " +
                      $"\"CustomerTypeId\": {nuevo.CustomerTypeId}, \"CompanyId\": {nuevo.CompanyId}, " +
                      $"\"LocationId\": {nuevo.LocationId} }}");
    Console.WriteLine($"        LocationId {nuevo.LocationId} resuelve a: " +
                      $"\"{locationPrevia?.Address}\" ({locationPrevia?.CityName})");

    var cliente = await client.CrearClienteAsync(nuevo);
    creados.Add(cliente);

    Console.WriteLine($"        Respuesta 201: Cliente Id {cliente.Id} | {cliente.Code} | {cliente.Name} | " +
                      $"Tipo {cliente.CustomerTypeId} ({cliente.CustomerTypeName}) | Empresa {cliente.CompanyId} ({cliente.CompanyName})");
    Console.WriteLine($"        LocationId persistido: {cliente.LocationId} -> {cliente.Location?.Address}, " +
                      $"{cliente.Location?.CityName}, {cliente.Location?.CountryName}");
}

// ---------------------------------------------------------------------------
// Paso 3. Relectura de los clientes creados (IRest.Get) para confirmar la persistencia
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("PASO 3  GET  {Customers}/api/customers/{id}   (verificacion de la persistencia)");

foreach (var creado in creados)
{
    var releido = await client.ObtenerClienteAsync(creado.Id);

    Console.WriteLine($"        Cliente {releido?.Id}: {releido?.Code} | {releido?.Name} | " +
                      $"LocationId {releido?.LocationId} | direccion \"{releido?.Location?.Address}\" " +
                      $"({releido?.Location?.CityName}) | IsActive {releido?.IsActive} | " +
                      $"creado {releido?.CreatedAt:yyyy-MM-dd HH:mm:ss}");
}

// ---------------------------------------------------------------------------
// Resumen
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("=====================================================================");
Console.WriteLine($" RESULTADO: {creados.Count} clientes creados con la libreria REST");
Console.WriteLine($"   Cliente {creados[0].Id} ({creados[0].Code}) -> LocationId {creados[0].LocationId}");
Console.WriteLine($"   Cliente {creados[1].Id} ({creados[1].Code}) -> LocationId {creados[1].LocationId}");
Console.WriteLine("=====================================================================");

return 0;