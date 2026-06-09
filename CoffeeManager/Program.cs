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

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "CoffeeManager");
            Directory.CreateDirectory(folder);

            string loginPath = Path.Combine(folder, "login.json");

            var loginService = new LoginService(loginPath);

            Application.Run(new FormLogin(loginService));
        }
    }
}
