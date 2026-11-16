using System.Security.Claims;
using Identity.Core.Features.Auth.Commands.Login;
using Identity.Core.Features.Auth.Commands.Logout;
using Identity.Core.Features.Auth.Commands.Refresh;
using Identity.Core.Features.Auth.Queries.GetMe;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

/// <summary>
/// Endpoints de autenticación. <c>login</c> y <c>refresh</c> son anónimos;
/// <c>logout</c> y <c>me</c> exigen un JWT de acceso válido.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Intercambia usuario + contraseña por access y refresh token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        // IP y dispositivo reales de la petición, para la bitácora de sesiones.
        command.DireccionIP = HttpContext.Connection.RemoteIpAddress?.ToString();
        command.Dispositivo = Request.Headers.UserAgent.ToString();

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Cambia un refresh token vencido por uno nuevo.</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>Cierra la sesión en la bitácora. El access token sigue válido hasta vencer.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand { UsuarioID = UsuarioId() });
        return NoContent();
    }

    /// <summary>Perfil del usuario autenticado, con rol y permisos.</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var result = await _mediator.Send(new GetMeQuery { UsuarioID = UsuarioId() });
        return Ok(result);
    }

    /// <summary>
    /// Id del usuario del JWT. <c>MapInboundClaims=false</c> mantiene el claim
    /// tal cual se emitió, así que se lee por su nombre literal "sub".
    /// </summary>
    private int UsuarioId()
    {
        var sub = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(sub, out var id) ? id : 0;
    }
}
