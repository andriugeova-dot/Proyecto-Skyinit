using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Plataforma
    {
        [Key]
        public int PlataformaID { get; set; }
        public string NombrePlataforma { get; set; } = null!;
        // Navegación
        public ICollection<RedSocial> RedesSociales { get; set; } = null!;
    }
}
