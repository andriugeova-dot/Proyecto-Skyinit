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

        // GET /Login/Index  (ruta raíz según Program.cs)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST /Login/Autenticar
        [HttpPost]
        public IActionResult Autenticar(string Correo, string Contraseña)
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Contraseña))
            {
                ViewBag.Error = "Ingresa tu correo y contraseña.";
                return View("Index");
            }

            // Busca el usuario por correo en la tabla "usuarios" (SkyInitContext)
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Correo == Correo && u.EstadoCuenta == "Activa");

            if (usuario == null)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View("Index");
            }

            // Verifica la contraseña con BCrypt (mismo que usa RegistroController)
            bool passwordValida = BCrypt.Net.BCrypt.Verify(Contraseña, usuario.ContraseñaHash);

            if (!passwordValida)
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View("Index");
            }

            // Aquí puedes guardar sesión en HttpContext.Session o en la tabla Sesiones
            // Por ahora redirige al Home después del login exitoso
            return RedirectToAction("Index", "Home");
        }
    }
}