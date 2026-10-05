using Notifications.Core.Exceptions;
using Notifications.Infraestructure.Services;

namespace Notifications.Tests.Services;

public class SimulatedEmailSenderTests
{
    [Fact]
    public async Task EnviarAsync_DevuelveLaNotificacionConMessageIdYDestinatario()
    {
        var sender = new SimulatedEmailSender();

        var notificacion = await sender.EnviarAsync(42, "ana@clinica.test", "Confirmación", "Hola Ana");

        Assert.Equal(42, notificacion.FacturaId);
        Assert.Equal("ana@clinica.test", notificacion.Destinatario);
        Assert.Equal("Confirmación", notificacion.Asunto);
        Assert.StartsWith("<sim-", notificacion.MessageId);
        Assert.NotEqual(default, notificacion.EnviadaUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ana")]
    public async Task EnviarAsync_DestinatarioInvalido_LanzaDomainExceptionDeCorreoNoEnviado(string destinatario)
    {
        var sender = new SimulatedEmailSender();

        var exception = await Assert.ThrowsAsync<DomainException>(
            () => sender.EnviarAsync(42, destinatario, "Confirmación", "Hola"));

        Assert.Equal(Errores.CORREO_NO_ENVIADO, exception.Codigo);
    }

    [Fact]
    public async Task EnviarAsync_Cancelado_PropagaOperationCanceledException()
    {
        var sender = new SimulatedEmailSender();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => sender.EnviarAsync(42, "ana@clinica.test", "Confirmación", "Hola", cts.Token));
    }
}
