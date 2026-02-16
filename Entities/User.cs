/// Забыл написать 
/// Обычно неймспейс описывает иерархию проекта
/// ИмяПроекта.Папка.Папка....
namespace GymBot.Data.Entities
{
	public class User
	{
		/// <summary>
		/// А нам хватит int?
		/// Может быть лучше будет uint или еще что-то 
		/// Я просто не делал ботов не могу точно сказать
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Используй понятные имена. Не сокращай (Только если везде так)
		/// Либо пиши комментарии на подобие этого 
		/// просто пропиши 3 слеша и он выдаст обычный summary
		/// </summary>
		public long TgId { get; set; } // Тоже самое как про int, но тут почему-то long

		/// <summary>
		/// Нам важно? Пользователь может сменить имя в любой момент. 
		/// </summary>
		public string Username { get; set; } = "";
	}
	/// По поводу комментов - Кто-то ЗА кто-то ПРОТИВ
	/// В идеале "Твой код не требует комментариев" те все понятно и без них
	/// Посмотри тут "https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/language-specification/documentation-comments"

}
