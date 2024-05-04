using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using ReaderFast.webui.Areas.Identity.Data;
using ReaderFast.webui.Data;
using ReaderFast.webui.Models;
using System.Diagnostics;

namespace ReaderFast.webui.Controllers
{



    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ReaderFastDbContext _context;
        private readonly IViewLocalizer _localizer; // Bu satırı ekleyin
                                                    // DbContext instance'ınız

        public HomeController(IViewLocalizer localizer, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ReaderFastDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context; // DbContext instance'ınızı set edin
            _localizer = localizer; // Bu satırı ekleyin

        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["UserID"] = _userManager.GetUserId(this.User);

            var firstProduct = _context.Products.FirstOrDefault();
            if (firstProduct != null)
            {
                ViewData["Price"] = firstProduct.Price;
           
            }

            return View();
        }
    


    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}