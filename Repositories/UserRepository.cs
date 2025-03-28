using MongoDB.Driver;
using Game2048.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Game2048.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<UserProfile> _users;

        public UserRepository()
        {
            var connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("Game2048DB");
            _users = database.GetCollection<UserProfile>("Users");
        }

        // Получение пользователя по имени
        public UserProfile GetUserByUsername(string username)
        {
            return _users.Find(u => u.Username == username).FirstOrDefault();
        }

        // Создание нового пользователя
        public void CreateUser(UserProfile user)
        {
            _users.InsertOne(user);
        }

        // Инкрементальное обновление статистики пользователя (как обсуждалось ранее)
        public void IncrementUserStats(string userId, int score, long gameDurationInSeconds)
        {
            var filter = Builders<UserProfile>.Filter.Eq(u => u.Id, userId);
            var update = Builders<UserProfile>.Update
                .Inc(u => u.GamesPlayed, 1)
                .Inc(u => u.TotalPlayTimeInSeconds, gameDurationInSeconds)
                .Max(u => u.MaxScore, score);
            _users.UpdateOne(filter, update);
        }

        // Новый метод для получения всех пользователей
        public ObservableCollection<UserProfile> GetAllUsers()
        {
            var users = _users.Find(u => true).ToList();
            return new ObservableCollection<UserProfile>(users);
        }
    }
}
