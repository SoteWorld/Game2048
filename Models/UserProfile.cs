using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game2048.Models
{
    // Модель профиля пользователя, которая будет храниться в MongoDB
    public class UserProfile
    {
        // Идентификатор в базе данных (MongoDB автоматически генерирует ObjectId)
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Имя пользователя (уникальное)
        public string Username { get; set; }

        // Хранит хэш пароля (для простоты, сейчас просто текст, но в реальном приложении обязательно используйте хэширование)
        public string PasswordHash { get; set; }

        // Статистика пользователя
        public int GamesPlayed { get; set; }
        public int MaxScore { get; set; }
        public long TotalPlayTimeInSeconds { get; set; }

        // Метод для проверки пароля (замените на реальное хэширование в продакшене)
        public bool VerifyPassword(string password)
        {
            return PasswordHash == password;
        }
    }
}
