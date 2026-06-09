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

        #region === Constructor (recibe 1 parámetro, como Program.cs necesita) ===
        public LoginService(string filePath)
        {
            _filePath = filePath;

            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            EnsureLoginFileExists();
        }
        #endregion

        #region === Crear login.json si no existe ===
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

                var json = JsonSerializer.Serialize(defaultLogin, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
        }
        #endregion

        #region === Cargar login ===
        public Login LoadLogin()
        {
            if (!File.Exists(_filePath))
                return new Login();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Login>(json) ?? new Login();
        }
        #endregion

        #region === Validar credenciales ===
        public bool Validate(string user, string pass)
        {
            var login = LoadLogin();

            if (login.Username != user)
                return false;

            var salt = Convert.FromBase64String(login.Salt);
            var hash = HashPassword(pass, salt);

            return hash == login.PasswordHash;
        }
        #endregion

        #region === Hashing ===
        private byte[] GenerateSalt()
        {
            var salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

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
        #endregion
    }
}
