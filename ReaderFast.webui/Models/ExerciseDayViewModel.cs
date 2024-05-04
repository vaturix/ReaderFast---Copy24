namespace ReaderFast.webui.Models
{
    public class ExerciseDayViewModel
    {
        public ExerciseDay ExerciseDay { get; set; }
        public List<bool> IsExerciseCompleted { get; set; }

        public int DayNumber { get; set; } // Eğer ExerciseDay içinde ise bu satırı kaldırabilirsiniz
        public int Id { get; set; } // Eğer ExerciseDay içinde ise bu satırı kaldırabilirsiniz
        public List<ExerciseViewModel> Exercises { get; set; }
    }
}
