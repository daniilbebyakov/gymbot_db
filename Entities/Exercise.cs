namespace GymBot.Data.Entities
{
    public class Exercise
    {
        public long Id { get; set; }
        public long WorkoutId { get; set; }
        public Workout Workout { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateOnly Date { get; set; }
        public ICollection<SetEntry> Sets { get; set; } = new List<SetEntry>();
    }
}
