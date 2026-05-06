using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Favorito
    {
        [Key]
        public int FavoritoID { get; set; }
        public int UsuarioID { get; set; }
        public int PropiedadID { get; set; }
        public DateTime FechaAgregado { get; set; }
        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Propiedad Propiedad { get; set; } = null!;
    }
}
