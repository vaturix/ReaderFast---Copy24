using Microsoft.AspNetCore.Identity;
using ReaderFast.webui.Areas.Identity.Data;

namespace ReaderFast.webui.Models
{
    public class UserExercise
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int ExerciseExerciseDayId { get; set; }
        public ExerciseExerciseDay ExerciseExerciseDay { get; set; }

        public bool IsCompleted { get; set; }
    }
}
