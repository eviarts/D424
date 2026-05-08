using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace D424.Classes
{
    public class Terms : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool showDelete;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [PrimaryKey, AutoIncrement]
        public int TermId { get; set; }

        public string TermTitle {  get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Ignore]
        public ObservableCollection<Courses> Courses { get; set; } = new ObservableCollection<Courses>();

        [Ignore]
        public bool ShowDates => !ShowDelete;

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
                    OnPropertyChanged(nameof(ShowDates));
                }
            }
        }
    }    
}
