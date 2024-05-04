using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using ReaderFast.webui.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ReaderFast.webui.Models;

namespace ReaderFast.webui.Controllers
{
   [Authorize(Roles = "Admin")]
    public class AppRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        // Tek bir constructor kullandık
        public AppRolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

       

        //List All the Roles created by Users
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault()
                });
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(IdentityRole model)
        {
            if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> UpdateRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // Hata mesajınızı burada ayarlayabilirsiniz
                return View("Error", new ErrorViewModel { Message = "UserId cannot be null or empty" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Hata mesajınızı burada ayarlayabilirsiniz
                return View("Error", new ErrorViewModel { Message = "User not found" });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            ViewData["UserId"] = userId;
            ViewData["UserRoles"] = new SelectList(userRoles);
            ViewData["AllRoles"] = new SelectList(allRoles, "Name", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(string userId, string selectedRole)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // Hata mesajınızı burada ayarlayabilirsiniz
                return View("Error", new ErrorViewModel { Message = "UserId cannot be null or empty" });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Hata mesajınızı burada ayarlayabilirsiniz
                return View("Error", new ErrorViewModel { Message = "User not found" });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (selectedRole == "PremiumUser")
            {
                user.PremiumRoleAssignedDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            await _userManager.AddToRoleAsync(user, selectedRole);

            return RedirectToAction("Index");
        }


    }
}
