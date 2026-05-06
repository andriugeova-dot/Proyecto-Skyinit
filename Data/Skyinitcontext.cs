// Data/SkyinitContext.cs
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Data
{
    public class SkyinitContext : DbContext
    {
        public SkyinitContext(DbContextOptions<SkyinitContext> options)
            : base(options) { }

        // ── Entidades principales ─────────────────────────
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Propiedad> Propiedades { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Constructora> Constructoras { get; set; }
        public DbSet<Reparacion> Reparaciones { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<DetalleReporte> DetalleReportes { get; set; }
        public DbSet<Estadistica> Estadisticas { get; set; }
        public DbSet<ImagenPropiedad> ImagenesPropiedad { get; set; }
        public DbSet<HistorialPropiedad> HistorialPropiedades { get; set; }
        public DbSet<RedSocial> RedesSociales { get; set; }

        // ── Catálogos (E04 — faltaban) ───────────────────
        public DbSet<Plataforma> Plataformas { get; set; }
        public DbSet<TipoOperacion> TiposOperacion { get; set; }
        public DbSet<EstadoAgenda> EstadosAgenda { get; set; }
        public DbSet<EstadoProyecto> EstadosProyecto { get; set; }
        public DbSet<EstadoReparacion> EstadosReparacion { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // E03 — Columna BD con tilde ↔ propiedad C# sin tilde
            mb.Entity<Usuario>()
              .Property(u => u.ContrasenaHash)
              .HasColumnName("ContraseñaHash");

            // Índice único: un usuario no puede tener el mismo favorito dos veces
            mb.Entity<Favorito>()
              .HasIndex(f => new { f.UsuarioID, f.PropiedadID })
              .IsUnique();

            // Índice único: token de sesión
            mb.Entity<Sesion>()
              .HasIndex(s => s.Token)
              .IsUnique();
        }
    }
}