using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Constructora
    {
        [Key]
        public int ConstructoraID { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        // Navegación
        public ICollection<Propiedad> Propiedades { get; set; } = null!;
        public ICollection<Proyecto> Proyectos { get; set; } = null!;
    }
}
