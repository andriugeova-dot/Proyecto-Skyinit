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
            var propiedades = _context.Propiedades
                .Include(p => p.Imagenes)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
                propiedades = propiedades.Where(p => p.Titulo.Contains(query) || p.Descripcion.Contains(query));

            if (tipoOperacion.HasValue)
                propiedades = propiedades.Where(p => p.TipoOperacionID == tipoOperacion);

            if (habitaciones.HasValue)
                propiedades = propiedades.Where(p => p.Habitaciones >= habitaciones);

            if (!string.IsNullOrEmpty(ciudad))
                propiedades = propiedades.Where(p => p.Ciudad.Contains(ciudad));

            return View("Index", propiedades.ToList());
        }

        public IActionResult Index()
        {
            var propiedades = _context.Propiedades
        .Include(p => p.Imagenes)
        .ToList();

            return View(propiedades);
        }
    }
}
