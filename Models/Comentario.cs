using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Comentario
    {
        [Key]
        public int ComentarioID { get; set; }
        public int UsuarioID { get; set; }
        public int PropiedadID { get; set; }
        public string Contenido { get; set; } = null!;
        public DateTime FechaComentario { get; set; }
        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Propiedad Propiedad { get; set; } = null!;
    }
}
