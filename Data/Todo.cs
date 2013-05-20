using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using Microsoft.Phone.Data.Linq.Mapping;

namespace Due.Data
{
    [Index(Columns = "DateInsert")]
    [Table]
    public class Todo
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false, DbType = "INT NOT NULL Identity", AutoSync = AutoSync.OnInsert)]
        public int ID
        {
            get;
            set;
        }

        private string title;

        [Column]
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    NotifyPropertyChanging("Title");
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private DateTime _dateInsert;

        [Column]
        public DateTime DateInsert
        {
            get { return _dateInsert; }
            set
            {
                if (_dateInsert != value)
                {
                    NotifyPropertyChanging("DateInsert");
                    _dateInsert = value;
                    NotifyPropertyChanged("DateInsert");
                }
            }
        }

        private DateTime _duedate;

        [Column]
        public DateTime DueDate
        {
            get { return _duedate; }
            set
            {
                if (_duedate != value)
                {
                    NotifyPropertyChanging("DueDate");
                    _duedate = value;
                    NotifyPropertyChanged("DueDate");
                }
            }
        }

        private bool _completed;

        [Column]
        public bool Completed
        {
            get { return _completed; }
            set
            {
                if (_completed != value)
                {
                    NotifyPropertyChanging("Completed");
                    _completed = value;
                    NotifyPropertyChanged("Completed");
                }
            }
        }

        public string TimeAgo
        {
            get
            {
                return Utilities.TimeAgo(this.DateInsert, "added").ToUpper();
            }
        }

        public bool Overdue
        {
            get
            {
                return this.DueDate.CompareTo(DateTime.Today) <= 0;
            }
        }

        public double UIOpacity
        {
            get
            {
                return this.Completed ? 0.3 : 1.0;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
