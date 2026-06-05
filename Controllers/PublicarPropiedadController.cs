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

        // Cargar listas desplegables
        private void CargarViewBags()
        {
            ViewBag.TiposOperacion = new SelectList(
                _context.TiposOperacion.ToList(),
                "TipoOperacionID",
                "Descripcion"
            );

            ViewBag.Agentes = new SelectList(
                _context.Usuarios
                    .Where(u => u.RolID == 2)
                    .ToList(),
                "UsuarioID",
                "Nombre"
            );

            ViewBag.constructoras = new SelectList(
                _context.Constructoras.ToList(),
                "ConstructoraID",
                "Nombre"
            );
        }

        // GET: PublicarPropiedad
        public IActionResult Index()
        {
            CargarViewBags();
            return View();
        }

        // POST: PublicarPropiedad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            Propiedad modelo,
            List<IFormFile> Imagenes)
        {
            if (!ModelState.IsValid)
            {
                CargarViewBags();

                TempData["Mensaje"] = "Error de validación en el formulario.";
                TempData["TipoMensaje"] = "error";

                return View(modelo);
            }

            try
            {
                _context.Propiedades.Add(modelo);
                await _context.SaveChangesAsync();

                await GuardarImagenes(
                    modelo.PropiedadID,
                    Imagenes);

                TempData["Mensaje"] = "Propiedad publicada correctamente.";
                TempData["TipoMensaje"] = "success";

                return RedireccionarSegunRol();
            }
            catch (Exception ex)
            {
                CargarViewBags();

                TempData["Mensaje"] =
                    $"Error al publicar la propiedad: {ex.Message}";
                TempData["TipoMensaje"] = "error";

                return View(modelo);
            }
        }

        // POST: Cancelar
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
                    "Agentes");
            }

            return RedirectToAction(
                "Index",
                "Login");
        }

        // Guardar imágenes
        private async Task GuardarImagenes(
            int propiedadID,
            List<IFormFile> imagenes)
        {
            if (imagenes == null || !imagenes.Any())
                return;

            var uploadPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "img",
                "propiedades");

            Directory.CreateDirectory(uploadPath);

            foreach (var img in imagenes)
            {
                if (img.Length <= 0)
                    continue;

                var fileName =
                    $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";

                var filePath =
                    Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                _context.ImagenesPropiedad.Add(
                    new ImagenPropiedad
                    {
                        PropiedadID = propiedadID,
                        URL = $"/img/propiedades/{fileName}"
                    });
            }

            await _context.SaveChangesAsync();
        }
    }
}