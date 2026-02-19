using GymBot.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymBot.Data.Data.Repositories
{
    public class WorkoutRepository
    {
        private readonly GymBotContext _context;
        public WorkoutRepository(GymBotContext context)
        {
            _context = context;
        }
        public async Task<Workout> AddWorkout(long tgid, string wName)
        {
            var user= await _context.Users.FirstOrDefaultAsync(u=>u.TgId == tgid) 
                ?? throw new InvalidOperationException("Пользователь не зарегистрирован");
            var workout = new Workout
            {
                Name = wName,
                UserId = user.Id,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return workout;
        }
    }
}
