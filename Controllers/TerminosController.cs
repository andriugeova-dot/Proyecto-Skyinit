using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Proyecto_SkyInit.Controllers
{
    public class TerminosController : Controller
    {
        private readonly SkyinitContext _context;

        public TerminosController(SkyinitContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarRegistro()
        {
            if (TempData["NuevoUsuarioID"] is int usuarioId)
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.UsuarioID == usuarioId);

                if (usuario != null && usuario.Rol != null)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name,  usuario.Nombre),
                        new(ClaimTypes.Email, usuario.Correo),
                        new("UsuarioID",      usuario.UsuarioID.ToString()),
                        new("Rol",            usuario.Rol.NombreRol)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties { IsPersistent = false });
                }
            }

            return RedirectToAction("Index", "Menu");
        }
    }
}
