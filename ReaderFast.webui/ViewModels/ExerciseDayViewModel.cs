using ReaderFast.webui.Models;

namespace ReaderFast.webui.ViewModels
{
    public class ExerciseDayViewModel
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public bool IsDayPremium { get; set; }
        public bool IsDayCompleted { get; set; }
        public IEnumerable<ExerciseDay> ExerciseDays { get; set; } = new List<ExerciseDay>();


    }
}
