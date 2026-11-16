using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Scheduling.Core;
using Scheduling.Core.Exceptions;
using Scheduling.Persistence.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

const string FrontendCors = "FrontendCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCors, policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:5173", "http://127.0.0.1:5173" };

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

// Middleware global de excepciones: hoy un `filter`/`orderBy` inválido lanzaba
// ValidationException sin manejador y caía como HTTP 500 sin mensaje.
app.UseExceptionHandler(exceptionHandler =>
{
    exceptionHandler.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var (status, code, message) = error switch
        {
            ValidationException v =>
                (StatusCodes.Status400BadRequest, 400, v.Message),
            DomainException d =>
                (StatusCodes.Status400BadRequest, (int)d.Codigo, d.Message),
            _ =>
                (StatusCodes.Status500InternalServerError, 500, "Error interno del servidor.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            Log.Error(error, "Error no controlado en Scheduling.Api");

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new { errorCode = code, errorMessage = message });
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors(FrontendCors);
app.UseAuthorization();
app.MapControllers();
app.Run();
