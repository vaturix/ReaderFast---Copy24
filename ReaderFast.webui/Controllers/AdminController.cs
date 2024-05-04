using Microsoft.AspNetCore.Mvc;
using ReaderFast.webui.Data;
using ReaderFast.webui.Models;
using System.Threading.Tasks;
using ReaderFast.webui.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ExerciseViewModel = ReaderFast.webui.ViewModels.ExerciseViewModel;

namespace ReaderFast.webui.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly ReaderFastDbContext _context;

        public AdminController(ReaderFastDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CreateExercise()
        {
            var viewModel = new ReaderFast.webui.ViewModels.ExerciseViewModel
            {
                Exercises = _context.Exercises.ToList() ?? new List<Exercise>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExercise(ReaderFast.webui.ViewModels.ExerciseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var exercise = new Exercise
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    IsPremium = viewModel.IsPremium,
                    ViewName = viewModel.ViewName,
                    IsCompleted = viewModel.IsCompleted,
                };
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateExercise));
            }
            return View(viewModel);
        }
        public IActionResult EditExercise(int id)
        {
            var exercise = _context.Exercises.Find(id);  // Veritabanından ilgili Exercise'ı bulun
            if (exercise == null)
            {
                return NotFound();  // Eğer Exercise bulunamazsa, NotFound sonucunu döndürün
            }

            var viewModel = new ReaderFast.webui.ViewModels.ExerciseViewModel
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Description = exercise.Description,
                IsPremium = exercise.IsPremium,
                IsCompleted= exercise.IsCompleted,
                ViewName = exercise.ViewName,
            };

            return View(viewModel);  // ViewModel'ı View'e geçirin
        }

        [HttpPost]
        public IActionResult Edit(ReaderFast.webui.ViewModels.ExerciseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);  // Eğer ModelState geçerli değilse, kullanıcıya formu tekrar gösterin
            }

            var exercise = _context.Exercises.Find(viewModel.Id);
            if (exercise == null)
            {
                return NotFound();  // Eğer Exercise bulunamazsa, NotFound sonucunu döndürün
            }

            exercise.Name = viewModel.Name;
            exercise.Description = viewModel.Description;
            exercise.IsPremium = viewModel.IsPremium;
            exercise.IsCompleted = viewModel.IsCompleted;
            exercise.ViewName = viewModel.ViewName;
            // ... diğer property güncellemeleri ...

            _context.Update(exercise);
            _context.SaveChanges();  // Değişiklikleri kaydedin

            return RedirectToAction("CreateExercise");  // Güncelleme işlemi tamamlandığında kullanıcıyı başka bir sayfaya yönlendirin
        }

        public async Task<IActionResult> DeleteExercise(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            var viewModel = new ExerciseViewModel
            {
                // Eğer ExerciseViewModel modeliniz Exercise modelinizdeki özelliklere sahipse,
                // bu özellikleri tek tek kopyalayabilirsiniz.
                Id = exercise.Id,
                Name = exercise.Name,
                Description = exercise.Description,
                IsPremium = exercise.IsPremium,
                IsCompleted = exercise.IsCompleted,
                ViewName = exercise.ViewName,

            };

            return View(viewModel); // Şimdi ExerciseViewModel örneğini geçiriyoruz.
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExerciseConfirmed(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult CreateExerciseDay()
        {
            var viewModel = new ReaderFast.webui.ViewModels.ExerciseDayViewModel
            {
                ExerciseDays = _context.ExerciseDays.ToList() ?? new List<ExerciseDay>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExerciseDay(ReaderFast.webui.ViewModels.ExerciseDayViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var exerciseDay = new ExerciseDay
                {
                    DayNumber = viewModel.DayNumber,
                    IsDayPremium = viewModel.IsDayPremium,
                    IsDayCompleted = viewModel.IsDayCompleted,
                };
                _context.Add(exerciseDay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateExerciseDay));
            }
            return View(viewModel);
        }

        public IActionResult EditExerciseDay(int id)
        {
            var exerciseDay = _context.ExerciseDays.Find(id);
            if (exerciseDay == null)
                return NotFound();

            var viewModel = new ReaderFast.webui.ViewModels.ExerciseDayViewModel
            {
                Id = exerciseDay.Id,
                DayNumber = exerciseDay.DayNumber,
                IsDayPremium = exerciseDay.IsDayPremium,
                IsDayCompleted = exerciseDay.IsDayCompleted,
                // Diğer alanlar...
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExerciseDay(ReaderFast.webui.ViewModels.ExerciseDayViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var exerciseDay = _context.ExerciseDays.Find(viewModel.Id);
            if (exerciseDay == null)
                return NotFound();

            exerciseDay.DayNumber = viewModel.DayNumber;
            exerciseDay.IsDayPremium = viewModel.IsDayPremium;
            exerciseDay.IsDayCompleted = viewModel.IsDayCompleted;
            // Diğer alan güncellemeleri...

            _context.Update(exerciseDay);
            await _context.SaveChangesAsync();

            return RedirectToAction("CreateExerciseDay"); // veya başka bir eylem.
        }

        public IActionResult DeleteExerciseDay(int id)
        {
            var exerciseDay = _context.ExerciseDays.Find(id);
            if (exerciseDay == null)
            {
                return NotFound();
            }

            var viewModel = new ViewModels.ExerciseDayViewModel
            {
                Id = exerciseDay.Id,
                DayNumber = exerciseDay.DayNumber,
                IsDayCompleted = exerciseDay.IsDayCompleted,
                IsDayPremium = exerciseDay.IsDayPremium,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExerciseDayConfirmed(int id)
        {
            var exerciseDay = await _context.ExerciseDays.FindAsync(id);
            _context.ExerciseDays.Remove(exerciseDay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ExerciseExerciseDay/Create
        public IActionResult CreateExerciseExerciseDay()
        {
            var viewModel = new ExerciseExerciseDayViewModel
            {
                  ExerciseExerciseDays = _context.ExerciseExerciseDays.Include(e => e.Exercise).Include(e => e.ExerciseDay).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExerciseExerciseDay(ExerciseExerciseDayViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var exerciseExerciseDay = new ExerciseExerciseDay
                {
                    ExerciseId = viewModel.ExerciseId,
                    ExerciseDayId = viewModel.ExerciseDayId,
                    SequenceNumber = viewModel.SequenceNumber
                };
                _context.Add(exerciseExerciseDay);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Exercise day successfully added.";

                // Burada listeyi tekrar doldurmak için yeni bir ViewModel örneği oluşturuyoruz.
                var newViewModel = new ExerciseExerciseDayViewModel
                {
                    ExerciseExerciseDays = _context.ExerciseExerciseDays.Include(e => e.Exercise).Include(e => e.ExerciseDay).ToList()
                };

                return View(newViewModel); // Bu yeni ViewModel'i view'a dönüyoruz.
            }
            // Model validasyon hataları varsa, aynı ViewModel ile formu tekrar göster
            viewModel.ExerciseExerciseDays = _context.ExerciseExerciseDays.Include(e => e.Exercise).Include(e => e.ExerciseDay).ToList();
            return View(viewModel);
        }



        // GET: ExerciseExerciseDay/Delete/5
        public async Task<IActionResult> DeleteExerciseExerciseDay(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exerciseExerciseDay = await _context.ExerciseExerciseDays
                .Include(e => e.Exercise)
                .Include(e => e.ExerciseDay)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exerciseExerciseDay == null)
            {
                return NotFound();
            }

            return View(exerciseExerciseDay);
        }

        // POST: ExerciseExerciseDay/Delete/5
        [HttpPost, ActionName("DeleteExerciseExerciseDay")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exerciseExerciseDay = await _context.ExerciseExerciseDays.FindAsync(id);
            _context.ExerciseExerciseDays.Remove(exerciseExerciseDay);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
