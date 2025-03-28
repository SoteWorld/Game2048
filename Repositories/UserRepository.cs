using MongoDB.Driver;
using Game2048.Models;

namespace Game2048.Repositories
{
    // Репозиторий для работы с коллекцией пользователей в MongoDB
    public class UserRepository
    {
        private readonly IMongoCollection<UserProfile> _users;

        public UserRepository()
        {
            // Укажите вашу строку подключения к MongoDB (например, локальный сервер)
            var connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("Game2048DB");
            _users = database.GetCollection<UserProfile>("Users");
        }

        // Поиск пользователя по имени
        public UserProfile GetUserByUsername(string username)
        {
            return _users.Find(u => u.Username == username).FirstOrDefault();
        }

        // Создание нового пользователя
        public void CreateUser(UserProfile user)
        {
            _users.InsertOne(user);
        }
    }
}
