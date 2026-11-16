namespace Scheduling.Core.Exceptions;

public enum Errores
{
    DATOS_INVALIDOS = 901,
    PACIENTE_NO_ENCONTRADO = 902,
    PACIENTE_DUPLICADO = 903,
    ASEGURADORA_NO_ENCONTRADA = 904,
    MEDICO_NO_ENCONTRADO = 905,
    ESTADO_CITA_NO_ENCONTRADO = 906,
    CITA_NO_ENCONTRADA = 907,
    CITA_YA_CANCELADA = 908
}

public class DomainException : Exception
{
    public Errores Codigo { get; }

    public DomainException(Errores codigo, string message) : base(message)
    {
        Codigo = codigo;
    }
}
