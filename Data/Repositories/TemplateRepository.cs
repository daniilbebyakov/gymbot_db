using GymBot.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Data.Repositories
{

    public class TemplateRepository
    {
        private readonly GymBotContext _context;
        public TemplateRepository(GymBotContext context)
        {
            _context = context;
        }
        public async Task<long> CreateTemplate(long userId,string name,List<TemplateExerciseDto> exercises)
        {
            var template = new WorkoutTemplate
            {
                UserId = userId,
                Name = name.Trim(),
                Exercises = exercises.Select(x => new WorkoutTemplateExercise
                {
                    ExerciseName = x.ExerciseName,
                }).ToList()
            };
            _context.WorkoutTemplates.Add(template);
            await _context.SaveChangesAsync();
            return template.Id;
        }
        public async Task<List<WorkoutTemplate>> GetUserTemplates(long userId)
        {
            return await _context.WorkoutTemplates
                .Where(x => x.UserId == userId)
                .Include(x => x.Exercises)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        public async Task<WorkoutTemplate?> GetTemplate(long templateId, long userId)
        {
            return await _context.WorkoutTemplates
                .Include(x => x.Exercises)
                .FirstOrDefaultAsync(x => x.Id == templateId && x.UserId == userId);
        }
        public record TemplateExerciseDto(string ExerciseName);
    }
}
