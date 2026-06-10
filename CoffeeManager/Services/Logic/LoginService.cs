using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Services.Logic
{
    public class LoginService
    {
        private readonly string _filePath;

        // ✔ Constructor vacío (para usar new LoginService())
        public LoginService() : this(PathService.Login)
        {
        }

        // ✔ Constructor real
        public LoginService(string filePath)
        {
            _filePath = filePath;

            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            EnsureLoginFileExists();
        }

        // ✔ Crear login.json si no existe
        private void EnsureLoginFileExists()
        {
            if (!File.Exists(_filePath))
            {
                var salt = GenerateSalt();
                var hash = HashPassword("1234", salt);

                var defaultLogin = new Login
                {
                    Username = "admin",
                    PasswordHash = hash,
                    Salt = Convert.ToBase64String(salt)
                };

                SaveLogin(defaultLogin);
            }
        }

        // ✔ Cargar login
        public Login LoadLogin()
        {
            if (!File.Exists(_filePath))
                return new Login();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Login>(json) ?? new Login();
        }

        // ✔ Guardar login
        private void SaveLogin(Login login)
        {
            var json = JsonSerializer.Serialize(login, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        // ✔ Validar usuario + contraseña
        public bool Validate(string user, string pass)
        {
            var login = LoadLogin();

            if (!string.Equals(login.Username, user, StringComparison.OrdinalIgnoreCase))
                return false;

            var salt = Convert.FromBase64String(login.Salt);
            var hash = HashPassword(pass, salt);

            return hash == login.PasswordHash;
        }

        // ✔ Validar SOLO contraseña actual
        public bool ValidatePassword(string password)
        {
            var login = LoadLogin();
            var salt = Convert.FromBase64String(login.Salt);
            var hash = HashPassword(password, salt);

            return hash == login.PasswordHash;
        }

        // ✔ Cambiar contraseña
        public void ChangePassword(string newPassword)
        {
            var login = LoadLogin();

            var salt = GenerateSalt();
            login.Salt = Convert.ToBase64String(salt);
            login.PasswordHash = HashPassword(newPassword, salt);

            SaveLogin(login);
        }

        // ✔ Generar salt
        private byte[] GenerateSalt()
        {
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        // ✔ Hash SHA256 + salt
        private string HashPassword(string password, byte[] salt)
        {
            using var sha = SHA256.Create();

            var passBytes = Encoding.UTF8.GetBytes(password);
            var combined = new byte[passBytes.Length + salt.Length];

            Buffer.BlockCopy(passBytes, 0, combined, 0, passBytes.Length);
            Buffer.BlockCopy(salt, 0, combined, passBytes.Length, salt.Length);

            var hash = sha.ComputeHash(combined);
            return Convert.ToBase64String(hash);
        }
    }
}
