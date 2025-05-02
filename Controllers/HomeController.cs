using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using secure_login_system.Models;

namespace secure_login_system.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Loads Views/Home/Index.cshtml
        }

        public IActionResult Privacy()
        {
            return View(); // Loads Views/Home/Privacy.cshtml
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
