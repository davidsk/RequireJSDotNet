using Microsoft.AspNetCore.Mvc;

namespace MVC6.Foo.Controllers
{
    [Area("Foo")]
    public class Home: Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}