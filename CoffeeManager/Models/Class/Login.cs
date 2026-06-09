namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents the login credentials stored locally.
    /// </summary>
    public class Login
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
    }
}
