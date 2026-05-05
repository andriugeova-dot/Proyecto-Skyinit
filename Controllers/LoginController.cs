using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class LoginController : Controller
    {
        private readonly SkyInitContext _context;

        public LoginController(SkyInitContext context)
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

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(Contraseña, usuario.ContraseñaHash))
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View("Index");
            }

            // Aquí puedes guardar sesión en HttpContext.Session
            return RedirectToAction("Index", "Home");
        }

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
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Index");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index");

            // Buscar usuario en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == email && u.EstadoCuenta == "Activa");

            if (usuario == null)
            {
                // Si no existe, no se crea automáticamente
                ViewBag.Error = "Tu correo no está registrado en el sistema. Debes registrarte primero.";
                return View("Index");
            }

            // Si existe, se permite el acceso
            return RedirectToAction("Index", "Home");
        }


        // GET /Login/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
