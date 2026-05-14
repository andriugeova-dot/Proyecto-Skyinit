using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class PublicarPropiedadController : Controller
    {
        private readonly SkyinitContext _context;

        public PublicarPropiedadController(SkyinitContext context)
        {
            _context = context;
        }

        // Método privado para no repetir los ViewBag en cada acción
        private void CargarViewBags()
        {
            ViewBag.TiposOperacion = new SelectList(_context.TiposOperacion.ToList(), "TipoOperacionID", "Descripcion");
            ViewBag.Agentes = new SelectList(_context.Usuarios.Where(u => u.RolID == 2).ToList(), "UsuarioID", "Nombre");
            ViewBag.constructoras = new SelectList(_context.Constructoras.ToList(), "ConstructoraID", "Nombre");
        }

        // PublicarPropiedad
        public IActionResult Index()
        {
            CargarViewBags();
            return View();
        }

        // Publicar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Propiedad modelo, List<IFormFile> Imagenes)
        {
            if (!ModelState.IsValid)
            {
                CargarViewBags();
                TempData["Mensaje"] = "❌ Error de validación en el formulario";
                return View(modelo);
            }

            _context.Propiedades.Add(modelo);
            await _context.SaveChangesAsync();

            await GuardarImagenes(modelo.PropiedadID, Imagenes);

            TempData["Mensaje"] = "✅ Propiedad publicada correctamente";
            return RedirectToAction("Index","GestionPropiedades");
        }

        //Cancelar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar()
        {
            return RedirectToAction("Index", "Administrador");
        }

        // Método privado para guardar imágenes 
        private async Task GuardarImagenes(int propiedadID, List<IFormFile> imagenes)
        {
            if (imagenes == null || imagenes.Count == 0) return;

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/propiedades");
            Directory.CreateDirectory(uploadPath);

            foreach (var img in imagenes)
            {
                if (img.Length == 0) continue;

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                _context.ImagenesPropiedad.Add(new ImagenPropiedad
                {
                    PropiedadID = propiedadID,
                    URL = "/img/propiedades/" + fileName
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}