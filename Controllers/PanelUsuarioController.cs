using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Data;
using Microsoft.EntityFrameworkCore;


namespace Proyecto_SkyInit.Controllers
{
    public class PanelUsuarioController : Controller
    {
        private readonly SkyinitContext _context;

        public PanelUsuarioController(SkyinitContext context)
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

        public IActionResult PanelPrincipal()
        {
            return RedirectToAction("Index", "PanelUsuario");
        }

        public IActionResult Perfil() { 
            return RedirectToAction("Index", "UsuarioPerfil");        
        }
    }
}
