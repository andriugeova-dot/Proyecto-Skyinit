using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuarioPerfilController : Controller
    {
        private readonly SkyinitContext _context;

        public UsuarioPerfilController(SkyinitContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);

            var usuario = _context.Usuarios
                .Include(u => u.Favoritos)
                .ThenInclude(f => f.Propiedad)
                .FirstOrDefault(u => u.UsuarioID == usuarioId);

            return View(usuario);
        }

        public IActionResult Perfil()
        {
            var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);
            return View("Index", usuario);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil(Usuario model, IFormFile? Avatar)
        {
            var usuario = await _context.Usuarios.FindAsync(model.UsuarioID);

            if (usuario != null)
            {
                usuario.Nombre = model.Nombre;
                usuario.Correo = model.Correo;
                usuario.Telefono = model.Telefono;

                if (Avatar != null && Avatar.Length > 0)
                {
                    var uploadsDir = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot", "img", "Avatars");

                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    var fileName = Guid.NewGuid().ToString()
                                   + Path.GetExtension(Avatar.FileName);
                    var fullPath = Path.Combine(uploadsDir, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await Avatar.CopyToAsync(stream);
                    }

                    usuario.FotoPerfil = "/img/Avatars/" + fileName;
                }

                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cambios guardados correctamente.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarFoto(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.FotoPerfil = null;
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Avatar eliminado.";
            }
            return RedirectToAction("Index");
        }
    }
}