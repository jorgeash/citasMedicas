using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Notification.Core;
using Notification.Core.Exceptions;
using Notification.Persistence.Extensions;
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

        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();

                var expirado = context.ErrorDescription?.Contains("expired", StringComparison.OrdinalIgnoreCase) == true
                    || context.Error == "token_expired";

                var (code, message) = expirado
                    ? ((int)Errores.TOKEN_EXPIRADO, "La sesión ha expirado. Vuelva a iniciar sesión.")
                    : ((int)Errores.TOKEN_INVALIDO, "Token inválido o ausente. Autentíquese para continuar.");

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { errorCode = code, errorMessage = message });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    errorCode = (int)Errores.SIN_PERMISOS,
                    errorMessage = "No tiene permisos para consultar o gestionar notificaciones."
                });
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

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
            Log.Error(error, "Error no controlado en Notification.Api");

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
