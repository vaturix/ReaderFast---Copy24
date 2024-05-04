using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReaderFast.webui.Data;
using ReaderFast.webui.Models;
using ReaderFast.webui.ViewModels;
using System.Linq;

namespace ReaderFast.webui.Controllers
{
   //[Authorize(Roles = "Admin")]

    public class StoriesController : Controller
    {
        private readonly ReaderFastDbContext _context;

        public StoriesController(ReaderFastDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var stories = _context.Stories
                .Select(s => new StoryViewModel { Id = s.Id, Title = s.Title, Content = s.Content, Difficulty = s.Difficulty, Language = s.Language })
                .ToList();
            return View(stories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var story = new Story { Title = viewModel.Title, Content = viewModel.Content, Difficulty = viewModel.Difficulty, Language = viewModel.Language};
                _context.Stories.Add(story);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }
    }
}
