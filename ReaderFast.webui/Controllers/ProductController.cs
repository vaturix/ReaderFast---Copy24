using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReaderFast.webui.Data;
using ReaderFast.webui.Models;
using ReaderFast.webui.ViewModels;

namespace ReaderFast.webui.Controllers
{
   [Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly ReaderFastDbContext _context; // YourDbContext yerine gerçek DbContext isminizi kullanın

        public ProductController(ReaderFastDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // Bu satırda, veritabanınızdan tüm ürünleri çekebilirsiniz. Eğer Entity Framework kullanıyorsanız, EF ile veritabanından veri çekme işlemi yapabilirsiniz.
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = viewModel.Name,
                    Category = viewModel.Category,
                    Price = viewModel.Price
                };
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);
                product.Name = viewModel.Name;
                product.Category = viewModel.Category;
                product.Price = viewModel.Price;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // Index ve diğer action metodları burada olacak
    }

}
