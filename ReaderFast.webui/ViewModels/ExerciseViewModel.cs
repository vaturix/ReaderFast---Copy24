using ReaderFast.webui.Models;

namespace ReaderFast.webui.ViewModels
{
    public class ExerciseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPremium { get; set; }
        public bool IsCompleted { get; set; }
        public string ViewName { get; set; }
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

    }
}
