using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Reporte
    {
        [Key]
        public int ReporteID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaReporte { get; set; }
        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public ICollection<DetalleReporte> Detalles { get; set; } = null!;
    }
}
