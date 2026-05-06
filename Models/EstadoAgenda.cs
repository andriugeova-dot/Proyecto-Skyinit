using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class EstadoAgenda
    {
        [Key]
        public int EstadoAgendaID { get; set; }
        public string Descripcion { get; set; } = null!;
        // Navegación
        public DbSet<Agenda> Agendas { get; set; } = null!;
    }
}
