using System.Collections.Generic;
using ReaderFast.webui.Models;

namespace ReaderFast.webui.ViewModels
{
    public class ExerciseExerciseDayViewModel
    {
        public int ExerciseId { get; set; }
        public int ExerciseDayId { get; set; }
        public int SequenceNumber { get; set; }
        public IEnumerable<ExerciseExerciseDay> ExerciseExerciseDays { get; set; }

        // Constructor to initialize ExerciseExerciseDays
        public ExerciseExerciseDayViewModel()
        {
            ExerciseExerciseDays = new List<ExerciseExerciseDay>();
        }
    }
}
