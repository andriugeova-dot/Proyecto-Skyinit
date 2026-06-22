using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Data;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuariosConstructoraController : Controller
    {
        private readonly SkyinitContext _context;
        public UsuariosConstructoraController(SkyinitContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var constructoras = _context.Constructoras
           .Include(c => c.Imagenes)

           .Where(c => c.Estado == "Activo")
           .Select(c => new ConstructoraViewModel
           {
               Id = c.ConstructoraID,
               Nombre = c.Nombre,


               Descripcion = c.Descripcion != null ? c.Descripcion : "Gran constructora",
               Ciudad = c.Ciudad != null ? c.Ciudad : "Sede Principal",

               Logo = c.Imagenes.FirstOrDefault() != null
                      ? c.Imagenes.FirstOrDefault().URL
                      : "default-avatar.svg",


               CantidadProyectos = c.Proyectos.Count()
           })
           .ToList();

            return View(constructoras);
        }
    }
}