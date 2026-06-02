using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class GestionProyectosController : Controller
    {
        private readonly SkyinitContext _context;

        public GestionProyectosController(SkyinitContext context)
        {
            _context = context;
        }

        // GET: Listado de proyectos con búsqueda
        public IActionResult Index(string? buscar)
        {
            var proyectos = _context.Proyectos
                .Select(p => new Proyecto
                {
                    ProyectoID = p.ProyectoID,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    EstadoProyectoID = p.EstadoProyectoID,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    ConstructoraID = p.ConstructoraID,
                    EstadoProyecto = p.EstadoProyecto,
                    Constructora = p.Constructora,
                    Estadisticas = p.Estadisticas,
                    ImagenesProyecto = p.ImagenesProyecto
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                proyectos = proyectos
                    .Where(p =>
                        p.Nombre.Contains(buscar) ||
                        (p.Constructora != null && p.Constructora.Nombre.Contains(buscar)))
                    .ToList();
            }

            ViewBag.Buscar = buscar;
            return View(proyectos);
        }
        // GET: Editar proyecto
        public IActionResult Editar(int id)
        {
            var proyecto = _context.Proyectos.Find(id);
            if (proyecto == null) return NotFound();

            PoblarSelects();
            return View(proyecto);
        }

        // POST: Editar proyecto
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
            return RedirectToAction("Index");
        }

        // POST: Eliminar proyecto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            var proyecto = _context.Proyectos.Find(id);
            if (proyecto == null) return NotFound();

            _context.Proyectos.Remove(proyecto);
            _context.SaveChanges();

            TempData["Mensaje"] = "✅ Proyecto eliminado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("Index");
        }

        // Método auxiliar para poblar selects
        private void PoblarSelects()
        {
            ViewBag.EstadosProyecto = new SelectList(_context.EstadosProyecto.ToList(), "EstadoProyectoID", "Descripcion");
            ViewBag.Constructoras = new SelectList(_context.Constructoras.ToList(), "ConstructoraID", "Nombre");
        }
    }
}
