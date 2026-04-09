using C971App.Classes;
using SQLite;
namespace C971App;

public partial class SearchReport : ContentPage
{
	Database _database;

	public SearchReport()
	{
		InitializeComponent();	
        
        _database = new Database();
    }

	private async void SearchTextChanged(object sender, TextChangedEventArgs e)
	{
		try
		{
            string query = e.NewTextValue;

            if (string.IsNullOrWhiteSpace(query))
            {
                SearchResults.ItemsSource = null;
                return;
            }

            var results = await _database.Search(query);
            SearchResults.ItemsSource = results;
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("Error: ", ex.Message, "OK");
        }
	}
}