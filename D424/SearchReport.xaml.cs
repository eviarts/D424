using D424.Classes;
using SQLite;
namespace D424;

public partial class SearchReport : ContentPage
{
	Database _database;

	public SearchReport()
	{
		InitializeComponent();	
        
        _database = new Database();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var courses = await _database.GetAllCourses();

        var names = courses
            .Select(c => c.InstructorName)
            .Distinct()
            .OrderBy(n => n);

        InstructorPicker.Items.Clear();

        foreach (var name in names)
        {
            InstructorPicker.Items.Add(name);
        }
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
            await DisplayAlert("Error: ", ex.Message, "OK");
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

    private async void ShowInstructorReport(object sender, EventArgs e)
    {
        InstructorReport.IsVisible = !InstructorReport.IsVisible;

        if (InstructorReport.IsVisible)
        {
            InstructorHeader.Text = "^ Courses by Instructor";
        }
        else
        {
            InstructorHeader.Text = "> Courses by Instructor";
        }
    }

    private async void GenerateDateReport(object sender, EventArgs e)
    {
        CoursesByDate();
    }

    private async void GenerateInstructorReport(object sender, EventArgs e)
    {
        CoursesByInstructor();
    }

    private async void CoursesByDate()
    {
        var start = StartDate?.Date ?? DateTime.Today;
        var end = EndDate?.Date ?? DateTime.Today;

        if (end < start)
        {
            await DisplayAlert("Incorrect Date Range", "End date must be after start date", "Ok");
            return;
        }

        var courses = await _database.CoursesByDate(start, end);

        if (courses == null || courses.Count == 0)
        {
            DateReportResults.ItemsSource = null;
            await DisplayAlert("No Results", "No courses were found in that date range", "Ok");
            return;
        }

        DateReportResults.ItemsSource = courses;
    }

    private async void CoursesByInstructor()
    {
        var name = InstructorPicker.SelectedItem as string;

        if (string.IsNullOrEmpty(name))
        {
            InstructorResults.ItemsSource = null;
            await DisplayAlert("Select Instructor", "Please select an instructor to generate a report", "Ok");
            return;
        }

        var courses = await _database.CoursesByInstructor(name);
        InstructorResults.ItemsSource = courses;
    }
}