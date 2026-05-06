using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class EstadoReparacion
    {
        [Key]
        public int EstadoReparacionID { get; set; }
        public string Descripcion { get; set; } = null!;
        // Navegación
        public ICollection<Reparacion> Reparaciones { get; set; } = null!;
    }
}
