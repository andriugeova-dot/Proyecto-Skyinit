using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuariosMantenimientosController : Controller
    {
        private readonly SkyinitContext _context;

        public UsuariosMantenimientosController(SkyinitContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            CargarFotoPerfil();
            return View();
        }

        private void CargarFotoPerfil()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);
                var foto = _context.Usuarios
                    .Where(u => u.UsuarioID == usuarioId)
                    .Select(u => u.FotoPerfil)
                    .FirstOrDefault();
                ViewBag.FotoPerfil = foto ?? "/img/default-avatar.svg";
            }
            else
            {
                ViewBag.FotoPerfil = "/img/default-avatar.svg";
            }
        }
    }
}
