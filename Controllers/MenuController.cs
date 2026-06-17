using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_SkyInit.Data;
using Proyecto_SkyInit.Models;
using System.Linq;

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
            ViewData["Title"] = "SkyInit";

            var propiedades = _context.Propiedades
            .Include(p => p.Imagenes)        
            .Include(p => p.TipoOperacion)   
            .Take(6)                         
            .ToList();
            MarcarFavoritos(propiedades);

            if (User.Identity!.IsAuthenticated)
            {
                var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);
                var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);

                ViewBag.FotoPerfil = usuario?.FotoPerfil ?? "/img/default-avatar.svg";
            }
            return View(propiedades);
        }

        private void MarcarFavoritos(List<Propiedad> propiedades)
        {
            if (User.Identity!.IsAuthenticated)
            {
                var usuarioId = int.Parse(User.FindFirst("UsuarioID")!.Value);
                var favoritos = _context.Favoritos
                .Where(f => f.UsuarioID == usuarioId)
                .Select(f => f.PropiedadID)
                .ToList();

                foreach (var p in propiedades)
                {
                    p.EsFavorito = favoritos.Contains(p.PropiedadID);
                }
            }
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
