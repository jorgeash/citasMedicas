namespace Domain.Billing.Entities;

public class Tarifa
{
    public int TarifaID { get; set; }
    public int ServicioID { get; set; }
    public int? AseguradoraID { get; set; }
    public string NombreTarifa { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public bool Activa { get; set; }

    public Servicio? Servicio { get; set; }
}
