using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Entities
{
    public class UserBodyMetric
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public decimal Weight { get; set; }
        public DateOnly Date { get; set; }

    }
}
