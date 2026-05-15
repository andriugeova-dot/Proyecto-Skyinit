using Microsoft.AspNetCore.Mvc;

namespace Proyecto_SkyInit.Controllers
{
    public class AdministradorController : Controller
    {
        public IActionResult PublicarPropiedad()
        {
            return RedirectToAction("Index", "PublicarPropiedad");
        }
        public IActionResult PublicarProyecto()
        {
            return RedirectToAction("Index", "PublicarProyecto");
        }
        public IActionResult RegistarAgente()
        {
            return RedirectToAction("Index", "RegistrarAgente");
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
        public IActionResult Index()
        {
            return View();
        }
    }
}
