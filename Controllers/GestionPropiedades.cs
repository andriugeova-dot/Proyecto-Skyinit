// Controllers/GestionPropiedadesController.cs
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class GestionPropiedades : Controller
    {
        private readonly SkyinitContext _context;

        public GestionPropiedades(SkyinitContext context)
        {
            _context = context;
        }

        // GET: /GestionPropiedades/Index
        public async Task<IActionResult> Index(string? buscar)
        {
            var query = _context.Propiedades
                .Include(p => p.TipoOperacion)
                .Include(p => p.Agente)
                .Include(p => p.Imagenes)   // ← carga las imágenes relacionadas
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.ToLower();
                query = query.Where(p =>
                    p.Titulo.ToLower().Contains(buscar) ||
                    (p.Ciudad != null && p.Ciudad.ToLower().Contains(buscar)) ||
                    (p.Direccion.ToLower().Contains(buscar))
                );
            }

            var propiedades = await query.ToListAsync();

            ViewBag.Buscar = buscar;
            return View(propiedades);
        }
        public async Task<IActionResult> Editar(int id)
        {
            var propiedad = await _context.Propiedades
                .FirstOrDefaultAsync(p => p.PropiedadID == id);

            if (propiedad == null)
                return NotFound();

            return View(propiedad); // redirige a la vista Editar.cshtml con el modelo cargado
        }

        // POST: /GestionPropiedades/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var propiedad = await _context.Propiedades
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.PropiedadID == id);

            if (propiedad == null)
                return NotFound();

            _context.Propiedades.Remove(propiedad);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Propiedad eliminada correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }


    }
}
