namespace ReaderFast.webui.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPremium { get; set; }
        public string ViewName { get; set; }
        public bool IsCompleted { get; set; }

        // Exercise ile ExerciseExerciseDay arasındaki ilişki
        public ICollection<ExerciseExerciseDay> ExerciseExerciseDays { get; set; }

    }
}
