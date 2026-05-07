// Controllers/LoginController.cs
// CORRECCIÓN E08: Todas las referencias a RedirectToAction("Index", "Home")
//                 cambiadas a RedirectToAction("Index", "Menu").
// La integración con Google API permanece INTACTA y sin modificar.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Models;
using Proyecto_SkyInit.Data;

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
        public IActionResult Autenticar(string Correo, string Contraseña)
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Contraseña))
            {
                ViewBag.Error = "Ingresa tu correo y contraseña.";
                return View("Index");
            }

            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Correo == Correo && u.EstadoCuenta == "Activa");

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(Contraseña, usuario.ContrasenaHash))
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View("Index");
            }

            // E08 CORREGIDO: redirige a Menu/Index en lugar de Home/Index
            return RedirectToAction("Index", "Menu");
        }

        // ════════════════════════════════════════════════════════════
        // GOOGLE OAUTH — ⚠️ NO MODIFICAR — Integración Google API
        // ════════════════════════════════════════════════════════════

        // GET /Login/LoginWithGoogle
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
            // ✅ CORRECTO: leer del scheme externo de Google, no de la cookie
            var result = await HttpContext.AuthenticateAsync(
                GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                ViewBag.Error = "No se pudo autenticar con Google. Intenta de nuevo.";
                return View("Index");
            }

            // Extraer el correo desde los claims de Google
            var email = result.Principal?
                .FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            var nombre = result.Principal?
                .FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Google no proporcionó un correo válido.";
                return View("Index");
            }

            // Buscar usuario en la base de datos
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Correo == email && u.EstadoCuenta == "Activa");

            if (usuario == null)
            {
                ViewBag.Error = "Tu correo no está registrado. Regístrate primero.";
                return View("Index");
            }

            // ✅ CORRECTO: crear la sesión en la cookie de la app
            var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.Name,  usuario.Nombre),
        new(System.Security.Claims.ClaimTypes.Email, usuario.Correo),
        new("UsuarioID", usuario.UsuarioID.ToString())
    };

            var identity = new System.Security.Claims.ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });

            return RedirectToAction("Index", "Menu");
        }

        // ════════════════════════════════════════════════════════════
        // FIN bloque Google API
        // ════════════════════════════════════════════════════════════

        // GET /Login/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
