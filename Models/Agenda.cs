using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Agenda
    {
        [Key]
        public int AgendaID { get; set; }
        public int UsuarioID { get; set; }
        public int PropiedadID { get; set; }
        public DateTime FechaVisita { get; set; }
        public int EstadoAgendaID { get; set; }
        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Propiedad Propiedad { get; set; } = null!;
        public EstadoAgenda EstadoAgenda { get; set; } = null!;
    }
}
