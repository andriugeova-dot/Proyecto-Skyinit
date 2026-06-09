using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Proyecto_SkyInit.Models
{
    public class Propiedad
    {
        [Key]
        public int PropiedadID { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El tipo de operación es obligatorio")]
        public int TipoOperacionID { get; set; }

        [Range(0, 99, ErrorMessage = "El número de habitaciones no es válido")]
        public int? Habitaciones { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]

        [NotMapped]
        public bool EsFavorito { get; set; }
        public string Direccion { get; set; } = null!;

        public string? Ciudad { get; set; }

        public int? ConstructoraID { get; set; }

        public int? AgenteID { get; set; }

        // ── Propiedades de navegación — NO validar desde el formulario ──────
        [ValidateNever]
        public TipoOperacion TipoOperacion { get; set; } = null!;

        [ValidateNever]
        public Constructora? Constructora { get; set; }

        [ValidateNever]
        public Usuario? Agente { get; set; }

        [ValidateNever]
        public ICollection<ImagenPropiedad> Imagenes { get; set; } = new List<ImagenPropiedad>();

        [ValidateNever]
        public ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();

        [ValidateNever]
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

        [ValidateNever]
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

        [ValidateNever]
        public ICollection<HistorialPropiedad> Historial { get; set; } = new List<HistorialPropiedad>();

        [ValidateNever]
        public ICollection<Reparacion> Reparaciones { get; set; } = new List<Reparacion>();

        [ValidateNever]
        public ICollection<Estadistica> Estadisticas { get; set; } = new List<Estadistica>();
    }
}