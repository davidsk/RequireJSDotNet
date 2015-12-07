using Microsoft.AspNet.Mvc;

namespace MVC6.Controllers
{
    public class Home: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}