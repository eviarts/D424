namespace C971App;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void LoginButton(object sender, EventArgs e)
	{
		var username = UsernameEntry.Text;
		var password = PasswordEntry.Text;

		if (username == null || password == null)
		{
			await DisplayAlertAsync("Error", "Please enter a username and password", "Ok");
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