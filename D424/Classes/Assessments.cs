using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace C971App.Classes
{
    public class Assessments : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool showDelete;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [PrimaryKey, AutoIncrement]
        public int AsmntId { get; set; }
        public int CourseId { get; set; } //Foreign key

        public string AsmntName { get; set; } = string.Empty;
        public string AsmntType { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }

        public bool StartNotif { get; set; }
        public bool EndNotif { get; set; }

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
