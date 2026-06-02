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
        public async Task<IActionResult> Index(Proyecto modelo, IFormFile Imagenes)
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
            await GuardarImagenes(modelo.ProyectoID, Imagenes);

            TempData["Mensaje"] = "✅ Proyecto publicado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("Index", "GestionProyectos");
        }
        private async Task GuardarImagenes(int proyectoID, IFormFile imagen)
        {
            if (imagen == null || imagen.Length == 0) return;

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/proyectos");
            Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }
            _context.ImagenesProyecto.Add(new ImagenProyecto
            {
                ProyectoID = proyectoID,
                URL = "/img/proyectos/" + fileName
            });
            await _context.SaveChangesAsync();
        }
    }
}
