using Microsoft.AspNetCore.Mvc;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly SkyinitContext _context;

        public AdministradorController(SkyinitContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalPropiedades = _context.Propiedades.Count(),
                TotalProyectos = _context.Proyectos.Count(),
                TotalUsuarios = _context.Usuarios.Count(),
                TotalAgentes = _context.Usuarios.Count(u => u.RolID == 2),
                TotalReparaciones = _context.Reparaciones.Count() 
            };

            return View(model);

        }

        public IActionResult PublicarPropiedad()
        {
            return RedirectToAction("Index", "PublicarPropiedad");
        }
        public IActionResult PublicarProyecto()
        {
            return RedirectToAction("Index", "PublicarProyecto");
        }
        public IActionResult RegistrarAgente()
        {
            return RedirectToAction("Index", "GestionUsuarios");
        }

        public IActionResult Propiedades()
        {
            return RedirectToAction("Index", "GestionPropiedades");
        }
        public IActionResult Proyectos()
        {
            return RedirectToAction("Index", "GestionProyectos");
        }
        public IActionResult Agentes()
        {
            return RedirectToAction("Index", "GestionUsuarios");
        }
        public IActionResult Mantenimiento()
        {
            return RedirectToAction("Index", "GestionMantenimiento");
        }
        public IActionResult Consultas()
        {
            return RedirectToAction("Index", "GestionConsultas");
        }
        public IActionResult Comentarios()
        {
            return RedirectToAction("Index", "GestionComentarios");
        }
        public IActionResult Certificaciones()
        {
            return RedirectToAction("Index", "GestionCertificaciones");
        }
        public IActionResult Historial()
        {
            return RedirectToAction("Index", "GestionHistorial");
        }
        public IActionResult Notificaciones()
        {
            return RedirectToAction("Index", "GestionNotificaciones");
        }
       
    }
}
