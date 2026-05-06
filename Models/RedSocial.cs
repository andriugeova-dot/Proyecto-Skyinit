using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class RedSocial
    {
        [Key]
        public int RedID { get; set; }
        public int UsuarioID { get; set; }
        public int PlataformaID { get; set; }
        public string URL { get; set; } = null!;
        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Plataforma Plataforma { get; set; } = null!;
    }
}
