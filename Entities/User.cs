namespace GymBot.Data.Entities
{
    public class User
    {
        public long Id { get; set; }
        public long TgId { get; set; }
        public string Username { get; set; } = string.Empty;
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
        public ICollection<UserBodyMetric> BodyMetrics { get; set; } = new List<UserBodyMetric>();
    }
}
