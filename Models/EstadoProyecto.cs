using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class EstadoProyecto
    {
        [Key]
        public int EstadoProyectoID { get; set; }
        public string Descripcion { get; set; } = null!;
        // Navegación
        public ICollection<Proyecto> Proyectos { get; set; } = null!;
    }
}
