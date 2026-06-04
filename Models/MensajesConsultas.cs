using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class MensajesConsultas
    {
        [Key]
        public int MensajeId { get; set; }

        [Required]
        public int ConsultasId { get; set; }

        [Required]
        public int RemitenteId { get; set; }

        [Required]
        public string Contenido { get; set; }

        public DateTime FechaEnvio { get; set; }=DateTime.Now;

        public bool Leido {  get; set; }=false;

        //Navegacion//
        public Consultas Consultas { get; set; }
        public Usuario Remitente { get; set; }
    }
}
