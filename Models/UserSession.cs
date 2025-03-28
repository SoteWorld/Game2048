namespace Game2048.Models
{
    // Класс для хранения текущего профиля пользователя после авторизации
    public static class UserSession
    {
        // Текущий авторизованный пользователь
        public static UserProfile CurrentUser { get; set; }
    }
}
