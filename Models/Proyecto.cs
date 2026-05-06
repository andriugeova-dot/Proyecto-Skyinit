using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Proyecto
    {
        [Key]
        public int ProyectoID { get; set; }
        public string Nombre { get; set; } = null!;
        public int EstadoProyectoID { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
        public int? ConstructoraID { get; set; }
        // Navegación
        public EstadoProyecto EstadoProyecto { get; set; } = null!;
        public Constructora? Constructora { get; set; }
        public ICollection<Estadistica> Estadisticas { get; set; } = null!;
    }
}
