using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class EditarProyectoController : Controller
    {
        private readonly SkyinitContext _context;

        public EditarProyectoController(SkyinitContext context)
        {
            _context = context;
        }

        // GET: EditarProyecto
        public IActionResult Editar(int id)
        {
            var proyecto = _context.Proyectos
                .Include(p => p.EstadoProyecto)
                .Include(p => p.Constructora)
                .FirstOrDefault(p => p.ProyectoID == id);

            if (proyecto == null) return NotFound();

            PoblarSelects();
            return View("Index", proyecto);
        }

        // POST: EditarProyecto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Proyecto modelo)
        {
            if (!ModelState.IsValid)
            {
                PoblarSelects();
                TempData["Mensaje"] = "❌ Error de validación";
                TempData["TipoMensaje"] = "error";
                return View(modelo);
            }

            _context.Proyectos.Update(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "✅ Proyecto actualizado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("Index", "GestionProyectos");
        }

        private void PoblarSelects()
        {
            ViewBag.EstadosProyecto = new SelectList(_context.EstadosProyecto.ToList(), "EstadoProyectoID", "Descripcion");
            ViewBag.Constructoras = new SelectList(_context.Constructoras.ToList(), "ConstructoraID", "Nombre");
        }
    }
}
