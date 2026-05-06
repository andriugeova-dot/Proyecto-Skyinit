using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class ImagenPropiedad
    {
        [Key]
        public int ImagenID { get; set; }
        public int PropiedadID { get; set; }
        public string URL { get; set; } = null!;
        // Navegación
        public Propiedad Propiedad { get; set; } = null!;
    }
}
