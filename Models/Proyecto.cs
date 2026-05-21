using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Proyecto
    {
        [Key]
        public int ProyectoID { get; set; }
        public string Nombre { get; set; } = null!;
        public int EstadoProyectoID { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? ConstructoraID { get; set; }
        // Navegación
        public EstadoProyecto? EstadoProyecto { get; set; }
        public Constructora? Constructora { get; set; }
        public ICollection<Estadistica>? Estadisticas { get; set; }
    }
}
