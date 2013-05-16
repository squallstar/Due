using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Due.Data;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Data.Linq;

namespace Due
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Context db;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            this.db = Context.Current;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var collection = (from Todo s in this.db.todos select s).ToList();
            todayItems.ItemsSource = collection;
        }

        private void AddNew(object sender, EventArgs e)
        {

        }
    }
}