namespace Clinical.Domain.Entities;

public class Habitacion
{
    public int HabitacionID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Piso { get; set; }
    public int? AreaID { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Cama> Camas { get; set; } = new List<Cama>();
}
