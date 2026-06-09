// Controllers/AgentePropiedadesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using System.Security.Claims;

namespace Proyecto_SkyInit.Controllers
{
    public class AgentePropiedadesController : Controller
    {
        private readonly SkyinitContext _context;

        public AgentePropiedadesController(SkyinitContext context)
        {
            _context = context;
        }

        // ── HELPER: verifica que el usuario sea Agente (RolID == 2) ──────────
        private bool EsAgente()
        {
            var rol = User.FindFirst("Rol")?.Value;
            return rol == "Agente";
        }

        // ── HELPER: obtiene el UsuarioID del claim ───────────────────────────
        private int? ObtenerAgenteId()
        {
            var idStr = User.FindFirst("UsuarioID")?.Value;
            return int.TryParse(idStr, out int id) ? id : null;
        }

        // ── GET: /AgentePropiedades/Index ────────────────────────────────────
        // RF201 – Mostrar listado de propiedades publicadas por el agente
        // RF202 – Editar propiedades
        // RF204 – Eliminar propiedades
        public async Task<IActionResult> Index(string? buscar)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteId = ObtenerAgenteId();
            if (agenteId == null)
                return RedirectToAction("Index", "Login");

            var query = _context.Propiedades
                .Include(p => p.TipoOperacion)
                .Include(p => p.Agente)
                .Include(p => p.Imagenes)
                .Where(p => p.AgenteID == agenteId)   // ← solo las del agente en sesión
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.ToLower();
                query = query.Where(p =>
                    p.Titulo.ToLower().Contains(buscar) ||
                    (p.Ciudad != null && p.Ciudad.ToLower().Contains(buscar)) ||
                    p.Direccion.ToLower().Contains(buscar)
                );
            }

            var propiedades = await query.ToListAsync();
            ViewBag.Buscar = buscar;
            return View(propiedades);
        }

        // ── POST: /AgentePropiedades/Eliminar ────────────────────────────────
        // RF204 – Permitir eliminar propiedades del agente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteId = ObtenerAgenteId();

            // Solo permite eliminar si la propiedad pertenece a este agente
            var propiedad = await _context.Propiedades
                .Include(p => p.Imagenes)
                .FirstOrDefaultAsync(p => p.PropiedadID == id && p.AgenteID == agenteId);

            if (propiedad == null)
            {
                TempData["Mensaje"] = "Propiedad no encontrada o sin permisos.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            // Eliminar archivos de imagen del servidor
            foreach (var img in propiedad.Imagenes)
            {
                var rutaFisica = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", img.URL.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(rutaFisica))
                    System.IO.File.Delete(rutaFisica);
            }

            _context.Propiedades.Remove(propiedad);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"La propiedad \"{propiedad.Titulo}\" fue eliminada correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}
