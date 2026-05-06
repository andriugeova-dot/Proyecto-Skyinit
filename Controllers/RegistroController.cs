using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Models;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class RegistroController : Controller
    {
        private readonly SkyinitContext _context;
        public RegistroController(SkyinitContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrar(string Nombre, string Correo, string Telefono, string Contraseña, string ConfirmarContraseña)
        {
            ViewBag.Error = null;
            if (Contraseña != ConfirmarContraseña)
            {
                ViewBag.Error = "La contraseña no coincide";
                return View("Index");
            }

            if (Contraseña.Length < 8 ||
                !Contraseña.Any(char.IsUpper) ||
                !Contraseña.Any(char.IsDigit) ||
                !Contraseña.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                ViewBag.Error = "La contraseña debe tener al menos 8 caracteres, una mayúscula, un número y un símbolo";
                return View("Index");
            }
            var existeCorreo = _context.Usuarios.Any(u => u.Correo == Correo);
            if (existeCorreo)
            {
                ViewBag.Error = "El correo ya está registrado. Use otro diferente";
                return View("Index");
            }

            var rolUsuario = _context.Roles.FirstOrDefault(r => r.NombreRol == "Usuario");
                if (rolUsuario == null)
                {
                    ViewBag.Error = "Error de configuración: rol no encontrado.";
                    return View("Index");
                }
            var usuario = new Usuario
            {
                Nombre = Nombre,
                Correo = Correo,
                Telefono = Telefono,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(Contraseña),
                FechaRegistro = DateTime.Now,
                EstadoCuenta = "Activa",
                RolID = rolUsuario.RolID

            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Terminos");
        }
    }
}