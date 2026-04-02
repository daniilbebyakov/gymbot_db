using GymBot.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Data.Repositories
{

    public class TemplateRepository(GymBotContext context)
    {
        private readonly GymBotContext _context = context;
        public async Task<long> CreateTemplate(long userId, string name, List<string> exercises)
        {
            var template = new WorkoutTemplate
            {
                UserId = userId,
                Name = name.Trim(),
            };
            _context.WorkoutTemplates.Add(template);
            await _context.SaveChangesAsync();
            var normalized = exercises.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList()
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
            var rows = normalized.Select((name) => new WorkoutTemplateExercise
            {
                WorkoutTemplateId = template.Id,
                ExerciseName = name,
            });
            _context.WorkoutTemplateExercises.AddRange(rows);
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
    }
}
