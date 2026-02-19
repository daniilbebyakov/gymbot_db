using GymBot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Data.Repositories
{
    public class ExerciseRepository
    {
        private readonly GymBotContext _context;

        public ExerciseRepository(GymBotContext context)
        {
            _context = context;
        }
        //public async Task<Exercise> AddExercise(long tgid, long workoutid, string exercisename, int )
        //{
        //    var workout = await _context.Workouts.Where(w => w.Id == workoutid).Where(w => w.User.TgId == tgid)
        //        .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Тренировка не найдена или не принадлежит пользователю");
        //};
    }
}
