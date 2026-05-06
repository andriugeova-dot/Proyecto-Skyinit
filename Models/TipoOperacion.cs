using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class TipoOperacion
    {
        [Key]
        public int TipoOperacionID { get; set; }
        public string Descripcion { get; set; } = null!;
        // Navegación
        public ICollection<Propiedad> Propiedades { get; set; } = null!;
    }
}
