namespace ReaderFast.webui.Models
{
    public class ExerciseExerciseDay
    {

        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public int ExerciseDayId { get; set; }
        public ExerciseDay ExerciseDay { get; set; }

        public int SequenceNumber { get; set; } // Egzersiz sıralama numarası
    }
}