using CoffeeManager.Models;
using CoffeeManager.Models.Class;
using System;
using System.IO;
using System.Text.Json;

namespace CoffeeManager.Services.Logic
{
    /// <summary>
    /// Servicio encargado de cargar y validar credenciales desde login.json.
    /// </summary>
    public class LoginService
    {
        private readonly string _filePath;

        public LoginService(string filePath)
        {
            _filePath = filePath;
            EnsureLoginFileExists();
        }

        /// <summary>
        /// Crea el archivo login.json si no existe.
        /// </summary>
        private void EnsureLoginFileExists()
        {
            if (!File.Exists(_filePath))
            {
                var defaultLogin = new Login
                {
                    Username = "admin",
                    PasswordHash = "1234",
                    Salt = "salt"
                };

                var json = JsonSerializer.Serialize(defaultLogin, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
        }

        /// <summary>
        /// Carga las credenciales desde login.json.
        /// </summary>
        public Login? LoadLogin()
        {
            if (!File.Exists(_filePath))
                return null;

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<Login>(json);
        }

        /// <summary>
        /// Valida usuario y contraseña.
        /// </summary>
        public bool Validate(string user, string pass)
        {
            var login = LoadLogin();
            if (login == null) return false;

            return login.Username == user && login.PasswordHash == pass;
        }
    }
}
