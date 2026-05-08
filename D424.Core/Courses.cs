using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace D424.Classes
{
    public class Courses : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool showDelete;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshAll()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }
        public int TermId { get; set; } //Foreign key

        public string CourseName { get; set; } = string.Empty;
        public string CourseStatus { get; set; } = string.Empty;
        public string? CourseNotes { get; set; }

        public string InstructorName { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public string InstructorPhone { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        
        public bool StartNotif { get; set; }        
        public bool EndNotif { get; set; }

        [Ignore]
        public ObservableCollection<Assessments> OA { get; set; } = new ObservableCollection<Assessments>();
        [Ignore]
        public ObservableCollection<Assessments> PA { get; set; } = new ObservableCollection<Assessments>();

        [Ignore]
        public bool ShowDetails => !ShowDelete;

        [Ignore]
        public bool ShowDelete
        {
            get => showDelete;
            set
            {
                if (showDelete != value)
                {
                    showDelete = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ShowDetails));
                }
            }
        }
    }
}
