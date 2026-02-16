using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Xml.Linq;
namespace GymBot.Data
{
    /// <summary>
    /// Лучше не хранить два класса в одном файле
    /// 1 Класс : 1 Файл (Бывают исключения но это обсуждается локально)
    /// </summary>
    public class GymBotContext : DbContext
    {
        public DbSet<Entities.User> Users => Set<Entities.User>();
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
			/// В идеале подобную инфу вынести в некоторый конфиг файл
			/// Чтобы например
			/// При запуске проекта можно было без изменения кода выбрать другие данные к БД
			/// <see cref="https://metanit.com/sharp/efcore/1.5.php"/>
			options.UseNpgsql("Host=localhost;Port=5432;Username=gymbotuser;Password=gymbotpass;Database=gymbot_db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.User>().HasIndex(u => u.TgId).IsUnique();
        }
    }
    public class UserRepository
    {
        /// <summary>
        /// Мб свойство добавить? 
        /// </summary>
        private readonly GymBotContext _context;

        /// <summary>
        /// И тип ток с ним общаться.
        /// </summary>
        /// public GymBotContext Context => _context;

        public UserRepository(GymBotContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUserIfNotExist(long tgId, string username)
        {
            bool exists = await _context.Users.AnyAsync(u => u.TgId == tgId);
            /// Посмотри как описывается код в документации C#
			/// https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/statements/selection-statements
			/// Кнч все зависит от стандартов компании но скорее всего все будет основываться на деф документации Майков
			if (exists) 
            { 
                return false;
            }
            try
            {
                _context.Users.Add(new Entities.User
                {
                    TgId = tgId,
                    Username = username ?? "" // Тут на любителя. Мне больше нрав String.Empty
                });
                await _context.SaveChangesAsync();
            }
            /// !!! В идеале мы должны понимать какое исключение может прийти
            /// И ловить и обрабаывать так как нужно
            /// Например, создадим кастомное исключение связанное с тем что пользователь уже сущ в базе бота
            /// catch (UserAlreadyExist ex) {
            /// bla-bla-bla;
            /// }
            /// 
            /// Иногда приходится ловить все искл но лучше этого избегать
            catch (Exception ex) 
            {
				/// Ок, но можно добавить описания https://learn.microsoft.com/ru-ru/dotnet/api/system.string.format?view=net-10.0
				/// Там оч много всяких приколюх вот есть на метаните https://metanit.com/sharp/tutorial/7.5.php
				Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
            /// Я любил добавлять пустую строку перед return if  и тп
            /// Так будто по читаемее ставновится 
            return true;
        }
    }
}
