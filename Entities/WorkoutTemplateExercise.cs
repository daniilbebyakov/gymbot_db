using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Entities
{
    public class WorkoutTemplateExercise
    {
        public long Id { get; set; }
        public long WorkoutTemplateId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public WorkoutTemplate? WorkoutTemplate { get; set; }
    }
}
