namespace GymBot.Data.Entities
{
    public class Workout
    {
        public long Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public DateOnly Date { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
