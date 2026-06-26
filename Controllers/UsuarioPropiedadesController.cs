using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuarioPropiedadesController : Controller
    {
        private readonly SkyinitContext _context;

        public UsuarioPropiedadesController(SkyinitContext context)
        {
            _context = context;
        }

        public IActionResult Buscar(string query, int? tipoOperacion, int? habitaciones, string ciudad)
        {
            CargarFotoPerfil();
            var propiedades = _context.Propiedades
                .Include(p => p.Imagenes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
                propiedades = propiedades.Where(p => p.Titulo.Contains(query) || p.Descripcion.Contains(query));

            if (tipoOperacion.HasValue)
                propiedades = propiedades.Where(p => p.TipoOperacionID == tipoOperacion);

            if (habitaciones.HasValue)
            {
                if (habitaciones.Value >= 3)
                    propiedades = propiedades.Where(p => p.Habitaciones >= 3);
                else
                    propiedades = propiedades.Where(p => p.Habitaciones == habitaciones.Value);
            }

            if (!string.IsNullOrEmpty(ciudad))
                propiedades = propiedades.Where(p => p.Ciudad.Contains(ciudad));

            return View("Index", propiedades.ToList());
        }

        public IActionResult Index()
        {
            CargarFotoPerfil();
            var propiedades = _context.Propiedades
                .Include(p => p.Imagenes)
                .ToList();

            if (User.Identity!.IsAuthenticated)
            {
                var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);
                var favoritos = _context.Favoritos
                    .Where(f => f.UsuarioID == usuarioId)
                    .Select(f => f.PropiedadID)
                    .ToList();

                foreach (var p in propiedades)
                {
                    p.EsFavorito = favoritos.Contains(p.PropiedadID);
                }
            }

            return View(propiedades);
        }

        private void CargarFotoPerfil()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);
                var foto = _context.Usuarios
                    .Where(u => u.UsuarioID == usuarioId)
                    .Select(u => u.FotoPerfil)
                    .FirstOrDefault();
                ViewBag.FotoPerfil = foto ?? "/img/default-avatar.svg";
            }
            else
            {
                ViewBag.FotoPerfil = "/img/default-avatar.svg";
            }
        }

        public async Task<IActionResult> ObtenerDetalles(int id)
        {
            var propiedad = await _context.Propiedades
                .Include(p => p.TipoOperacion)
                .Include(p => p.Agente)
                .Include(p => p.Constructora)
                    .ThenInclude(c => c.Imagenes)
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.PropiedadID == id);

            if (propiedad == null) return NotFound();

            var logoConstructora = propiedad.Constructora?.Imagenes?.FirstOrDefault(i => i.EsLogo)?.URL;

            return Json(new
            {
                titulo = propiedad.Titulo,
                descripcion = propiedad.Descripcion,
                precio = propiedad.Precio,
                direccion = propiedad.Direccion,
                ciudad = propiedad.Ciudad,
                habitaciones = propiedad.Habitaciones,
                tipoOperacion = propiedad.TipoOperacion?.Descripcion,
                imagenUrl = propiedad.Imagenes.FirstOrDefault()?.URL ?? "/img/default-property.jpg",

                agenteNombre = propiedad.Agente?.Nombre,
                agenteFoto = string.IsNullOrEmpty(propiedad.Agente?.FotoPerfil)
                    ? "/img/default-avatar.svg"
                    : propiedad.Agente!.FotoPerfil,

                constructoraNombre = propiedad.Constructora?.Nombre,
                constructoraLogo = string.IsNullOrEmpty(logoConstructora)
                    ? null
                    : "/images/" + logoConstructora
            });
        }
    }
}
