using Microsoft.AspNetCore.Mvc;

namespace Proyecto_SkyInit.Controllers
{
    public class TerminosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmarRegistro()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
