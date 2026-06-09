namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents the login credentials stored locally.
    /// </summary>
    public class Login
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
    }
}
