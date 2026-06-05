using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class FavoritosController : Controller
    {
        private readonly SkyinitContext _context;

        public FavoritosController(SkyinitContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Toggle(int propiedadId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["MostarModalRegistro"] = true;
                return Unauthorized();
            }

            var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);

            var FavoritoExistente = _context.Favoritos
                .FirstOrDefault(f => f.UsuarioID == usuarioId && f.PropiedadID ==   propiedadId);

            if (FavoritoExistente != null)
            {
                _context.Favoritos.Remove(FavoritoExistente);
                
            } else
            {
                var favorito = new Favorito
                {
                    UsuarioID = usuarioId,
                    PropiedadID = propiedadId
                };
                _context.Favoritos.Add(favorito);
            }

            _context.SaveChanges();

            return Ok();
        }
    }
}
