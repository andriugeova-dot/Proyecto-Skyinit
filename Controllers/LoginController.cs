using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;
using System.Security.Claims;

namespace Proyecto_SkyInit.Controllers
{
    public class LoginController : Controller
    {
        private readonly SkyinitContext _context;

        public LoginController(SkyinitContext context)
        {
            _context = context;
        }

        // GET /Login/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST /Login/Autenticar (login tradicional con correo/contraseña)
        [HttpPost]
        public async Task<IActionResult> Autenticar(string Correo, string Contraseña)
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Contraseña))
            {
                ViewBag.Error = "Ingresa tu correo y contraseña.";
                return View("Index");
            }

            var usuario = _context.Usuarios
                .Include(u => u.Rol)   // 👈 Cargar también el rol
                .FirstOrDefault(u => u.Correo == Correo && u.EstadoCuenta == "Activa");

            if (usuario == null || usuario.Rol == null ||
                !BCrypt.Net.BCrypt.Verify(Contraseña, usuario.ContrasenaHash))
            {
                ViewBag.Error = "Correo, contraseña o rol inválido.";
                return View("Index");
            }

            var claims = new List<Claim>
    {
        new(ClaimTypes.Name, usuario.Nombre),
        new(ClaimTypes.Email, usuario.Correo),
        new("UsuarioID", usuario.UsuarioID.ToString()),
        new("Rol", usuario.Rol.NombreRol)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });

            switch (usuario.Rol.NombreRol)
            {
                case "Administrador":
                    return RedirectToAction("Index", "Administrador");
                case "Agente":
                    return RedirectToAction("Index", "Agentes");
                case "Cliente":
                    return RedirectToAction("Index", "Menu");
                default:
                    return RedirectToAction("Index", "Menu");
            }
        }


        // ════════════════════════════════════════════════════════════
        // GOOGLE OAUTH — ⚠️ NO MODIFICAR — Integración Google API
        // ════════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
                Items = { { "prompt", "select_account" } }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                ViewBag.Error = "No se pudo autenticar con Google. Intenta de nuevo.";
                return View("Index");
            }

            var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            var nombre = result.Principal?.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Google no proporcionó un correo válido.";
                return View("Index");
            }

            var usuario = _context.Usuarios
                .Include(u => u.Rol)   // 👈 Cargar también el rol
                .FirstOrDefault(u => u.Correo == email && u.EstadoCuenta == "Activa");

            if (usuario == null || usuario.Rol == null)
            {
                ViewBag.Error = "Tu correo no está registrado o no tiene rol asignado.";
                return View("Index");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.Correo),
                new("UsuarioID", usuario.UsuarioID.ToString()),
            new("Rol", usuario.Rol.NombreRol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });

            // 🚀 Redirigir según rol
            switch (usuario.Rol.NombreRol)
            {
                case "Administrador":
                    return RedirectToAction("Index", "Administrador");
                case "Agente":
                    return RedirectToAction("Index", "Agentes");
                case "Cliente":
                    return RedirectToAction("Index", "Menu");
                default:
                    return RedirectToAction("Index", "Menu");
            }
        }



        // ════════════════════════════════════════════════════════════
        // FIN bloque Google API
        // ════════════════════════════════════════════════════════════

        [HttpGet]
        public IActionResult Ajustes()
        {
            return RedirectToAction("Index", "PanelUsuario");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
