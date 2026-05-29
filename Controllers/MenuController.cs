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

            var propiedades = _context.Propiedades
        .Include(p => p.Imagenes)        
        .Include(p => p.TipoOperacion)   
        .Take(6)                         
        .ToList();

            return View(propiedades);
        }

        // Alias para que la ruta /Menu/Inicio también funcione
        public IActionResult Inicio()
        {
            return RedirectToAction("Index");
        }

        
        public IActionResult Propiedades()
        {
            return RedirectToAction("Index", "UsuarioPropiedades");
        }

        
        public IActionResult Proyectos()
        {
            return RedirectToAction("Index", "UsuarioProyectos");
        }

        
        public IActionResult Constructoras()
        {
            return RedirectToAction("Index", "Constructoras");
        }

        
        public IActionResult Servicios()
        {
            return RedirectToAction("Index", "UsuariosMantenimientos");
        }

        
        public IActionResult Registrar()
        {
            return RedirectToAction("Index", "Login");
        }

        
        public IActionResult Login()
        {
            return RedirectToAction("Index", "Login");
        }

        
        public IActionResult CerrarSesion()
        {
            return RedirectToAction("Logout", "Login");
        }

        
        public IActionResult Terminos()
        {
            return RedirectToAction("Index", "Terminos");
        }
    }
}
