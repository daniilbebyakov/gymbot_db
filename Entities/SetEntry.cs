using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Entities
{
    public class SetEntry
    {
        public long Id { get; set; }
        public long ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;
        public int Reps { get; set; }
        public decimal Weight { get; set; }
        public DateOnly Date { get; set; }
    }
}
