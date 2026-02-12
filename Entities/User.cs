using System;
using System.Collections.Generic;
using System.Text;

namespace GymBot.Data.Entities
{
    public class User
    {
            public int Id { get; set; }
            public long TgId { get; set; }
            public string Username { get; set; } = "";
    }
}
