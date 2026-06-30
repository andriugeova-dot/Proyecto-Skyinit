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
        public async Task<IActionResult> Index(Propiedad propiedad, List<IFormFile> Imagenes)
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

                await ActualizarImagenes(propiedad.PropiedadID, Imagenes);

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

        // Reemplaza las imágenes de la propiedad por las nuevas subidas (si se subió alguna)
        private async Task ActualizarImagenes(int propiedadID, List<IFormFile> imagenes)
        {
            if (imagenes == null || !imagenes.Any(i => i.Length > 0))
                return; // No se subieron imágenes nuevas: se conservan las actuales

            var imagenesActuales = _context.ImagenesPropiedad
                .Where(i => i.PropiedadID == propiedadID)
                .ToList();

            // Elimina los archivos físicos antiguos
            foreach (var imagenActual in imagenesActuales)
            {
                var rutaFisica = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    imagenActual.URL.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }
            }

            _context.ImagenesPropiedad.RemoveRange(imagenesActuales);

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

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                _context.ImagenesPropiedad.Add(new ImagenPropiedad
                {
                    PropiedadID = propiedadID,
                    URL = $"/img/propiedades/{fileName}"
                });
            }

            await _context.SaveChangesAsync();
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