using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class GestionServiciosController : Controller
    {
        private readonly SkyinitContext _context;
        private readonly IWebHostEnvironment _env;

        public GestionServiciosController(SkyinitContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ── Admin: lista completa con búsqueda y filtro ──────────────────────
        public async Task<IActionResult> Index(string buscar, string filtroEstado)
        {
            var query = _context.ServiciosMantenimiento.AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(s => s.Nombre.Contains(buscar) || s.Descripcion.Contains(buscar));

            if (!string.IsNullOrEmpty(filtroEstado))
                query = query.Where(s => s.Estado == filtroEstado);

            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            return View(await query.ToListAsync());
        }

        // ── Crear ────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Crear(ServicioMantenimiento model, IFormFile? imagenFile)
        {
            if (imagenFile != null && imagenFile.Length > 0)
                model.Imagen = await GuardarImagen(imagenFile);

            _context.ServiciosMantenimiento.Add(model);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Servicio creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── Editar ───────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Editar(ServicioMantenimiento model, IFormFile? imagenFile)
        {
            var existente = await _context.ServiciosMantenimiento.FindAsync(model.ServicioID);
            if (existente == null) return NotFound();

            existente.Nombre = model.Nombre;
            existente.Descripcion = model.Descripcion;
            existente.Precio = model.Precio;
            existente.Estado = model.Estado;

            if (imagenFile != null && imagenFile.Length > 0)
                existente.Imagen = await GuardarImagen(imagenFile);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Servicio actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── Eliminar ─────────────────────────────────────────────────────────
        public async Task<IActionResult> Eliminar(int id)
        {
            var servicio = await _context.ServiciosMantenimiento.FindAsync(id);
            if (servicio != null)
            {
                _context.ServiciosMantenimiento.Remove(servicio);
                await _context.SaveChangesAsync();
            }

            TempData["Mensaje"] = "Servicio eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // ── Helper: guardar imagen ───────────────────────────────────────────
        private async Task<string> GuardarImagen(IFormFile archivo)
        {
            var carpeta = Path.Combine(_env.WebRootPath, "img", "Servicios");
            Directory.CreateDirectory(carpeta);
            var nombre = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
            var ruta = Path.Combine(carpeta, nombre);
            using var stream = new FileStream(ruta, FileMode.Create);
            await archivo.CopyToAsync(stream);
            return $"/img/Servicios/{nombre}";
        }
    }
}