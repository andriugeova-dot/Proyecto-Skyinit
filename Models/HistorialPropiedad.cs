using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class HistorialPropiedad
    {
        [Key]
        public int HistorialID { get; set; }
        public int UsuarioID { get; set; }
        public int PropiedadID { get; set; }
        public DateTime FechaAcceso { get; set; }
        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Propiedad Propiedad { get; set; } = null!;
    }
}
