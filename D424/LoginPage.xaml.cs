namespace D424;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void LoginButton(object sender, EventArgs e)
	{
        await LoginLogic();
	}

	private async Task LoginLogic()
	{
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (username == null || password == null)
        {
            await DisplayAlert("Error", "Please enter a username and password", "Ok");
            return;
        }

        if (username == "test" && password == "test")
        {
            await SecureStorage.SetAsync("auth_token", "logged_in");
            App.Current.MainPage = new AppShell();
        }
        else
        {
            ErrorLabel.Text = "Incorrect username or password";

            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;

            UsernameEntry.Focus();
        }
    }
}