using Microsoft.AspNetCore.Mvc;
using ReaderFast.webui.Models;
using Microsoft.AspNetCore.Identity;
using ReaderFast.webui.Areas.Identity.Data;
using System.Threading.Tasks;
using ReaderFast.webui.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Localization;
using ReaderFast.webui.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReaderFast.webui.Controllers
{
    [Authorize]
    public class ExerciseController : Controller
    {
        private readonly ReaderFastDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IViewLocalizer _localizer; // Bu satırı ekleyin

        public ExerciseController(IViewLocalizer localizer, ReaderFastDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _localizer = localizer; // Bu satırı ekleyin
        }

        public IActionResult MainPage()
        {
            return View();
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var dayNumberToExclude = 30; // Gözükmemesini istediğiniz gün numarasını buraya yazın

            var exerciseDays = _context.ExerciseDays
                                       .Where(ed => ed.DayNumber != dayNumberToExclude) // Bu satırı ekleyin
                                       .Include(ed => ed.ExerciseExerciseDays)
                                       .ThenInclude(eed => eed.Exercise)
                                       .ToList();

            var userExercises = _context.UserExercises
                                        .Where(ue => ue.UserId == userId)
                                        .ToList();

            var viewModel = exerciseDays.Select(ed => new Models.ExerciseDayViewModel
            {
                ExerciseDay = ed,
                Exercises = ed.ExerciseExerciseDays.Select(eed => new Models.ExerciseViewModel
                {
                    Exercise = eed.Exercise,
                    IsCompleted = userExercises.Any(ue => ue.ExerciseExerciseDayId == eed.Id && ue.IsCompleted),
                    ExerciseExerciseDayId = eed.Id // Bu satırı ekleyin
                }).ToList()
            }).ToList();

            var isPremiumUser = User.IsInRole("PremiumUser");
            ViewBag.IsPremiumUser = isPremiumUser;

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> CompleteDay(int dayId)
        {
            var userId = _userManager.GetUserId(User);

            var day = _context.ExerciseDays
                              .Include(ed => ed.ExerciseExerciseDays)
                              .ThenInclude(eed => eed.Exercise)
                              .SingleOrDefault(d => d.Id == dayId);

            if (day != null)
            {
                foreach (var exerciseExerciseDay in day.ExerciseExerciseDays)
                {
                    var userExercise = new UserExercise
                    {
                        UserId = userId,
                        ExerciseExerciseDayId = exerciseExerciseDay.Id,
                        IsCompleted = true
                    };
                    _context.UserExercises.Add(userExercise);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> CompleteExercise(int exerciseExerciseDayId) // Parametre ismini güncelledim
        {
            var userId = _userManager.GetUserId(User);

            var exerciseExerciseDay = _context.ExerciseExerciseDays
                                              .FirstOrDefault(eed => eed.Id == exerciseExerciseDayId); // Bu satırı güncelledim

            if (exerciseExerciseDay != null)
            {
                var existingUserExercise = _context.UserExercises
                                                   .FirstOrDefault(ue => ue.UserId == userId && ue.ExerciseExerciseDayId == exerciseExerciseDay.Id);

                if (existingUserExercise == null)
                {
                    var newUserExercise = new UserExercise
                    {
                        UserId = userId,
                        ExerciseExerciseDayId = exerciseExerciseDay.Id,
                        IsCompleted = true
                    };

                    _context.UserExercises.Add(newUserExercise);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (!existingUserExercise.IsCompleted)
                    {
                        existingUserExercise.IsCompleted = true;
                        _context.UserExercises.Update(existingUserExercise);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ShowExercise(int id, string source)
        {
            var exerciseExerciseDay = await _context.ExerciseExerciseDays
                                                    .Include(eed => eed.Exercise)
                                                    .FirstOrDefaultAsync(eed => eed.Id == id);

            if (exerciseExerciseDay == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var existingUserExercise = await _context.UserExercises
                                                     .FirstOrDefaultAsync(ue => ue.UserId == userId && ue.ExerciseExerciseDayId == exerciseExerciseDay.Id);

           


            if (existingUserExercise == null)
            {
                var newUserExercise = new UserExercise
                {
                    UserId = userId,
                    ExerciseExerciseDayId = exerciseExerciseDay.Id,
                    IsCompleted = true
                };

                _context.UserExercises.Add(newUserExercise);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (!existingUserExercise.IsCompleted)
                {
                    existingUserExercise.IsCompleted = true;
                    await _context.SaveChangesAsync();
                }
            }

            // Filter and order ExerciseExerciseDays for the specific day
            var exercisesForTheDay = await _context.ExerciseExerciseDays
                                                   .Where(eed => eed.ExerciseDayId == exerciseExerciseDay.ExerciseDayId)
                                                   .OrderBy(eed => eed.SequenceNumber)
                                                   .ToListAsync();

            // Find the index of the current ExerciseExerciseDay
            int currentIndex = exercisesForTheDay.FindIndex(eed => eed.Id == id);

            // Find the previous and next ExerciseExerciseDay based on the index
            var prevExercise = currentIndex > 0 ? exercisesForTheDay[currentIndex - 1] : null;
            var nextExercise = currentIndex < exercisesForTheDay.Count - 1 ? exercisesForTheDay[currentIndex + 1] : null;

            // Pass the IDs to the ViewBag
            ViewBag.PrevId = prevExercise?.Id;
            ViewBag.NextId = nextExercise?.Id;

            ViewBag.Source = source;


            return View(exerciseExerciseDay.Exercise.ViewName); // Exercise'in view'ini render edin.
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
        [HttpPost]
        public IActionResult IncreaseDifficulty(string returnUrl)
        {
            // Mevcut zorluk seviyesini cookie'den oku veya varsayılan değeri kullan
            int currentDifficulty = int.Parse(Request.Cookies["userDifficulty"] ?? "1");

            // Zorluk seviyesini artır
            currentDifficulty++;

            // Yeni zorluk seviyesini cookie'ye kaydet
            Response.Cookies.Append("userDifficulty", currentDifficulty.ToString(), new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return LocalRedirect(returnUrl);
        }


        [HttpGet]
        public async Task<IActionResult> GetSentences(int difficulty = 1)
        {
            // Kullanıcının mevcut dil tercihini al
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var userLanguage = requestCulture.RequestCulture.UICulture.Name;

            var sentences = await _context.Sentences
                                           .Where(s => s.Language == userLanguage && s.Difficulty == difficulty)
                                           .OrderBy(r => Guid.NewGuid()) // Rastgele sıralama için
                                           .Select(s => s.Text)
                                           .ToListAsync();

            if (!sentences.Any())
            {
                return Json(new { status = "completed" }); // Eğer cümle yoksa, egzersizin tamamlandığını belirt
            }

            return Json(sentences);
        }



        public async Task<IActionResult> Practice()
        {
            var dayNumber = 30; // Özel gün numarası
            var exercises = await _context.ExerciseExerciseDays
                                           .Where(eed => eed.ExerciseDay.DayNumber == dayNumber)
                                           .Select(eed => new ViewModels.PracticeViewModel
                                           {
                                               ExerciseExerciseDayId = eed.Id, // ExerciseExerciseDay ID'sini seç
                                               Name = eed.Exercise.Name,
                                               Description = eed.Exercise.Description,
                                           })
                                           .ToListAsync();

            return View(exercises);
        

    }

    [HttpGet]
        public async Task<IActionResult> ListStories(int difficulty = 1)
        {
            var requestCulture = HttpContext.Features.Get<IRequestCultureFeature>();
            var userLanguage = requestCulture.RequestCulture.UICulture.Name;

            var stories = await _context.Stories
                .Where(s => s.Language == userLanguage && s.Difficulty == difficulty)
                .Select(s => new { s.Id, s.Title }) // s.Title eklenmeli
                .ToListAsync();

            return Json(stories);
        }
        [HttpGet]
        public async Task<IActionResult> GetStoryContent(int storyId)
        {
            var story = await _context.Stories
                                      .Where(s => s.Id == storyId)
                                      .Select(s => new { s.Content })
                                      .FirstOrDefaultAsync();

            if (story == null)
            {
                return NotFound("Hikaye bulunamadı.");
            }

            return Json(story);
        }


    }
}
