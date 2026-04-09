using C971App.Classes;
using System.Collections.ObjectModel;
using SQLite;
using Plugin.LocalNotification;

namespace C971App
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Terms> Terms { get; set; } = new();
        public Terms? SelectedTerm { get; set; }
        public ObservableCollection<Courses> Courses { get; set; }        

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            NotifPermission();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var terms = await App.db.GetTermsAsync();
            if (terms.Count == 0)
            {
                await Database.InsertTestData();
                terms = await App.db.GetTermsAsync();
            }

            Terms.Clear();

            foreach (var term in terms)
            {
                Terms.Add(term);
            }

            var courseList = await App.db.GetAllCourses();

            foreach (var courses in courseList)
            {
                await CourseStartNotif(courses);
                await CourseEndNotif(courses);
            }
        }

        private async void NotifPermission()
        {
            if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
        }

        private async Task CourseStartNotif(Courses course)
        {
            var startDate = DateTime.Today.AddDays(1);

            if (course.StartDate.Date == startDate && course.StartNotif)
            {
                var notif = new NotificationRequest
                {
                    NotificationId = course.CourseId + 10,
                    Title = "Course Begins Soon",
                    Description = $"{course.CourseName} begins tomorrow",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(5)
                    }
                };
                await LocalNotificationCenter.Current.Show(notif);
            }
        }

        private async Task CourseEndNotif(Courses course)
        {
            var endDate = DateTime.Today.AddDays(1);

            if (course.EndDate.Date == endDate && course.EndNotif)
            {
                var notif = new NotificationRequest
                {
                    NotificationId = course.CourseId + 20,
                    Title = "Course Ends Soon",
                    Description = $"{course.CourseName} ends tomorrow",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(5)
                    }
                };
                await LocalNotificationCenter.Current.Show(notif);
            }
        }        

        async void TermsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is not CollectionView collectionView)
                    return;

                var selected = e.CurrentSelection?.FirstOrDefault() as Terms;
                if (selected == null)
                    return;

                SelectedTerm = selected;                
                
                await Task.Yield();

                if (CoursesCv != null)
                {
                    var courses = await App.db.GetCoursesPerTerm(SelectedTerm.TermId);
                    SelectedTerm.Courses = new ObservableCollection<Courses>(courses);
                    CoursesCv.ItemsSource = SelectedTerm.Courses;
                }              
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", ex.Message, "Ok");
            }
        }

        bool isEditMode = false;     

        private async void EditSaveModeOn_Clicked(object? sender, EventArgs e)
        {
            try
            {
                if (sender is not ToolbarItem tbi)
                    return;

                isEditMode = !isEditMode;           

                foreach (var termCard in Terms)
                {
                    termCard.ShowDelete = isEditMode;
                }

                if (CoursesCv.ItemsSource != null)
                {
                    foreach (var course in CoursesCv.ItemsSource.Cast<Courses>())
                    {
                        course.ShowDelete = isEditMode;
                    }
                } 
                
                if (isEditMode)
                {
                    tbi.Text = "Save";
                    tbi.IconImageSource = "save.png";
                }
                else
                {
                    foreach (var term in Terms)
                    {
                        await App.db.SaveTermAsync(term);
                    }

                    if (SelectedTerm != null)
                    {
                        foreach (var course in SelectedTerm.Courses)
                        {
                            await App.db.SaveCourseAsync(course);
                        }
                    }                    

                    tbi.Text = "Edit";
                    tbi.IconImageSource = "edit_icon.png";
                }                
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", ex.Message, "Ok");
            }            
        }               

        async void OnCourseSelected(object  sender, SelectionChangedEventArgs e)
        {
            var course = e.CurrentSelection.FirstOrDefault() as Courses;
            if (course == null) return;

            await Navigation.PushAsync(new CoursePage(course)); 
        }

        async void GoToSearchReport_Clicked(object sender, EventArgs e)
        {
           await Navigation.PushAsync(new SearchReport());
        }

        private async void DeleteTerm(object sender, EventArgs e)
        {
            if (sender is not ImageButton b || b.BindingContext is not Terms term)
                return;

            bool confirm = await DisplayAlertAsync("Delete Term", $"Are you sure you want to delete '{term.TermTitle}'?", "Yes", "No");
            if (!confirm) return;

            if (SelectedTerm == term)
            {
                SelectedTerm = null;
            }               

            await App.db.DeleteTermAsync(term);
            Terms.Remove(term);
        }

        private async void DeleteCourse(object sender, EventArgs e)
        {
            if (sender is not ImageButton b || b.BindingContext is not Courses course)
                return;

            bool confirm = await DisplayAlertAsync("Delete Course", $"Are you sure you want to delete '{course.CourseName}'?", "Yes", "No");
            if (!confirm) return;

            await App.db.DeleteCourseAsync(course);
            SelectedTerm?.Courses.Remove(course);
        }

        private async void AddTerm_Clicked(object sender, EventArgs e)
        {
            string termTitle = NewTermTitle.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(termTitle))
            {
                await DisplayAlertAsync("Error", "Please enter term title", "Ok");
                return;
            }

            if (NewTermStart.Date > NewTermEnd.Date)
            {
                await DisplayAlertAsync("Error", "Start date cannot be later than end date", "Ok");
                return;
            }

            var newTerm = new Terms
            {
                TermTitle = termTitle,
                StartDate = NewTermStart.Date.Value,
                EndDate = NewTermEnd.Date.Value,
                ShowDelete = isEditMode
            };

            Terms.Add(newTerm);
            await App.db.SaveTermAsync(newTerm);
            AddTerm.IsVisible = false;
        }
        
        private async void ShowAddTerm_Clicked(object sender, EventArgs e)
        {
            AddTerm.IsVisible = true;

            NewTermTitle.Text = string.Empty;
            NewTermStart.Date = DateTime.Today;
            NewTermEnd.Date = DateTime.Today;
        }

        private async void CancelAddTerm_Clicked(object sender, EventArgs e)
        {
            AddTerm.IsVisible = false;
        }

        private async void ShowAddCourse_Clicked(object sender, EventArgs e)
        {
            if (SelectedTerm == null)
            {
                await DisplayAlertAsync("Error", "Please choose a term to add the course to", "Ok");
                return;
            }

            AddCourse.IsVisible = true;
        }

        private async void AddCourse_Clicked(object sender, EventArgs e)
        {
            if (SelectedTerm == null)
            {
                await DisplayAlertAsync("Error", "Term not selected, please select a term", "Ok");
                return;
            }

            string courseName = NewCourseName.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(courseName))
            {
                await DisplayAlertAsync("Error", "Please enter course name", "Ok");
                return;
            }

            if (NewCourseStart.Date > NewCourseEnd.Date)
            {
                await DisplayAlertAsync("Error", "Start date cannot be later than end date", "Ok");
                return;
            }

            if (CourseStatusPicker.SelectedItem == null)
            {
                await DisplayAlertAsync("Error", "Please select a course status", "Ok");
                return;
            }

            if (NewInstructorName == null || NewInstructorEmail == null || NewInstructorPhone == null)
            {
                await DisplayAlertAsync("Error", "Instructor fields cannot be empty", "Ok");
                return;
            }

            var newCourse = new Courses
            {
                TermId = SelectedTerm.TermId,
                CourseName = courseName,
                CourseStatus = CourseStatusPicker.SelectedItem?.ToString() ?? "Inactive",
                InstructorName = NewInstructorName.Text ?? "",
                InstructorEmail = NewInstructorEmail.Text ?? "",
                InstructorPhone = NewInstructorPhone.Text ?? "",
                StartDate = NewCourseStart.Date ?? DateTime.Today,
                EndDate = NewCourseEnd.Date ?? DateTime.Today,
                ShowDelete = isEditMode
            };

            await App.db.SaveCourseAsync(newCourse);

            SelectedTerm.Courses.Add(newCourse);
            CoursesCv.ItemsSource = null;
            CoursesCv.ItemsSource = SelectedTerm.Courses;

            NewCourseName.Text = "";
            NewInstructorName.Text = "";
            NewInstructorPhone.Text = "";
            NewCourseStart.Date = DateTime.Today;
            NewCourseEnd.Date = DateTime.Today;

            AddCourse.IsVisible = false;
        }

        private async void CancelAddCourse_Clicked(object sender, EventArgs e)
        {
            AddCourse.IsVisible = false;
        }
    }
}
