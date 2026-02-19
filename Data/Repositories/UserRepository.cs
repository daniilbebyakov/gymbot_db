using GymBot.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymBot.Data.Data.Repositories
{
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
                _context.Users.Add(new User
                {
                    TgId = tgId,
                    Username = username ?? string.Empty,
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
