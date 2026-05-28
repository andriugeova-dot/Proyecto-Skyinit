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
        .Include(p => p.Imagenes)        // ← NUEVO: carga las imágenes
        .Include(p => p.TipoOperacion)   // ← NUEVO: carga el tipo (Venta/Arriendo/Empeño)
        .Take(6)                         // ← NUEVO: máximo 6 propiedades destacadas
        .ToList();

            return View(propiedades);
        }

        // Alias para que la ruta /Menu/Inicio también funcione
        public IActionResult Inicio()
        {
            return RedirectToAction("Index");
        }

        
        public IActionResult Buscar()
        {
            return RedirectToAction("Index", "UsuarioPropiedades");
        }

        
        public IActionResult Proyectos()
        {
            return RedirectToAction("Index", "Proyectos");
        }

        
        public IActionResult Constructoras()
        {
            return RedirectToAction("Index", "Constructoras");
        }

        
        public IActionResult Servicios()
        {
            return RedirectToAction("Index", "Servicios");
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
