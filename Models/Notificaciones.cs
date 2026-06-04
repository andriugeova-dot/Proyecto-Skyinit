using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Notificaciones
    {
        [Key]
        public int NotificacionId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required, StringLength(150)]
        public string Titulo { get; set; } = null!;

        [Required]
        public string Mensaje { get; set; } = null!;

        public bool Leida { get; set; }=false;

        public DateTime FechaCreacion { get; set; }=DateTime.Now;

        [Required, StringLength(50)]
        public string TipoNotificacion { get; set; } = null!;

        public int? EntidadId { get; set; }
        
        public string? EntidadTipo {  get; set; }

        //Navegacion//
        public Usuario Usuario { get; set; }
    }
}
