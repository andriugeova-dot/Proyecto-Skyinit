using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Models;
using Proyecto_SkyInit.Data;
using static System.Net.Mime.MediaTypeNames;

namespace Proyecto_SkyInit.Controllers
{
    public class GestionConstructorasController : Controller
    {
        private readonly SkyinitContext _context;

        public GestionConstructorasController(SkyinitContext context)
        {
            _context = context;
        }

        public IActionResult Index(string buscar, string filtroEstado)
        {
            var query = _context.Constructoras.AsQueryable();

           
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(c => c.Nombre.Contains(buscar) || c.Contacto.Contains(buscar));
            }

            
            if (!string.IsNullOrEmpty(filtroEstado))
            {
                query = query.Where(c => c.Estado == filtroEstado);
            }

           
            var model = query.Select(c => new ConstructoraViewModel
            {
                Id = c.ConstructoraID,
                Nombre = c.Nombre,
                Contacto = c.Contacto,
                Telefono = c.Telefono,
                Correo = c.Correo,
                Estado = c.Estado,
                Descripcion= c.Descripcion,
                Ciudad=c.Ciudad,
                Logo = c.Imagenes.FirstOrDefault(i => i.EsLogo).URL
            }).ToList();

           
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            return View(model);
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Crear(ConstructoraViewModel model, IFormFile Logo)
        {
            // 1. Primero guarda la constructora
            var constructora = new Constructora
            {
                Nombre = model.Nombre,
                Contacto = model.Contacto,
                Telefono = model.Telefono,
                Correo = model.Correo,
                Descripcion= model.Descripcion,
                Ciudad= model.Ciudad,
                Estado = model.Estado
            };

            _context.Constructoras.Add(constructora);
            _context.SaveChanges(); // ✅ Aquí ya tiene ID

            // 2. Luego guarda la imagen con el ID real
            if (Logo != null && Logo.Length > 0)
            {
                var fileName = Path.GetFileName(Logo.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                    Logo.CopyTo(stream);

                var imagen = new ImagenesConstructoras
                {
                    ConstructoraId = constructora.ConstructoraID, // ✅ Ahora sí tiene ID
                    URL = fileName,
                    EsLogo = true
                };
                _context.ImagenesConstructoras.Add(imagen);
                _context.SaveChanges();
            }

            TempData["Mensaje"] = "Constructora agregada correctamente";
            return RedirectToAction("Index");
        }
        public IActionResult Eliminar(int id)
        {
            var constructora = _context.Constructoras.Find(id);
            if (constructora != null)
            {
                _context.Constructoras.Remove(constructora);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Editar(ConstructoraViewModel model, IFormFile Logo)
        {
            var constructora = _context.Constructoras.Find(model.Id);
            if (constructora != null)
            {
                constructora.Nombre = model.Nombre;
                constructora.Contacto = model.Contacto;
                constructora.Telefono = model.Telefono;
                constructora.Correo = model.Correo;
                constructora.Descripcion = model.Descripcion;
                constructora.Ciudad = model.Ciudad;
                constructora.Estado = model.Estado;
                _context.SaveChanges(); // ✅ Guarda cambios de la constructora

                // ✅ Maneja la imagen en tabla Imagenes
                if (Logo != null && Logo.Length > 0)
                {
                    var fileName = Path.GetFileName(Logo.FileName);
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(folder);
                    var path = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                        Logo.CopyTo(stream);

                    // Busca si ya tiene logo y lo actualiza, si no crea uno nuevo
                    var logoExistente = _context.ImagenesConstructoras
                        .FirstOrDefault(i => i.ConstructoraId == constructora.ConstructoraID && i.EsLogo == true);

                    if (logoExistente != null)
                    {
                        logoExistente.URL = fileName; // ✅ Actualiza
                    }
                    else
                    {
                        _context.ImagenesConstructoras.Add(new ImagenesConstructoras
                        {
                            ConstructoraId = constructora.ConstructoraID,
                            URL = fileName,
                            EsLogo = true
                        });
                    }
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }






    }
}
