using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Windows;

namespace Due.Data
{
    public class Context : DataContext
    {
        public Context()
            : base("Data Source=isostore:/metroboard.sdf")
        { }

        public Table<Todo> todos;

        public static Context Current
        {
            get
            {
                return (Application.Current as App).db;
            }
        }
    }
}
