using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuarioProyectosController : Controller
    {
        private readonly SkyinitContext _context;

        public UsuarioProyectosController(SkyinitContext context)
        {
            _context = context;
        }
      
        public IActionResult Index()
        {
            ViewData["Title"] = "SkyInit";
            CargarFotoPerfil();
            var proyectos = _context.Proyectos
                .Include(p => p.Constructora)
                .Include(p => p.EstadoProyecto)
                .Include(p => p.ImagenesProyecto)
           .ToList();

            return View(proyectos);
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

        //REDIRECCIÓN A MENU//
        public IActionResult Inicio()
        {
            return RedirectToAction("Inicio","Menu");
        }
    }
}
