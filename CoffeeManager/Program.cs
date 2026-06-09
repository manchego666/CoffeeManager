using CoffeeManager.Front;
using CoffeeManager.Services.Logic;

namespace CoffeeManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Crear LoginService con la ruta del login.json
            var loginService = new LoginService(Path.Combine(AppContext.BaseDirectory, "login.json"));

            // Abrir primero el login
            Application.Run(new FormLogin(loginService));
        }
    }
}
