
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;

    namespace Proyecto_SkyInit.Models
    {
  
        public class SkyInitContext : DbContext
        {
            public SkyInitContext(DbContextOptions<SkyInitContext> options) : base(options) { }

            // --- Catálogos ---
            public DbSet<Rol> Roles { get; set; }
            public DbSet<TipoOperacion> TiposOperacion { get; set; }
            public DbSet<EstadoAgenda> EstadosAgenda { get; set; }
            public DbSet<EstadoProyecto> EstadosProyecto { get; set; }
            public DbSet<EstadoReparacion> EstadosReparacion { get; set; }
            public DbSet<Plataforma> Plataformas { get; set; }

            // --- Entidades principales ---
            public DbSet<Usuario> Usuarios { get; set; }
            public DbSet<Constructora> Constructoras { get; set; }
            public DbSet<Propiedad> Propiedades { get; set; }
            public DbSet<Proyecto> Proyectos { get; set; }

            // --- Actividad de usuarios ---
            public DbSet<Sesion> Sesiones { get; set; }
            public DbSet<RedSocial> RedesSociales { get; set; }
            public DbSet<Agenda> Agendas { get; set; }
            public DbSet<Favorito> Favoritos { get; set; }
            public DbSet<Comentario> Comentarios { get; set; }
            public DbSet<HistorialPropiedad> HistorialPropiedades { get; set; }
            public DbSet<ImagenPropiedad> ImagenesPropiedades { get; set; }

            // --- Soporte y reportes ---
            public DbSet<Reparacion> Reparaciones { get; set; }
            public DbSet<Reporte> Reportes { get; set; }
            public DbSet<DetalleReporte> DetallesReporte { get; set; }
            public DbSet<Estadistica> Estadisticas { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // ---- roles ----
                modelBuilder.Entity<Rol>(e =>
                {
                    e.ToTable("roles");
                    e.HasKey(x => x.RolID);
                    e.Property(x => x.NombreRol).IsRequired().HasMaxLength(50);
                    e.HasIndex(x => x.NombreRol).IsUnique();
                });

                // ---- usuarios ----
                modelBuilder.Entity<Usuario>(e =>
                {
                    e.ToTable("usuarios");
                    e.HasKey(x => x.UsuarioID);
                    e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                    e.Property(x => x.Correo).IsRequired().HasMaxLength(100);
                    e.HasIndex(x => x.Correo).IsUnique();
                    e.Property(x => x.ContraseñaHash).IsRequired().HasMaxLength(255);
                    e.Property(x => x.Telefono).HasMaxLength(20);
                    e.Property(x => x.FechaRegistro).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                    e.Property(x => x.EstadoCuenta).IsRequired().HasMaxLength(20).HasDefaultValue("Activa");
                    e.HasOne(x => x.Rol)
                        .WithMany(r => r.Usuarios)
                        .HasForeignKey(x => x.RolID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- sesiones ----
                modelBuilder.Entity<Sesion>(e =>
                {
                    e.ToTable("sesiones");
                    e.HasKey(x => x.SesionID);
                    e.Property(x => x.Token).IsRequired().HasMaxLength(255);
                    e.HasIndex(x => x.Token).IsUnique();
                    e.Property(x => x.FechaInicio).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.Sesiones)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- plataformas ----
                modelBuilder.Entity<Plataforma>(e =>
                {
                    e.ToTable("plataformas");
                    e.HasKey(x => x.PlataformaID);
                    e.Property(x => x.NombrePlataforma).IsRequired().HasMaxLength(50);
                    e.HasIndex(x => x.NombrePlataforma).IsUnique();
                });

                // ---- redessociales ----
                modelBuilder.Entity<RedSocial>(e =>
                {
                    e.ToTable("redessociales");
                    e.HasKey(x => x.RedID);
                    e.Property(x => x.URL).IsRequired().HasMaxLength(255);
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.RedesSociales)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.Plataforma)
                        .WithMany(p => p.RedesSociales)
                        .HasForeignKey(x => x.PlataformaID)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // ---- tiposoperacion ----
                modelBuilder.Entity<TipoOperacion>(e =>
                {
                    e.ToTable("tiposoperacion");
                    e.HasKey(x => x.TipoOperacionID);
                    e.Property(x => x.Descripcion).IsRequired().HasMaxLength(50);
                    e.HasIndex(x => x.Descripcion).IsUnique();
                });

                // ---- constructoras ----
                modelBuilder.Entity<Constructora>(e =>
                {
                    e.ToTable("constructoras");
                    e.HasKey(x => x.ConstructoraID);
                    e.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
                    e.Property(x => x.Contacto).HasMaxLength(100);
                    e.Property(x => x.Telefono).HasMaxLength(20);
                    e.Property(x => x.Correo).HasMaxLength(100);
                });

                // ---- propiedades ----
                modelBuilder.Entity<Propiedad>(e =>
                {
                    e.ToTable("propiedades");
                    e.HasKey(x => x.PropiedadID);
                    e.Property(x => x.Titulo).IsRequired().HasMaxLength(150);
                    e.Property(x => x.Descripcion);
                    e.Property(x => x.Precio).IsRequired().HasColumnType("decimal(18,2)");
                    e.Property(x => x.Direccion).IsRequired().HasMaxLength(255);
                    e.Property(x => x.Ciudad).HasMaxLength(100);
                    e.HasOne(x => x.TipoOperacion)
                        .WithMany(t => t.Propiedades)
                        .HasForeignKey(x => x.TipoOperacionID)
                        .OnDelete(DeleteBehavior.Restrict);
                    e.HasOne(x => x.Constructora)
                        .WithMany(c => c.Propiedades)
                        .HasForeignKey(x => x.ConstructoraID)
                        .OnDelete(DeleteBehavior.SetNull);
                    e.HasOne(x => x.Agente)
                        .WithMany(u => u.PropiedadesComoAgente)
                        .HasForeignKey(x => x.AgenteID)
                        .OnDelete(DeleteBehavior.SetNull);
                });

                // ---- imagenespropiedad ----
                modelBuilder.Entity<ImagenPropiedad>(e =>
                {
                    e.ToTable("imagenespropiedad");
                    e.HasKey(x => x.ImagenID);
                    e.Property(x => x.URL).IsRequired().HasMaxLength(255);
                    e.HasOne(x => x.Propiedad)
                        .WithMany(p => p.Imagenes)
                        .HasForeignKey(x => x.PropiedadID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- estadosproyecto ----
                modelBuilder.Entity<EstadoProyecto>(e =>
                {
                    e.ToTable("estadosproyecto");
                    e.HasKey(x => x.EstadoProyectoID);
                    e.Property(x => x.Descripcion).IsRequired().HasMaxLength(50);
                    e.HasIndex(x => x.Descripcion).IsUnique();
                });

                // ---- proyectos ----
                modelBuilder.Entity<Proyecto>(e =>
                {
                    e.ToTable("proyectos");
                    e.HasKey(x => x.ProyectoID);
                    e.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
                    e.HasOne(x => x.EstadoProyecto)
                        .WithMany(ep => ep.Proyectos)
                        .HasForeignKey(x => x.EstadoProyectoID)
                        .OnDelete(DeleteBehavior.Restrict);
                    e.HasOne(x => x.Constructora)
                        .WithMany(c => c.Proyectos)
                        .HasForeignKey(x => x.ConstructoraID)
                        .OnDelete(DeleteBehavior.SetNull);
                });

                // ---- estadosagenda ----
                modelBuilder.Entity<EstadoAgenda>(e =>
                {
                    e.ToTable("estadosagenda");
                    e.HasKey(x => x.EstadoAgendaID);
                    e.Property(x => x.Descripcion).IsRequired().HasMaxLength(50);
                    e.HasIndex(x => x.Descripcion).IsUnique();
                });

                // ---- agendas ----
                modelBuilder.Entity<Agenda>(e =>
                {
                    e.ToTable("agendas");
                    e.HasKey(x => x.AgendaID);
                    e.Property(x => x.FechaVisita).IsRequired();
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.Agendas)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.Propiedad)
                        .WithMany(p => p.Agendas)
                        .HasForeignKey(x => x.PropiedadID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.EstadoAgenda)
                        .WithMany(ea => ea.Agendas)
                        .HasForeignKey(x => x.EstadoAgendaID)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // ---- favoritos ----
                modelBuilder.Entity<Favorito>(e =>
                {
                    e.ToTable("favoritos");
                    e.HasKey(x => x.FavoritoID);
                    e.Property(x => x.FechaAgregado).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                    e.HasIndex(x => new { x.UsuarioID, x.PropiedadID }).IsUnique();
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.Favoritos)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.Propiedad)
                        .WithMany(p => p.Favoritos)
                        .HasForeignKey(x => x.PropiedadID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- comentarios ----
                modelBuilder.Entity<Comentario>(e =>
                {
                    e.ToTable("comentarios");
                    e.HasKey(x => x.ComentarioID);
                    e.Property(x => x.Contenido).IsRequired();
                    e.Property(x => x.FechaComentario).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.Comentarios)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.Propiedad)
                        .WithMany(p => p.Comentarios)
                        .HasForeignKey(x => x.PropiedadID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- historialpropiedades ----
                modelBuilder.Entity<HistorialPropiedad>(e =>
                {
                    e.ToTable("historialpropiedades");
                    e.HasKey(x => x.HistorialID);
                    e.Property(x => x.FechaAcceso).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.Historial)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.Propiedad)
                        .WithMany(p => p.Historial)
                        .HasForeignKey(x => x.PropiedadID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- estadosreparacion ----
                modelBuilder.Entity<EstadoReparacion>(e =>
                {
                    e.ToTable("estadosreparacion");
                    e.HasKey(x => x.EstadoReparacionID);
                    e.Property(x => x.Descripcion).IsRequired().HasMaxLength(50);
                    e.HasIndex(x => x.Descripcion).IsUnique();
                });

                // ---- reparaciones ----
                modelBuilder.Entity<Reparacion>(e =>
                {
                    e.ToTable("reparaciones");
                    e.HasKey(x => x.ReparacionID);
                    e.Property(x => x.Descripcion).IsRequired();
                    e.Property(x => x.FechaSolicitud).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.Reparaciones)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.Propiedad)
                        .WithMany(p => p.Reparaciones)
                        .HasForeignKey(x => x.PropiedadID)
                        .OnDelete(DeleteBehavior.SetNull);
                    e.HasOne(x => x.EstadoReparacion)
                        .WithMany(er => er.Reparaciones)
                        .HasForeignKey(x => x.EstadoReparacionID)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // ---- reportes ----
                modelBuilder.Entity<Reporte>(e =>
                {
                    e.ToTable("reportes");
                    e.HasKey(x => x.ReporteID);
                    e.Property(x => x.FechaReporte).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                    e.HasOne(x => x.Usuario)
                        .WithMany(u => u.Reportes)
                        .HasForeignKey(x => x.UsuarioID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- detallereporte ----
                modelBuilder.Entity<DetalleReporte>(e =>
                {
                    e.ToTable("detallereporte");
                    e.HasKey(x => x.DetalleID);
                    e.Property(x => x.Descripcion).IsRequired();
                    e.HasOne(x => x.Reporte)
                        .WithMany(r => r.Detalles)
                        .HasForeignKey(x => x.ReporteID)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // ---- estadisticas ----
                modelBuilder.Entity<Estadistica>(e =>
                {
                    e.ToTable("estadisticas");
                    e.HasKey(x => x.EstadisticaID);
                    e.Property(x => x.Visitas).HasDefaultValue(0);
                    e.Property(x => x.Favoritos).HasDefaultValue(0);
                    e.Property(x => x.Comentarios).HasDefaultValue(0);
                    e.HasOne(x => x.Propiedad)
                        .WithMany(p => p.Estadisticas)
                        .HasForeignKey(x => x.PropiedadID)
                        .OnDelete(DeleteBehavior.Cascade);
                    e.HasOne(x => x.Proyecto)
                        .WithMany(p => p.Estadisticas)
                        .HasForeignKey(x => x.ProyectoID)
                        .OnDelete(DeleteBehavior.Cascade);
                });
            }
        }


        // =============================================
        //  MODELOS
        // =============================================

        public class Rol
        {
            public int RolID { get; set; }
            public string NombreRol { get; set; } = null!;
            // Navegación
            public ICollection<Usuario> Usuarios { get; set; } = null!;
        }

        public class Usuario
        {
            public int UsuarioID { get; set; }
            public string Nombre { get; set; } = null!;
            public string Correo { get; set; } = null!;
            public string ContraseñaHash { get; set; } = null!;
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
            public ICollection<Comentario> Comentarios { get; set; }= null!;
            public ICollection<HistorialPropiedad> Historial { get; set; } = null!;
            public ICollection<Reparacion> Reparaciones { get; set; } = null!;
            public ICollection<Reporte> Reportes { get; set; } = null!;
            public ICollection<Propiedad> PropiedadesComoAgente { get; set; } = null!;
        }

        public class Sesion
        {
            public int SesionID { get; set; }
            public int UsuarioID { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public string Token { get; set; } = null!;
            // Navegación
            public Usuario Usuario { get; set; } = null!;
        }

        public class Plataforma
        {
            public int PlataformaID { get; set; }
            public string NombrePlataforma { get; set; } = null!;
            // Navegación
            public ICollection<RedSocial> RedesSociales { get; set; } = null!;
        }

        public class RedSocial
        {
            public int RedID { get; set; }
            public int UsuarioID { get; set; }
            public int PlataformaID { get; set; }
            public string URL { get; set; } = null!;
            // Navegación
            public Usuario Usuario { get; set; } = null!;
            public Plataforma Plataforma { get; set; } = null!;
        }

        public class TipoOperacion
        {
            public int TipoOperacionID { get; set; }
            public string Descripcion { get; set; } = null!;
            // Navegación
            public ICollection<Propiedad> Propiedades { get; set; } = null!;
        }

        public class Constructora
        {
            public int ConstructoraID { get; set; }
            public string Nombre { get; set; } = null!;
            public string? Contacto { get; set; }
            public string? Telefono { get; set; }
            public string? Correo { get; set; }
            // Navegación
            public ICollection<Propiedad> Propiedades { get; set; } = null!;
            public ICollection<Proyecto> Proyectos { get; set; } = null!;
        }

        public class Propiedad
        {
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

        public class ImagenPropiedad
        {
            public int ImagenID { get; set; }
            public int PropiedadID { get; set; }
            public string URL { get; set; } = null!;
            // Navegación
            public Propiedad Propiedad { get; set; } = null!;
        }

        public class EstadoProyecto
        {
            public int EstadoProyectoID { get; set; }
            public string Descripcion { get; set; } = null!;
            // Navegación
            public ICollection<Proyecto> Proyectos { get; set; } = null!;
        }

        public class Proyecto
        {
            public int ProyectoID { get; set; }
            public string Nombre { get; set; } = null!;
            public int EstadoProyectoID { get; set; }
            public DateOnly? FechaInicio { get; set; }
            public DateOnly? FechaFin { get; set; }
            public int? ConstructoraID { get; set; }
            // Navegación
            public EstadoProyecto EstadoProyecto { get; set; } = null!;
            public Constructora? Constructora { get; set; }
            public ICollection<Estadistica> Estadisticas { get; set; } = null!;
        }

        public class EstadoAgenda
        {
            public int EstadoAgendaID { get; set; }
            public string Descripcion { get; set; } = null!;
        // Navegación
            public DbSet<Agenda> Agendas { get; set; } = null!;
         }

        public class Agenda
        {
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

        public class Favorito
        {
            public int FavoritoID { get; set; }
            public int UsuarioID { get; set; }
            public int PropiedadID { get; set; }
            public DateTime FechaAgregado { get; set; }
            // Navegación
            public Usuario Usuario { get; set; } = null!;
            public Propiedad Propiedad { get; set; } = null!;
        }

        public class Comentario
        {
            public int ComentarioID { get; set; }
            public int UsuarioID { get; set; }
            public int PropiedadID { get; set; }
            public string Contenido { get; set; } = null!;
            public DateTime FechaComentario { get; set; }
            // Navegación
            public Usuario Usuario { get; set; } = null!;
            public Propiedad Propiedad { get; set; } = null!;
        }

        public class HistorialPropiedad
        {
            public int HistorialID { get; set; }
            public int UsuarioID { get; set; }
            public int PropiedadID { get; set; }
            public DateTime FechaAcceso { get; set; }
            // Navegación
            public Usuario Usuario { get; set; } = null!;
            public Propiedad Propiedad { get; set; } = null!;
        }

        public class EstadoReparacion
        {
            public int EstadoReparacionID { get; set; }
            public string Descripcion { get; set; } = null!;
            // Navegación
            public ICollection<Reparacion> Reparaciones { get; set; } = null!;
        }

        public class Reparacion
        {
            public int ReparacionID { get; set; }
            public int UsuarioID { get; set; }
            public int? PropiedadID { get; set; }
            public string Descripcion { get; set; } = null!;
            public int EstadoReparacionID { get; set; }
            public DateTime FechaSolicitud { get; set; }
            // Navegación
            public Usuario Usuario { get; set; } = null!;
            public Propiedad? Propiedad { get; set; }
            public EstadoReparacion EstadoReparacion { get; set; } = null!;
        }

        public class Reporte
        {
            public int ReporteID { get; set; }
            public int UsuarioID { get; set; }
            public DateTime FechaReporte { get; set; }
            // Navegación
            public Usuario Usuario { get; set; } = null!;
            public ICollection<DetalleReporte> Detalles { get; set; } = null!;
        }

        public class DetalleReporte
        {
            public int DetalleID { get; set; }
            public int ReporteID { get; set; }
            public string Descripcion { get; set; } = null!; 
            // Navegación
            public Reporte Reporte { get; set; } = null!;
        }

        public class Estadistica
        {
            public int EstadisticaID { get; set; }
            public int? PropiedadID { get; set; }
            public int? ProyectoID { get; set; }
            public int Visitas { get; set; }
            public int Favoritos { get; set; }
            public int Comentarios { get; set; }
        // Navegación
        public Propiedad? Propiedad { get; set; }
            public Proyecto? Proyecto { get; set; }
        }
    }

