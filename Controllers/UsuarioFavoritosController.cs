using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuarioFavoritosController : Controller
    {
        private readonly SkyinitContext _context;
        public UsuarioFavoritosController(SkyinitContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var usuarioID = int.Parse(User.FindFirst("UsuarioID")!.Value);
            var favoritos = _context.Favoritos
                .Include(f => f.Propiedad)
                .ThenInclude(p => p.Imagenes)
                .Where(f => f.UsuarioID == usuarioID)
                .ToList();
            return View(favoritos);
        }
        public IActionResult Quitar(int propiedadID)
        {
            var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);
            var favorito = _context.Favoritos
                .FirstOrDefault(f => f.UsuarioID == usuarioId && f.PropiedadID == propiedadID);

            if (favorito != null)
            {
                _context.Favoritos.Remove(favorito);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
