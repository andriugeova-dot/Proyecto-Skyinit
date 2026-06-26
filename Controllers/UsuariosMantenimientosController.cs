using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuariosMantenimientosController : Controller
    {
        private readonly SkyinitContext _context;

        public UsuariosMantenimientosController(SkyinitContext context)
        {
            _context = context;
        }

        // ─── GET: /UsuariosMantenimientos/Index ────────────────────────────────
        public async Task<IActionResult> Index()
        {
            CargarFotoPerfil();

            var servicios = await _context.ServiciosMantenimiento
                .Where(s => s.Estado == "Activo")
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return View(servicios);
        }

        // ─── POST: /UsuariosMantenimientos/SolicitarServicio ───────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarServicio(int servicioId, string? notas)
        {
            // Solo usuarios autenticados pueden solicitar un servicio
            if (User.Identity is null || !User.Identity.IsAuthenticated)
            {
                TempData["Error"] = "Debes iniciar sesión para solicitar un servicio.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index", "Login");
            }

            var servicio = await _context.ServiciosMantenimiento
                .FirstOrDefaultAsync(s => s.ServicioID == servicioId && s.Estado == "Activo");

            if (servicio == null)
            {
                TempData["Error"] = "El servicio seleccionado no existe o ya no está disponible.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);

            // Estado inicial: "Pendiente" (se busca por descripción para no depender de IDs fijos)
            var estadoPendiente = await _context.EstadosReparacion
                .FirstOrDefaultAsync(e => e.Descripcion == "Pendiente");
            var estadoId = estadoPendiente?.EstadoReparacionID ?? 1;

            var descripcion = $"Solicitud de servicio: {servicio.Nombre}";
            if (!string.IsNullOrWhiteSpace(notas))
                descripcion += $" — Notas del usuario: {notas.Trim()}";

            var reparacion = new Reparacion
            {
                UsuarioID = usuarioId,
                PropiedadID = null,
                Descripcion = descripcion,
                EstadoReparacionID = estadoId,
                FechaSolicitud = DateTime.Now
            };

            _context.Reparaciones.Add(reparacion);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Tu solicitud de \"{servicio.Nombre}\" fue enviada correctamente. Un administrador la revisará pronto.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
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