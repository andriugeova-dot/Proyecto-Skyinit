using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class PublicarProyectoController : Controller
    {
        private readonly SkyinitContext _context;

        public PublicarProyectoController(SkyinitContext context)
        {
            _context = context;
        }

        // GET: PublicarProyecto
        public IActionResult Index()
        {
            ViewBag.EstadosProyecto = new SelectList(_context.EstadosProyecto.ToList(), "EstadoProyectoID", "Descripcion");
            ViewBag.Constructoras = new SelectList(_context.Constructoras.ToList(), "ConstructoraID", "Nombre");
            return View();
        }

        // POST: PublicarProyecto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Proyecto modelo, List<IFormFile> Imagenes)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Mensaje"] = "❌ Error de validación: " + string.Join(", ", errores);
                TempData["TipoMensaje"] = "error";

                ViewBag.EstadosProyecto = new SelectList(_context.EstadosProyecto.ToList(), "EstadoProyectoID", "Descripcion");
                ViewBag.Constructoras = new SelectList(_context.Constructoras.ToList(), "ConstructoraID", "Nombre");

                return View(modelo);
            }

            _context.Proyectos.Add(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "✅ Proyecto publicado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("Index", "GestionProyectos");
        }
    }
}
