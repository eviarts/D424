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

    private async void ShowDateReport(object sender, EventArgs e)
    {
        DateRangeReport.IsVisible = !DateRangeReport.IsVisible;

        if (DateRangeReport.IsVisible)
        {
            DateRangeHeader.Text = "^ Courses by Date Range";
        }
        else
        {
            DateRangeHeader.Text = "> Courses by Date Range";
        }
    }

    private async void GenerateDateReport(object sender, EventArgs e)
    {
        CoursesByDate();
    }

    private async void CoursesByDate()
    {
        var start = StartDate.Date ?? DateTime.Today;
        var end = EndDate.Date ?? DateTime.Today;

        if (end < start)
        {
            DisplayAlertAsync("Incorrect Date Range", "End date must be after start date", "Ok");
            return;
        }

        var courses = await _database.CoursesByDate(start, end);

        if (courses == null || courses.Count == 0)
        {
            DateReportResults.ItemsSource = null;
            DisplayAlertAsync("No Results", "No courses were found in that date range", "Ok");
            return;
        }

        DateReportResults.ItemsSource = courses;
    }
}