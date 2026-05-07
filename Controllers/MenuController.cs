using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Models;
using Proyecto_SkyInit.Data;

namespace Proyecto_SkyInit.Controllers
{
    public class MenuController : Controller
    {
        private readonly SkyinitContext _context;

        public MenuController(SkyinitContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            ViewData["Title"] = "Inicio — SkyInit";
            return View();
        }

        // Alias para que la ruta /Menu/Inicio también funcione
        public IActionResult Inicio()
        {
            return RedirectToAction("Index");
        }

        // ════════════════════════════════════════════════════════════
        // BUSCAR → PropiedadesController/Index
        // ════════════════════════════════════════════════════════════
        public IActionResult Buscar()
        {
            return RedirectToAction("Index", "Propiedades");
        }

        // ════════════════════════════════════════════════════════════
        // PROYECTOS → ProyectosController/Index
        // ════════════════════════════════════════════════════════════
        public IActionResult Proyectos()
        {
            return RedirectToAction("Index", "Proyectos");
        }

        // ════════════════════════════════════════════════════════════
        // CONSTRUCTORAS → ConstructorasController/Index
        // ════════════════════════════════════════════════════════════
        public IActionResult Constructoras()
        {
            return RedirectToAction("Index", "Constructoras");
        }

        // ════════════════════════════════════════════════════════════
        // SERVICIOS → ServiciosController/Index
        // ════════════════════════════════════════════════════════════
        public IActionResult Servicios()
        {
            return RedirectToAction("Index", "Servicios");
        }

        // ════════════════════════════════════════════════════════════
        // REGISTRAR → RegistroController/Index
        // (apunta al controlador existente "Registro" ya que no existe "Usuarios")
        // ════════════════════════════════════════════════════════════
        public IActionResult Registrar()
        {
            return RedirectToAction("Index", "Login");
        }

        // ════════════════════════════════════════════════════════════
        // LOGIN → LoginController/Index
        // ════════════════════════════════════════════════════════════
        public IActionResult Login()
        {
            return RedirectToAction("Index", "Login");
        }

        // ════════════════════════════════════════════════════════════
        // CERRAR SESIÓN → LoginController/Logout
        // ════════════════════════════════════════════════════════════
        public IActionResult CerrarSesion()
        {
            return RedirectToAction("Logout", "Login");
        }

        // ════════════════════════════════════════════════════════════
        // TÉRMINOS Y CONDICIONES → TerminosController/Index
        // ════════════════════════════════════════════════════════════
        public IActionResult Terminos()
        {
            return RedirectToAction("Index", "Terminos");
        }
    }
}
