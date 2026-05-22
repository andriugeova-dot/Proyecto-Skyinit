using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class GestionMantenimientosController : Controller
    {
        private readonly SkyinitContext _context;

        public GestionMantenimientosController(SkyinitContext context)
        {
            _context = context;
        }

        // ─── HELPER: sólo Administradores acceden ──────────────────────────────
        private async Task<bool> EsAdministradorAsync()
        {
            var idStr = User.FindFirst("UsuarioID")?.Value;
            if (!int.TryParse(idStr, out int id)) return false;

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioID == id);

            return usuario?.Rol?.NombreRol == "Administrador";
        }

        // ─── GET: /GestionMantenimientos/Index ─────────────────────────────────
        public async Task<IActionResult> Index(string? buscar, int? filtroEstado)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var query = _context.Reparaciones
                .Include(r => r.Usuario)
                .Include(r => r.Propiedad)
                .Include(r => r.EstadoReparacion)
                .AsQueryable();

            // RF149 – Filtrar por estado
            if (filtroEstado.HasValue)
                query = query.Where(r => r.EstadoReparacionID == filtroEstado.Value);

            // RF281 – Búsqueda libre (usuario o descripción)
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim().ToLower();
                query = query.Where(r =>
                    r.Descripcion.ToLower().Contains(buscar) ||
                    r.Usuario.Nombre.ToLower().Contains(buscar) ||
                    r.Usuario.Correo.ToLower().Contains(buscar));
            }

            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.Estados = await _context.EstadosReparacion.ToListAsync();
            ViewBag.Usuarios = await _context.Usuarios
                                        .OrderBy(u => u.Nombre).ToListAsync();
            ViewBag.Propiedades = await _context.Propiedades
                                        .OrderBy(p => p.Titulo).ToListAsync();

            return View(await query
                .OrderByDescending(r => r.FechaSolicitud)
                .ToListAsync());
        }

        // ─── POST: /GestionMantenimientos/Crear ────────────────────────────────
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            int UsuarioID,
            int? PropiedadID,
            string Descripcion,
            int EstadoReparacionID)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            // RF184 – Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                TempData["Error"] = "La descripción es obligatoria.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            // Verificar que el usuario exista
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.UsuarioID == UsuarioID);
            if (!usuarioExiste)
            {
                TempData["Error"] = "El usuario seleccionado no existe.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            // Verificar que la propiedad exista (si se proporcionó)
            if (PropiedadID.HasValue)
            {
                var propiedadExiste = await _context.Propiedades
                    .AnyAsync(p => p.PropiedadID == PropiedadID.Value);
                if (!propiedadExiste)
                    PropiedadID = null; // silenciosamente ignorar ID inválido
            }

            var reparacion = new Reparacion
            {
                UsuarioID = UsuarioID,
                PropiedadID = PropiedadID,
                Descripcion = Descripcion.Trim(),
                EstadoReparacionID = EstadoReparacionID,
                FechaSolicitud = DateTime.Now
            };

            _context.Reparaciones.Add(reparacion);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Solicitud de mantenimiento registrada correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // ─── POST: /GestionMantenimientos/Editar ───────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            string Descripcion,
            int EstadoReparacionID)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var reparacion = await _context.Reparaciones.FindAsync(id);
            if (reparacion == null)
                return NotFound();

            // RF184 – Validación
            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                TempData["Error"] = "La descripción no puede estar vacía.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            reparacion.Descripcion = Descripcion.Trim();
            reparacion.EstadoReparacionID = EstadoReparacionID;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Solicitud #{id} actualizada correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // ─── POST: /GestionMantenimientos/Eliminar/5 ───────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var reparacion = await _context.Reparaciones.FindAsync(id);
            if (reparacion == null)
                return NotFound();

            _context.Reparaciones.Remove(reparacion);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Solicitud #{id} eliminada correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // ─── POST: /GestionMantenimientos/CambiarEstado ────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, int estadoId)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var reparacion = await _context.Reparaciones.FindAsync(id);
            if (reparacion == null)
                return NotFound();

            reparacion.EstadoReparacionID = estadoId;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Estado de la solicitud #{id} actualizado.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}
