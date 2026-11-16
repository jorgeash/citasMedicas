namespace Identity.Core.Exceptions;

/// <summary>
/// Rango de errores de Identity: 701–708. Coherente con Billing (801–810)
/// y Scheduling (901–908): cada servicio dueño de su propia franja.
///
/// 701 credenciales · 702 usuario inactivo · 703 refresh inválido/vencido ·
/// 704 no autenticado (sin token) · 705 token expirado · 706 token inválido ·
/// 707 sin permiso · 708 solicitud malformada.
/// </summary>
public enum Errores
{
    CREDENCIALES_INVALIDAS = 701,
    USUARIO_INACTIVO = 702,
    REFRESH_INVALIDO = 703,
    NO_AUTENTICADO = 704,
    TOKEN_VENCIDO = 705,
    TOKEN_INVALIDO = 706,
    SIN_PERMISO = 707,
    DATOS_INVALIDOS = 708
}

public class DomainException : Exception
{
    public Errores Codigo { get; }

    public DomainException(Errores codigo, string message) : base(message)
    {
        Codigo = codigo;
    }
}
