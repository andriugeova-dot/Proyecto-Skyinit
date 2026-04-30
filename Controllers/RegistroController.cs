using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Crypto.Generators;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class RegistroController : Controller
    {
        private readonly SkyInitContext _context;
        public RegistroController(SkyInitContext context)
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
            var usuario = new Usuario
            {
                Nombre = Nombre,
                Correo = Correo,
                Telefono = Telefono,
                ContraseñaHash = BCrypt.Net.BCrypt.HashPassword(Contraseña),
                FechaRegistro = DateTime.Now,
                EstadoCuenta = "Activa",
                RolID = 3

            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Terminos");
        }
    }
}