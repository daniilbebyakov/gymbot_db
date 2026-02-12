using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Xml.Linq;
namespace GymBot.Data
{
    public class GymBotContext : DbContext
    {
        public DbSet<Entities.User> Users => Set<Entities.User>();
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=localhost;Port=5432;Username=gymbotuser;Password=gymbotpass;Database=gymbot_db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.User>().HasIndex(u => u.TgId).IsUnique();
        }
    }
    public class UserRepository
    {
        private readonly GymBotContext _context;
        public UserRepository(GymBotContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserIfNotExist(long tgId, string username)
        {
            bool exists = await _context.Users.AnyAsync(u => u.TgId == tgId);
            if (exists) return false;
            try
            {
                _context.Users.Add(new Entities.User
                {
                    TgId = tgId,
                    Username = username ?? ""
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
            return true;
        }
    }
}
