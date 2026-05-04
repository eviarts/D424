using C971App.Classes;
using Microsoft.Maui.Controls;
using SQLite;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel;

namespace C971App;

public partial class CoursePage : ContentPage
{
	private Courses _course;   

	public CoursePage(Courses selectedCourse)
	{
		InitializeComponent();

		_course = selectedCourse;
		BindingContext = _course;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        var aList = await App.db.GetAssessemtnsPerCourse(_course.CourseId);

        _course.OA.Clear();
        _course.PA.Clear();

        foreach (var a in aList)
        {
            if (a.AsmntType == "Objective")
                _course.OA.Add(a);
            else if (a.AsmntType == "Performance")
                _course.PA.Add(a);

            //await AssessmentStartNotif(a);
            //await AssessmentEndNotif(a);
        }
	}

    //private async Task AssessmentStartNotif(Assessments assessment)
    //{
    //    var startDate = DateTime.Today.AddDays(1);

    //    if (assessment.StartDate.Date == startDate && assessment.StartNotif)
    //    {
    //        var notif = new NotificationRequest
    //        {
    //            NotificationId = assessment.AsmntId + 30,
    //            Title = "Assessment Starts Soon",
    //            Description = $"{assessment.AsmntName} starts tomorrow",
    //            Schedule = new NotificationRequestSchedule
    //            {
    //                NotifyTime = DateTime.Now.AddSeconds(5)
    //            }
    //        };
    //        assessment.StartNotif = true;
    //        await LocalNotificationCenter.Current.Show(notif);
    //    }
    //}

    //private async Task AssessmentEndNotif(Assessments assessment)
    //{
    //    var endDate = DateTime.Today.AddDays(1);

    //    if (assessment.EndDate.Date == endDate && assessment.EndNotif)
    //    {
    //        var notif = new NotificationRequest
    //        {
    //            NotificationId = assessment.AsmntId + 40,
    //            Title = "Assessment Is Due Soon",
    //            Description = $"{assessment.AsmntName} is due tomorrow",
    //            Schedule = new NotificationRequestSchedule
    //            {
    //                NotifyTime = DateTime.Now.AddSeconds(5)
    //            }
    //        };
    //        assessment.EndNotif = true;
    //        await LocalNotificationCenter.Current.Show(notif);
    //    }
    //}

    bool isEditSaveMode = false;

    private async void EditSaveButton_Clicked(object? sender, EventArgs e)
	{
        try
        {
            if (sender is not ToolbarItem tbi)
                return;

            isEditSaveMode = !isEditSaveMode;
            _course.ShowDelete = isEditSaveMode;

            foreach (var oa in _course.OA)
            {
                oa.ShowDelete = isEditSaveMode;
            }

            foreach (var pa in _course.PA)
            {
                pa.ShowDelete = isEditSaveMode;
            }

            if (isEditSaveMode)
            {
                tbi.Text = "Save";
                tbi.IconImageSource = "save.png";
            }
            else
            {
                await App.db.SaveCourseAsync(_course);  
                
                foreach (var oa in _course.OA)
                {
                    oa.CourseId = _course.CourseId;
                    await App.db.SaveAssessmentAsync(oa);
                }

                foreach (var pa in _course.PA)
                {
                    pa.CourseId = _course.CourseId;
                    await App.db.SaveAssessmentAsync(pa);
                }

                _course.RefreshAll();

                tbi.Text = "Edit";
                tbi.IconImageSource = "edit_icon.png";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
	}

    private async void ShowAddAssessment_Clicked(object sender, EventArgs e)
    {
        AddAssessment.IsVisible = true;
    }

    private async void CancelAddAssessment_Clicked(object sender, EventArgs e)
    {
        AddAssessment.IsVisible = false;
    }

    private async void AddAssessment_Clicked(object sender, EventArgs e)
    {
        string asmntName = NewAssessmentName.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(asmntName))
        {
            await DisplayAlert("Error", "Please enter assessment name", "Ok");
            return;
        }

        if (NewAssessmentStart.Date > NewAssessmentEnd.Date)
        {
            await DisplayAlert("Error", "Start date cannot be later than end date", "Ok");
            return;
        }

        if (AsmntType.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please select assessment type", "Ok");
            return;
        }

        var typeLimit = AsmntType.SelectedItem?.ToString() ?? "Objective";

        if (typeLimit == "Objective" && _course.OA.Count >= 1)
        {
            await DisplayAlert("Error", "Objective assessment limit reached, only one assessment per type", "Ok");
            return;
        }

        if (typeLimit == "Performance" && _course.PA.Count >= 1)
        {
            await DisplayAlert("Error", "Performance assessment limit reached, only one assessment per type", "Ok");
            return;
        }

        var newAssessment = new Assessments
        {
            CourseId = _course.CourseId,
            AsmntName = asmntName,
            StartDate = NewAssessmentStart?.Date ?? DateTime.Today,
            EndDate = NewAssessmentEnd?.Date ?? DateTime.Today,
            AsmntType = AsmntType.SelectedItem?.ToString() ?? "Objective",
            ShowDelete = isEditSaveMode
        };
        await App.db.SaveAssessmentAsync(newAssessment);

        if (newAssessment.AsmntType == "Objective")
            _course.OA.Add(newAssessment);
        else
            _course.PA.Add(newAssessment);

        NewAssessmentName.Text = "";
        NewAssessmentStart.Date = DateTime.Today;
        NewAssessmentEnd.Date = DateTime.Today;

        AddAssessment.IsVisible = false;
    }

    private async void DeleteAssessment(object sender, EventArgs e)
    {
        if (sender is not ImageButton b || b.BindingContext is not Assessments assessment)
            return;

        bool confirm = await DisplayAlert("Delete Assessment", $"Are you sure you want to delete {assessment.AsmntName}?", "Yes", "No");
        if (!confirm) return;

        await App.db.DeleteAssessment(assessment);   
        
        if (_course != null)
        {
            if (_course.OA.Contains(assessment))
                _course.OA.Remove(assessment);
            else if (_course.PA.Contains(assessment))
                _course.PA.Remove(assessment);
        }
    }

    private async void ShareButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_course.CourseNotes))
        {
            await DisplayAlert("Error", "Course notes are empty, nothing to share", "Ok");
            return;
        }
        await Share.RequestAsync(new ShareTextRequest { Text = _course.CourseNotes, Title = "Course Notes" });
    }
}