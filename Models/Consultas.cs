using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Consultas
    {
        [Key]
        public int ConsultaId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public int? PropiedadId { get; set; }

        public int? AgenteId { get; set; }

        [Required, StringLength(200)]
        public string Asunto { get; set; } = null!;

        [Required, StringLength(30)]
        public string Estado { get; set; } = "Abierta";

        public DateTime FechaCreacion {  get; set; }=DateTime.Now;

        //Navegacion//
        public Usuario Usuario { get; set; }
        public Propiedad? Propiedad { get; set; }
        
        public Usuario? Agente { get; set; }

        public ICollection<MensajesConsultas> Mensaje { get; set; } = new List<MensajesConsultas>();
    }
}
