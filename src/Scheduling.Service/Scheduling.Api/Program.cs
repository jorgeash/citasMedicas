using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
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

// Autenticación JWT emitida por Identity.Api (:52096). La clave y el emisor
// están replicados en los tres appsettings.json (demo sin secret store).
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtIssuer = jwtSection["Issuer"] ?? string.Empty;
var jwtAudience = jwtSection["Audience"] ?? string.Empty;
var jwtKey = jwtSection["SigningKey"]
    ?? throw new InvalidOperationException("Falta Jwt:SigningKey en appsettings.json.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        // El 401 por defecto sale vacío; se respeta el contrato { errorCode, errorMessage }.
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                var expirado = context.ErrorDescription?.Contains("expired", StringComparison.OrdinalIgnoreCase) == true
                    || context.Error == "token_expired";

                var (code, message) = expirado
                    ? (705, "El token ha expirado. Renove la sesión con el refresh token.")
                    : (706, "Token inválido o ausente. Inicie sesión.");

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { errorCode = code, errorMessage = message });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    errorCode = 707,
                    errorMessage = "No tiene permisos para realizar esta operación."
                });
            }
        };
    });

// Todo endpoint exige usuario autenticado salvo que diga [AllowAnonymous].
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
