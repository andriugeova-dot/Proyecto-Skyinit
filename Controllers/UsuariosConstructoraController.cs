using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Data;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuariosConstructoraController : Controller
    {
        private readonly SkyinitContext _context;
        public UsuariosConstructoraController(SkyinitContext context)
        {
            _context = context;
        }
        public IActionResult Index(string buscar, string filtroEstado, string ciudad, DateTime? desde)
        {
            var query = _context.Constructoras.AsQueryable();
            query = query.Where(c => c.Estado == "Activo");

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(c => c.Nombre.Contains(buscar) || c.Contacto.Contains(buscar));

            if (!string.IsNullOrEmpty(filtroEstado))
                query = query.Where(c => c.Estado == filtroEstado);

            if (!string.IsNullOrEmpty(ciudad))
                query = query.Where(c => c.Ciudad == ciudad);


            var model = query.Select(c => new ConstructoraViewModel
            {
                Id = c.ConstructoraID,
                Nombre = c.Nombre,
                Contacto = c.Contacto,
                Telefono = c.Telefono,
                Estado = c.Estado,
                Ciudad = c.Ciudad,
                Descripcion = c.Descripcion,
                Logo = c.Imagenes.FirstOrDefault(i => i.EsLogo).URL
            }).ToList();

            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.Ciudad = ciudad;
            ViewBag.Desde = desde;

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerDetalles(int id)
        {
            var constructora = await _context.Constructoras
                .Include(c => c.Proyectos)
                .Include(c => c.Propiedades)
                    .ThenInclude(p => p.Imagenes)
                .Include(c => c.Imagenes) // 👈 Asegúrate de incluir las imágenes
                .FirstOrDefaultAsync(c => c.ConstructoraID == id);

            if (constructora == null)
            {
                return NotFound();
            }

            return Json(new
            {
                nombre = constructora.Nombre ?? "Sin nombre",
                sede = constructora.Ciudad ?? "Sede Principal",

                // ✅ Aquí tomamos el logo desde la relación ImagenesConstructoras
                logoUrl = constructora.Imagenes.FirstOrDefault(i => i.EsLogo) != null
                         ? "/images/" + constructora.Imagenes.FirstOrDefault(i => i.EsLogo).URL
                         : "/img/fenix-default.png",

                proyectos = constructora.Proyectos.Select(p => new {
                    nombre = p.Nombre ?? "Proyecto sin nombre",
                    ubicacion = p.Ubicacion ?? "Ubicación no especificada"
                }),
                propiedades = constructora.Propiedades.Select(p => new {
                    nombre = p.Titulo ?? "Sin título",
                    precio = p.Precio,
                    imagenUrl = (p.Imagenes != null && p.Imagenes.FirstOrDefault() != null)
                        ? p.Imagenes.FirstOrDefault().URL
                        : "/img/casa-default.jpg"
                })
            });

        }

    }

}
