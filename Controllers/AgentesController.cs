using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;
using System.Security.Claims;

namespace Proyecto_SkyInit.Controllers
{
    public class AgentesController : Controller
    {
        private readonly SkyinitContext _context;

        public AgentesController(SkyinitContext context)
        {
            _context = context;
        }

        // ──────────────────────────────────────────────────────────────
        // HELPER: verificar que el usuario autenticado sea Agente
        // ──────────────────────────────────────────────────────────────
        private bool EsAgente()
        {
            var rol = User.FindFirstValue("Rol");
            return User.Identity != null
                   && User.Identity.IsAuthenticated
                   && rol == "Agente";
        }

        private int ObtenerAgenteID()
        {
            var idStr = User.FindFirstValue("UsuarioID");
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        // ──────────────────────────────────────────────────────────────
        // GET /Agentes/Index  →  Dashboard del agente  (RF217, RF201)
        // ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();

            // Datos reales de la BD
            var agente = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioID == agenteID);

            if (agente == null)
                return RedirectToAction("Index", "Login");

            var propiedades = await _context.Propiedades
                .Include(p => p.TipoOperacion)
                .Include(p => p.Imagenes)
                .Where(p => p.AgenteID == agenteID)
                .ToListAsync();

            var agendas = await _context.Agendas
                .Include(a => a.Propiedad)
                .Include(a => a.EstadoAgenda)
                .Where(a => a.Propiedad.AgenteID == agenteID)
                .OrderBy(a => a.FechaVisita)
                .Take(5)
                .ToListAsync();

            // Clientes únicos que tienen agenda con propiedades del agente
            var clientesIDs = await _context.Agendas
                .Where(a => a.Propiedad.AgenteID == agenteID)
                .Select(a => a.UsuarioID)
                .Distinct()
                .CountAsync();

            var consultasPendientes = await _context.Agendas
                .Where(a => a.Propiedad.AgenteID == agenteID
                         && a.EstadoAgenda.Descripcion == "Pendiente")
                .CountAsync();

            // ViewBag para la vista
            ViewBag.NombreAgente = agente.Nombre;
            ViewBag.TotalPropiedades = propiedades.Count;
            ViewBag.TotalClientes = clientesIDs;
            ViewBag.CitasHoy = agendas
                .Count(a => a.FechaVisita.Date == DateTime.Today);
            ViewBag.ConsultasPendientes = consultasPendientes;
            ViewBag.Propiedades = propiedades;
            ViewBag.ProximasCitas = agendas;

            return View();
        }

        // ──────────────────────────────────────────────────────────────
        // POST /Agentes/EliminarPropiedad  (RF204)
        // ──────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPropiedad(int id)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();

            var propiedad = await _context.Propiedades
                .FirstOrDefaultAsync(p => p.PropiedadID == id && p.AgenteID == agenteID);

            if (propiedad == null)
            {
                TempData["Mensaje"] = "Propiedad no encontrada o no autorizada.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(Index));
            }

            _context.Propiedades.Remove(propiedad);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Propiedad eliminada correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // ──────────────────────────────────────────────────────────────
        // GET /Agentes/EditarPropiedad/5  (RF202)
        // ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> EditarPropiedad(int id)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();

            var propiedad = await _context.Propiedades
                .FirstOrDefaultAsync(p => p.PropiedadID == id && p.AgenteID == agenteID);

            if (propiedad == null)
                return NotFound();

            // Redirige al módulo de edición existente pasando el ID
            return RedirectToAction("Index", "EditarPropiedad", new { id });
        }

        // ──────────────────────────────────────────────────────────────
        // GET /Agentes/PublicarPropiedad  →  Acción rápida (RF183)
        // ──────────────────────────────────────────────────────────────
        public IActionResult PublicarPropiedad()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            return RedirectToAction("Index", "PublicarPropiedad");
        }

        // ──────────────────────────────────────────────────────────────
        // GET /Agentes/MiPerfil  (RF213, RF214)
        // ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> MiPerfil()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();
            var agente = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioID == agenteID);

            if (agente == null)
                return NotFound();

            return View(agente);
        }

        // ──────────────────────────────────────────────────────────────
        // POST /Agentes/ActualizarPerfil  (RF214)
        // ──────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPerfil(string Nombre, string? Telefono)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                TempData["Mensaje"] = "El nombre no puede estar vacío.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(MiPerfil));
            }

            var agenteID = ObtenerAgenteID();
            var agente = await _context.Usuarios.FindAsync(agenteID);

            if (agente == null)
                return NotFound();

            agente.Nombre = Nombre.Trim();
            agente.Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim();

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Perfil actualizado correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(MiPerfil));
        }

        // ──────────────────────────────────────────────────────────────
        // GET /Agentes/MisConsultas  (RF205)
        // ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> MisConsultas()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();

            var agendas = await _context.Agendas
                .Include(a => a.Usuario)
                .Include(a => a.Propiedad)
                .Include(a => a.EstadoAgenda)
                .Where(a => a.Propiedad.AgenteID == agenteID)
                .OrderByDescending(a => a.FechaVisita)
                .ToListAsync();

            return View(agendas);
        }

        // ──────────────────────────────────────────────────────────────
        // POST /Agentes/CambiarEstadoCita  (RF206)
        // ──────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoCita(int agendaID, int estadoID)
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();

            var agenda = await _context.Agendas
                .Include(a => a.Propiedad)
                .FirstOrDefaultAsync(a => a.AgendaID == agendaID
                                       && a.Propiedad.AgenteID == agenteID);

            if (agenda == null)
            {
                TempData["Mensaje"] = "Cita no encontrada o no autorizada.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(MisConsultas));
            }

            agenda.EstadoAgendaID = estadoID;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Estado de la cita actualizado.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(MisConsultas));
        }

        // ──────────────────────────────────────────────────────────────
        // GET /Agentes/MisProyectos  (RF208)
        // ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> MisProyectos()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            // Los proyectos son globales (no tienen AgenteID en el modelo actual).
            // Se muestran todos los proyectos disponibles según SRS RF110.
            var proyectos = await _context.Proyectos
                .Include(p => p.EstadoProyecto)
                .Include(p => p.Constructora)
                .ToListAsync();

            return View(proyectos);
        }

        // ──────────────────────────────────────────────────────────────
        // GET /Agentes/Estadisticas  (RF207)
        // ──────────────────────────────────────────────────────────────
        public async Task<IActionResult> Estadisticas()
        {
            if (!EsAgente())
                return RedirectToAction("Index", "Login");

            var agenteID = ObtenerAgenteID();

            var totalPropiedades = await _context.Propiedades
                .CountAsync(p => p.AgenteID == agenteID);

            var totalAgendas = await _context.Agendas
                .CountAsync(a => a.Propiedad.AgenteID == agenteID);

            var agendasConfirmadas = await _context.Agendas
                .CountAsync(a => a.Propiedad.AgenteID == agenteID
                              && a.EstadoAgenda.Descripcion == "Confirmada");

            ViewBag.TotalPropiedades = totalPropiedades;
            ViewBag.TotalAgendas = totalAgendas;
            ViewBag.AgendasConfirmadas = agendasConfirmadas;
            ViewBag.AgendasPendientes = totalAgendas - agendasConfirmadas;

            return View();
        }

        public IActionResult AgenteEditarPerfil()
        {
            return RedirectToAction("Index", "AgenteEditarPerfil");
        }
    }
}
