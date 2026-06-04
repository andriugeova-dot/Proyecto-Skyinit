using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Calificaciones
    {
        [Key]
        public int CalificacionId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int PropiedadId { get; set; }

        [Range(1,5)]
        public int Puntaje { get; set; }

        public DateTime FechaCalificacion { get; set; } = DateTime.Now;

        //Navegacion//
        public Usuario Usuario { get; set; }
        public Propiedad Propiedad { get; set; }
    }
}