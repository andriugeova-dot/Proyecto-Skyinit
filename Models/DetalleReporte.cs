using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class DetalleReporte
    {
        [Key]
        public int DetalleID { get; set; }
        public int ReporteID { get; set; }
        public string Descripcion { get; set; } = null!;
        // Navegación
        public Reporte Reporte { get; set; } = null!;
    }
}
