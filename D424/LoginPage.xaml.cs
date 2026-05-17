using D424.Classes;

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

    private void ShowForm(object sender, EventArgs e)
    {
        CreateAccountBorder.IsVisible = true;
    }

    private void CancelCreateForm(object sender, EventArgs e)
    {
        CreateAccountBorder.IsVisible = false;
    }

    private async void CreateAccountTapped(object sender, EventArgs e)
    {
        Database db = new Database();

        string username = CreateUsername.Text;
        string password = CreatePassword.Text;
        string confirm = ConfirmPassword.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please fill all fields", "Ok");
            return;
        }

        if (password != confirm)
        {
            await DisplayAlert("Error", "Passwords do not match", "Ok");
            return;
        }

        bool success = await db.CreateAccount(username, password);

        if (!success)
        {
            await DisplayAlert("Error", "Username already exists", "Ok");
            return;
        }

        await DisplayAlert("Success", "Account created", "Ok");

        CreateAccountBorder.IsVisible = false;

        CreateUsername.Text = string.Empty;
        CreatePassword.Text = string.Empty;
        ConfirmPassword.Text = string.Empty;
    }
    

	private async Task LoginLogic()
	{
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter a username and password", "Ok");
            return;
        }

        Database db = new Database();

        bool loginSuccess = await db.Login(username, password);

        if (loginSuccess)
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