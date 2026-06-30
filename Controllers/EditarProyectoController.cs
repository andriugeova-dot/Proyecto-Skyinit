using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class EditarProyectoController : Controller
    {
        private readonly SkyinitContext _context;

        public EditarProyectoController(SkyinitContext context)
        {
            _context = context;
        }

        // GET: EditarProyecto
        public IActionResult Editar(int id)
        {
            var proyecto = _context.Proyectos
                .Include(p => p.EstadoProyecto)
                .Include(p => p.Constructora)
                .Include(p => p.ImagenesProyecto)
                .FirstOrDefault(p => p.ProyectoID == id);

            if (proyecto == null) return NotFound();

            PoblarSelects(proyecto);
            return View("Index", proyecto);
        }

        // POST: EditarProyecto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Proyecto modelo, IFormFile? Imagenes)
        {
            if (!ModelState.IsValid)
            {
                PoblarSelects(modelo);
                TempData["Mensaje"] = "❌ Error de validación";
                TempData["TipoMensaje"] = "error";
                return View("Index", modelo);
            }

            var proyectoDb = _context.Proyectos
                .FirstOrDefault(p => p.ProyectoID == modelo.ProyectoID);

            if (proyectoDb == null) return NotFound();

            proyectoDb.Nombre = modelo.Nombre;
            proyectoDb.EstadoProyectoID = modelo.EstadoProyectoID;
            proyectoDb.FechaInicio = modelo.FechaInicio;
            proyectoDb.FechaFin = modelo.FechaFin;
            proyectoDb.ConstructoraID = modelo.ConstructoraID;
            proyectoDb.Descripcion = modelo.Descripcion;

            await _context.SaveChangesAsync();

            await GuardarImagenes(modelo.ProyectoID, Imagenes,true);

            TempData["Mensaje"] = "✅ Proyecto actualizado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction("Index", "GestionProyectos");
        }

        private void PoblarSelects(Proyecto proyecto)
        {
            ViewBag.EstadosProyecto = new SelectList(_context.EstadosProyecto.ToList(), "EstadoProyectoID", "Descripcion",proyecto.EstadoProyectoID);
            ViewBag.Constructoras = new SelectList(_context.Constructoras.ToList(), "ConstructoraID", "Nombre", proyecto.ConstructoraID);
        }
        private async Task GuardarImagenes(int proyectoID, IFormFile Imagenes, bool EsEdicion = false)
        {
            if (Imagenes == null || Imagenes.Length == 0)
            {
                if (EsEdicion) return;
                throw new InvalidOperationException("Debe subir una imagen al crear un proyecto");
            }
            var imagenAnterior = _context.ImagenesProyecto
              .FirstOrDefault(i => i.ProyectoID == proyectoID);

            if (imagenAnterior != null)
            {
                var rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagenAnterior.URL.TrimStart('/'));
                if (System.IO.File.Exists(rutaAnterior))
                    System.IO.File.Delete(rutaAnterior);

                _context.ImagenesProyecto.Remove(imagenAnterior);
            }


            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/proyectos");
            Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Imagenes.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Imagenes.CopyToAsync(stream);
            }
            _context.ImagenesProyecto.Add(new ImagenProyecto
            {
                ProyectoID = proyectoID,
                URL = "/img/proyectos/" + fileName
            });
            await _context.SaveChangesAsync();
        }
    }
}
