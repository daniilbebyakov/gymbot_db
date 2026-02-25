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
        public async Task<Workout> AddWorkout(long tgId, string wName)
        {
            var user= await _context.Users.FirstOrDefaultAsync(u=>u.TgId == tgId) 
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
        public sealed record WorkoutExerciseDto(string Name, decimal Weight, int Reps, int Sets);
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
        }
    }

