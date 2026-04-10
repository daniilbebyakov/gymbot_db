using GymBot.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymBot.Data.Data.Repositories
{
    public class WorkoutRepository(GymBotContext context)
    {
        private readonly GymBotContext _context = context;

        public sealed record WorkoutExerciseDto(string Name, decimal Weight, int Reps, int Sets);
        public sealed record WorkoutHistoryItemDto(long Id, string Name, DateOnly Date, int ExercisesCount);
        public async Task<long> SaveWorkout(long tgId, DateOnly workoutDate, string workoutTemplate,
            IReadOnlyCollection<WorkoutExerciseDto> exercises)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TgId == tgId)
                ?? throw new InvalidOperationException("Пользователь не зарегистрирован");
            var workout = new Workout
            {
                Name = workoutTemplate.Trim(),
                UserId = user.Id,
                Date = workoutDate,
            };
            foreach (var exerciseDto in exercises)
            {
                var exercise = new Exercise
                {
                    Name = exerciseDto.Name.Trim(),
                    Date = workoutDate,
                };

                for (var setNumber = 0; setNumber < exerciseDto.Sets; setNumber++)
                {
                    exercise.Sets.Add(new SetEntry
                    {
                        Reps = exerciseDto.Reps,
                        Weight = exerciseDto.Weight,
                        Date = workoutDate,
                    });
                }
                workout.Exercises.Add(exercise);
            }
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
            return workout.Id;
        }
        public async Task<IReadOnlyCollection<WorkoutHistoryItemDto>> GetWorkoutHistory(long tgId, int page, int pageSize = 5)
        {
            return await _context.Workouts
                .AsNoTracking()
                .Where(w => w.User.TgId == tgId)
                .OrderByDescending(w => w.Date)
                .ThenByDescending(w => w.Id)
                .Skip(page * pageSize)
                .Take(pageSize)
                .Select(w => new WorkoutHistoryItemDto(
                    w.Id,
                    w.Name,
                    w.Date,
                    w.Exercises.Count))
                .ToListAsync();
        }
        public async Task<int> GetWorkoutHistoryCount(long tgId)
        {
            return await _context.Workouts.CountAsync(w => w.User.TgId == tgId);
        }
        public async Task<bool> DeleteWorkout (long tgId, long workoutId)
        {
            var workout=await _context.Workouts.Include(w=>w.User)
                .FirstOrDefaultAsync(w=>w.Id == workoutId && w.User.TgId==tgId);
            if (workout is null)
            {
                return false;
            }
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

