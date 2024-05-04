namespace ReaderFast.webui.Models
{
    public class ExerciseDay
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public bool IsDayCompleted { get; set; }
        public bool IsDayPremium { get; set; }

        // ExerciseDay ile ExerciseExerciseDay arasındaki ilişki
        public ICollection<ExerciseExerciseDay> ExerciseExerciseDays { get; set; }

    }
}
