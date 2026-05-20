using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class GestionUsuariosController : Controller
    {
        private readonly SkyinitContext _context;

        public GestionUsuariosController(SkyinitContext context)
        {
            _context = context;
        }

        // ─── HELPER: verifica que el usuario en sesión sea Administrador ───────
        private async Task<bool> EsAdministradorAsync()
        {
            // El claim "UsuarioID" es guardado tanto en Google OAuth como (una vez
            // corregido E01) en el login tradicional.
            var idStr = User.FindFirst("UsuarioID")?.Value;
            if (!int.TryParse(idStr, out int id)) return false;

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioID == id);

            return usuario?.Rol?.NombreRol == "Administrador";
        }

        // ─── GET: /GestionUsuarios/Index ────────────────────────────────────────
      
        public async Task<IActionResult> Index(string? buscar, string? filtroRol, string? filtroEstado)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var query = _context.Usuarios
                .Include(u => u.Rol)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim().ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(buscar) ||
                    u.Correo.ToLower().Contains(buscar));
            }

            if (!string.IsNullOrWhiteSpace(filtroRol))
                query = query.Where(u => u.Rol.NombreRol == filtroRol);

            if (!string.IsNullOrWhiteSpace(filtroEstado))
                query = query.Where(u => u.EstadoCuenta == filtroEstado);

            ViewBag.Buscar = buscar;
            ViewBag.FiltroRol = filtroRol;
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.Roles = await _context.Roles.ToListAsync();

            return View(await query.OrderBy(u => u.Nombre).ToListAsync());
        }

        // ─── GET: /GestionUsuarios/Crear ────────────────────────────────────────
        // RF235 – Permitir crear usuarios
        public async Task<IActionResult> Crear()
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        // ─── POST: /GestionUsuarios/Crear ───────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            string Nombre, string Correo, string Contrasena,
            string? Telefono, string EstadoCuenta, int RolID)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            // RF08 / RF04 – Evitar correos duplicados y campos vacíos
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Correo) ||
                string.IsNullOrWhiteSpace(Contrasena))
            {
                TempData["Error"] = "Nombre, correo y contraseña son obligatorios.";
                TempData["TipoMsg"] = "error";
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View();
            }

            if (_context.Usuarios.Any(u => u.Correo == Correo))
            {
                TempData["Error"] = "El correo ya está registrado.";
                TempData["TipoMsg"] = "error";
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View();
            }

            // RF10 – Longitud mínima de contraseña
            if (Contrasena.Length < 8)
            {
                TempData["Error"] = "La contraseña debe tener al menos 8 caracteres.";
                TempData["TipoMsg"] = "error";
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View();
            }

            var usuario = new Usuario
            {
                Nombre = Nombre.Trim(),
                Correo = Correo.Trim().ToLower(),
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(Contrasena),
                Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim(),
                EstadoCuenta = EstadoCuenta,
                RolID = RolID,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Usuario '{usuario.Nombre}' creado correctamente.";
            TempData["TipoMsg"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // ─── GET: /GestionUsuarios/Editar/5 ────────────────────────────────────
     
        public async Task<IActionResult> Editar(int id)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioID == id);

            if (usuario == null) return NotFound();

            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(usuario);
        }

        // ─── POST: /GestionUsuarios/Editar/5 ───────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id, string Nombre, string Correo,
            string? NuevaContrasena, string? Telefono,
            string EstadoCuenta, int RolID)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // RF40 – Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Correo))
            {
                TempData["Error"] = "Nombre y correo son obligatorios.";
                TempData["TipoMsg"] = "error";
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View(usuario);
            }

            // RF08 – Evitar correos duplicados en otro usuario
            if (_context.Usuarios.Any(u => u.Correo == Correo && u.UsuarioID != id))
            {
                TempData["Error"] = "Ese correo ya está en uso por otro usuario.";
                TempData["TipoMsg"] = "error";
                ViewBag.Roles = await _context.Roles.ToListAsync();
                return View(usuario);
            }

            // Campos editables (RF permitidos para Administrador)
            usuario.Nombre = Nombre.Trim();
            usuario.Correo = Correo.Trim().ToLower();
            usuario.Telefono = string.IsNullOrWhiteSpace(Telefono) ? null : Telefono.Trim();
            usuario.EstadoCuenta = EstadoCuenta;
            usuario.RolID = RolID;

            // RF39 – Cambiar contraseña solo si se proporcionó una nueva
            if (!string.IsNullOrWhiteSpace(NuevaContrasena))
            {
                if (NuevaContrasena.Length < 8)
                {
                    TempData["Error"] = "La nueva contraseña debe tener al menos 8 caracteres.";
                    TempData["TipoMsg"] = "error";
                    ViewBag.Roles = await _context.Roles.ToListAsync();
                    return View(usuario);
                }
                usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(NuevaContrasena);
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Usuario '{usuario.Nombre}' actualizado correctamente.";
            TempData["TipoMsg"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // ─── POST: /GestionUsuarios/Activar/5 ──────────────────────────────────
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.EstadoCuenta = "Activa";
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Usuario '{usuario.Nombre}' activado.";
            TempData["TipoMsg"] = "success";
            return RedirectToAction(nameof(Index));
        }

        // ─── POST: /GestionUsuarios/Desactivar/5 ───────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.EstadoCuenta = "Inactiva";
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Usuario '{usuario.Nombre}' desactivado.";
            TempData["TipoMsg"] = "warning";
            return RedirectToAction(nameof(Index));
        }

        // ─── POST: /GestionUsuarios/AsignarRol ─────────────────────────────────
        // RF239 – Asignar roles a usuarios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRol(int id, int RolID)
        {
            if (!await EsAdministradorAsync())
                return RedirectToAction("Index", "Login");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.RolID = RolID;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = $"Rol actualizado para '{usuario.Nombre}'.";
            TempData["TipoMsg"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}
