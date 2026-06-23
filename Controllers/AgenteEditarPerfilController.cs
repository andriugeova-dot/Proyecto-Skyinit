using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using System.Security.Claims;
using Proyecto_SkyInit.Models;
using BCrypt.Net;

namespace Proyecto_SkyInit.Controllers
{
    public class AgenteEditarPerfilController : Controller
    {
        private readonly SkyinitContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AgenteEditarPerfilController(SkyinitContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool EsAgente()
        {
            var rol = User.FindFirstValue("Rol");
            return User.Identity != null
                   && User.Identity.IsAuthenticated
                   && rol == "Agente";
        }

        private int ObtenerAgenteID()
        {
            var idStr = User.FindFirstValue("UsuarioID");
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        // ── E-01 CORREGIDO: renombrado de AgenteEditarPerfil() → Index()
        // ── E-02 CORREGIDO: agregado [HttpGet] explícito
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();
            var agente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UsuarioID == agenteID);

            if (agente == null)
                return NotFound();

            return View(agente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPerfil(string Nombre, string? Telefono)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                TempData["Mensaje"] = "El nombre no puede estar vacío.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index)); // ← ahora apunta a Index
            }

            var agenteID = ObtenerAgenteID();
            var agente = await _context.Usuarios.FindAsync(agenteID);

            if (agente == null)
                return NotFound();

            agente.Nombre = Nombre.Trim();
            agente.Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim();

            try
            {
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Perfil actualizado correctamente.";
                TempData["TipoMensaje"] = "success";
            }
            catch (DbUpdateException)
            {
                TempData["Mensaje"] = "Error al guardar los cambios.";
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarFotoPerfil(IFormFile? fotoPerfil)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();
            var agente = await _context.Usuarios.FindAsync(agenteID);

            if (agente == null)
                return NotFound();

            if (fotoPerfil != null && fotoPerfil.Length > 0)
            {
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(fotoPerfil.FileName).ToLower();

                if (!extensionesPermitidas.Contains(extension))
                {
                    TempData["Mensaje"] = "Solo se aceptan imágenes en formato JPG, PNG o WebP.";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction(nameof(Index));
                }

                if (fotoPerfil.Length > 5 * 1024 * 1024)
                {
                    TempData["Mensaje"] = "La imagen no debe superar 5 MB.";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction(nameof(Index));
                }

                try
                {
                    if (!string.IsNullOrEmpty(agente.FotoPerfil))
                    {
                        var rutaAnterior = Path.Combine(_webHostEnvironment.WebRootPath, agente.FotoPerfil.TrimStart('/'));
                        if (System.IO.File.Exists(rutaAnterior))
                            System.IO.File.Delete(rutaAnterior);
                    }

                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaDirectorio = Path.Combine(_webHostEnvironment.WebRootPath, "img", "Avatars");

                    if (!Directory.Exists(rutaDirectorio))
                        Directory.CreateDirectory(rutaDirectorio);

                    var rutaCompleta = Path.Combine(rutaDirectorio, nombreArchivo);

                    using (var stream = System.IO.File.Create(rutaCompleta))
                    {
                        await fotoPerfil.CopyToAsync(stream);
                    }

                    agente.FotoPerfil = $"/img/Avatars/{nombreArchivo}";
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "Foto de perfil actualizada correctamente.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = $"Error al subir la imagen: {ex.Message}";
                    TempData["TipoMensaje"] = "error";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(string ContrasenaActual, string ContrasenaNueva, string ConfirmarContrasena)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(ContrasenaActual) ||
                string.IsNullOrWhiteSpace(ContrasenaNueva) ||
                string.IsNullOrWhiteSpace(ConfirmarContrasena))
            {
                TempData["Mensaje"] = "Todos los campos de contraseña son obligatorios.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            if (ContrasenaNueva != ConfirmarContrasena)
            {
                TempData["Mensaje"] = "Las contraseñas nuevas no coinciden.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            if (ContrasenaNueva.Length < 6)
            {
                TempData["Mensaje"] = "La contraseña debe tener al menos 6 caracteres.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var agenteID = ObtenerAgenteID();
            var agente = await _context.Usuarios.FindAsync(agenteID);

            if (agente == null)
                return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(ContrasenaActual, agente.ContrasenaHash))
            {
                TempData["Mensaje"] = "La contraseña actual es incorrecta.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                agente.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(ContrasenaNueva);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Contraseña actualizada correctamente.";
                TempData["TipoMensaje"] = "success";
            }
            catch (DbUpdateException)
            {
                TempData["Mensaje"] = "Error al guardar la contraseña.";
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarFotoPerfil()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();
            var agente = await _context.Usuarios.FindAsync(agenteID);

            if (agente == null)
                return NotFound();

            if (!string.IsNullOrEmpty(agente.FotoPerfil))
            {
                try
                {
                    var rutaAnterior = Path.Combine(_webHostEnvironment.WebRootPath, agente.FotoPerfil.TrimStart('/'));
                    if (System.IO.File.Exists(rutaAnterior))
                        System.IO.File.Delete(rutaAnterior);

                    agente.FotoPerfil = null;
                    await _context.SaveChangesAsync();

                    TempData["Mensaje"] = "Foto de perfil eliminada.";
                    TempData["TipoMensaje"] = "success";
                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = $"Error al eliminar la foto: {ex.Message}";
                    TempData["TipoMensaje"] = "error";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}