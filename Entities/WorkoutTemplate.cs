using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Entities
{
    public class WorkoutTemplate
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<WorkoutTemplateExercise> Exercises { get; set; } = new();
    }
}
