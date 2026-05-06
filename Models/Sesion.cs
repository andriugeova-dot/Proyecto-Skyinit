using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Sesion
    {
        [Key]
        public int SesionID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Token { get; set; } = null!;
        // Navegación
        public Usuario Usuario { get; set; } = null!;
    }
}
