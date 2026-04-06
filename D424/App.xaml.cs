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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}