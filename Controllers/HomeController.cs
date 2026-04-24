using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Proyecto_SkyInit.Models;

namespace Proyecto_SkyInit.Controllers;

public class HomeController : Controller
{
    private string connStr = "server=localhost;port=3306;database=skyinit;uid=root;password=andres.08;";

    // ✅ CORRECTO: el método se llama Index
    public IActionResult Index()
    {
        using (var conn = new MySqlConnection(connStr))
        {
            conn.Open();
            ViewBag.Estado = "✅ Conexión exitosa a skyinit";
        }        return View();
    }
}
//public class HomeController : Controller
//{
//    private readonly ILogger<HomeController> _logger;

//    public HomeController(ILogger<HomeController> logger)
//    {
//        _logger = logger;
//    }


//    public IActionResult Index()
//    {
//        return View();
//    }

//    public IActionResult Privacy()
//    {
//        return View();
//    }

//    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//    public IActionResult Error()
//    {
//        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//    }
//}
