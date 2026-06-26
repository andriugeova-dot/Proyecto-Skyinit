using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuariosSolicitudServiciosController : Controller
    {
        private readonly SkyinitContext _context;

        public UsuariosSolicitudServiciosController(SkyinitContext context)
        {
            _context = context;
        }

        // GET: /UsuariosSolicitudServicios/Index
        public async Task<IActionResult> Index()
        {
            var idStr = User.FindFirst("UsuarioID")?.Value;
            if (!int.TryParse(idStr, out int usuarioId))
                return RedirectToAction("Index", "Login");

            var solicitudes = await _context.Reparaciones
                .Include(r => r.EstadoReparacion)
                .Where(r => r.UsuarioID == usuarioId)
                .OrderByDescending(r => r.FechaSolicitud)
                .ToListAsync();

            return View(solicitudes);
        }
    }
}
