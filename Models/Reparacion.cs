using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Reparacion
    {
        [Key]
        public int ReparacionID { get; set; }
        public int UsuarioID { get; set; }
        public int? PropiedadID { get; set; }
        public string Descripcion { get; set; } = null!;
        public int EstadoReparacionID { get; set; }
        public DateTime FechaSolicitud { get; set; }
        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Propiedad? Propiedad { get; set; }
        public EstadoReparacion EstadoReparacion { get; set; } = null!;
    }
}
