using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class UsuarioProyectosController : Controller
    {
        private readonly SkyinitContext _context;

        public UsuarioProyectosController(SkyinitContext context)
        {
            _context = context;
        }
      

       
        
        public IActionResult Index()
        {
            ViewData["Title"] = "Proyectos — SkyInit";
            var proyectos = _context.Proyectos.Include(p => p.Constructora)
                .Include(p => p.EstadoProyecto)
           .ToList();

            return View(proyectos);
        }
    //REDIRECCIÓN A MENU//
    public IActionResult Inicio()
        {
            return RedirectToAction("Inicio","Menu");
        }
    }
}
