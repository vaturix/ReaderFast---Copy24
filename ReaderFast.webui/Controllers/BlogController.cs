using Microsoft.AspNetCore.Mvc;

namespace ReaderFast.webui.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
