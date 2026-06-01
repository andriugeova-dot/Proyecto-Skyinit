using Microsoft.AspNetCore.Mvc;

namespace Proyecto_SkyInit.Controllers
{
    public class PanelUsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
