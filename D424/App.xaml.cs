using Microsoft.Extensions.DependencyInjection;
using SQLite;
using C971App.Classes;

namespace C971App
{
    public partial class App : Application
    {
        public static Database db { get; private set; }

        public App()
        {
            InitializeComponent();

            db = new Database();

            CheckLoginStatus();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private async void CheckLoginStatus()
        {
            var token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
            {
                MainPage = new LoginPage();
            }
            else
            {
                MainPage = new AppShell();
            }
        }
    }
}