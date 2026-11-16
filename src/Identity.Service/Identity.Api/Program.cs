using System.ComponentModel.DataAnnotations;
using System.Text;
using Identity.Core;
using Identity.Core.Exceptions;
using Identity.Core.Options;
using Identity.Persistence.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
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

// Mismos datos que consume Billing.Api y Scheduling.Api para validar los tokens
// que emite este servicio. La clave está replicada en los tres appsettings.json
// (al ser una demo sin secret store); en producción iría en un vault o en claves
// asimétricas con firma privada aquí y pública allá.
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException($"Falta la sección de configuración '{JwtOptions.SectionName}'.");

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
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
                    ? ((int)Errores.TOKEN_VENCIDO, "El token ha expirado. Renove la sesión con el refresh token.")
                    : ((int)Errores.TOKEN_INVALIDO, "Token inválido o ausente. Inicie sesión.");

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { errorCode = code, errorMessage = message });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    errorCode = (int)Errores.SIN_PERMISO,
                    errorMessage = "No tiene permisos para realizar esta operación."
                });
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware global de excepciones: ValidationException/DomainException → 400 con
// { errorCode, errorMessage }; el resto → 500 genérico (sin filtrar internos).
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
            Log.Error(error, "Error no controlado en Identity.Api");

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
