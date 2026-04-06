namespace C971App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CoursePage), typeof(CoursePage));
        }
    }
}
