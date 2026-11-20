namespace Clinical.Domain.Entities;

public class Cama
{
    public int CamaID { get; set; }
    public int HabitacionID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Habitacion Habitacion { get; set; } = null!;
    public ICollection<Hospitalizacion> Hospitalizaciones { get; set; } = new List<Hospitalizacion>();
}
