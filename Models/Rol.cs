using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Rol
    {
        [Key]
        public int RolID { get; set; }
        public string NombreRol { get; set; } = null!;
        // Navegación
        public ICollection<Usuario> Usuarios { get; set; } = null!;
    }
}
