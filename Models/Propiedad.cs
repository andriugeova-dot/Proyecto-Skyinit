using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Propiedad
    {
        [Key]
        public int PropiedadID { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int TipoOperacionID { get; set; }
        public int? Habitaciones { get; set; }
        public string Direccion { get; set; } = null!;
        public string? Ciudad { get; set; }
        public int? ConstructoraID { get; set; }
        public int? AgenteID { get; set; }
        // Navegación
        public TipoOperacion TipoOperacion { get; set; } = null!;
        public Constructora? Constructora { get; set; }
        public Usuario? Agente { get; set; }
        public ICollection<ImagenPropiedad> Imagenes { get; set; } = null!;
        public DbSet<Agenda> Agendas { get; set; } = null!;
        public ICollection<Favorito> Favoritos { get; set; } = null!;
        public ICollection<Comentario> Comentarios { get; set; } = null!;
        public ICollection<HistorialPropiedad> Historial { get; set; } = null!;
        public ICollection<Reparacion> Reparaciones { get; set; } = null!;
        public ICollection<Estadistica> Estadisticas { get; set; } = null!;
    }
}
