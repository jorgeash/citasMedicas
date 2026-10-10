using System.Net;
using Addresses.Core.DTOs;
using Addresses.Core.Exceptions;
using Newtonsoft.Json;

namespace Addresses.Api;

/// <summary>
/// Convierte un DomainException en una respuesta 400 con el sobre HttpResponse, de modo que
/// el cliente recibe un codigo de error de negocio en lugar de un 500 generico.
/// </summary>
public class DomainExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public DomainExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json; charset=utf-8";

            var envelope = HttpResponse<string>.Failure((int)ex.Codigo, ex.Message);

            await context.Response.WriteAsync(JsonConvert.SerializeObject(envelope));
        }
    }
}