using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string ContrasenaHash { get; set; } = null!;
        public string? Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string EstadoCuenta { get; set; } = null!;
        public int RolID { get; set; }
        // Navegación
        public Rol Rol { get; set; } = null!;
        public ICollection<Sesion> Sesiones { get; set; } = null!;
        public ICollection<RedSocial> RedesSociales { get; set; } = null!;
        public ICollection<Agenda> Agendas { get; set; } = null!;
        public ICollection<Favorito> Favoritos { get; set; } = null!;
        public ICollection<Comentario> Comentarios { get; set; } = null!;
        public ICollection<HistorialPropiedad> Historial { get; set; } = null!;
        public ICollection<Reparacion> Reparaciones { get; set; } = null!;
        public ICollection<Reporte> Reportes { get; set; } = null!;
        public ICollection<Propiedad> PropiedadesComoAgente { get; set; } = null!;
    }
}
