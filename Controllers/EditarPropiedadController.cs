using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class EditarPropiedadController : Controller
    {
        private readonly SkyinitContext _context;

        public EditarPropiedadController(SkyinitContext context)
        {
            _context = context;
        }

        // GET: EditarPropiedad/Index/5
        public async Task<IActionResult> Index(int id)
        {
            var propiedad = await _context.Propiedades
                .FirstOrDefaultAsync(p => p.PropiedadID == id);

            if (propiedad == null)
                return NotFound();

            PoblarCombos(propiedad);

            return View(propiedad);
        }

        // POST: EditarPropiedad/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Propiedad propiedad)
        {
            if (!ModelState.IsValid)
            {
                PoblarCombos(propiedad);
                return View(propiedad);
            }

            try
            {
                _context.Update(propiedad);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Propiedad modificada correctamente.";
                TempData["TipoMensaje"] = "success";

                return RedireccionarSegunRol();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Propiedades.Any(e => e.PropiedadID == propiedad.PropiedadID))
                    return NotFound();

                throw;
            }
        }

        // POST: EditarPropiedad/Cancelar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar()
        {
            return RedireccionarSegunRol();
        }

        // Redirección según rol
        private IActionResult RedireccionarSegunRol()
        {
            var rol = User.FindFirst("Rol")?.Value;

            if (string.Equals(
                rol,
                "Administrador",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(
                    "Index",
                    "GestionPropiedades");
            }

            if (string.Equals(
                rol,
                "Agente",
                StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(
                    "Index",
                    "AgentePropiedades");
            }

            return RedirectToAction(
                "Index",
                "Login");
        }

        // Carga de combos
        private void PoblarCombos(Propiedad propiedad)
        {
            ViewBag.TiposOperacion = new SelectList(
                _context.TiposOperacion,
                "TipoOperacionID",
                "Descripcion",
                propiedad.TipoOperacionID
            );

            ViewBag.Agentes = new SelectList(
                _context.Usuarios
                    .Where(u => u.RolID == 2)
                    .ToList(),
                "UsuarioID",
                "Nombre",
                propiedad.AgenteID
            );

            ViewBag.constructoras = new SelectList(
                _context.Constructoras,
                "ConstructoraID",
                "Nombre",
                propiedad.ConstructoraID
            );
        }
    }
}