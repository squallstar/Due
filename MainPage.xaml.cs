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
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.RefreshData();
        }

        private void RefreshData()
        {
            var collection = (from Todo s in this.db.todos select s).ToList();

            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);

            var todayList = new List<Todo>();
            var tomorrowList = new List<Todo>();
            var somedayList = new List<Todo>();

            foreach (var todo in collection)
            {
                if (todo.DueDate == today) todayList.Add(todo);
                else if (todo.DueDate == tomorrow) tomorrowList.Add(todo);
                else somedayList.Add(todo);
            }

            todayItems.ItemsSource = todayList;
            tomorrowItems.ItemsSource = tomorrowList;
            somedayItems.ItemsSource = somedayList;
        }



        private void AddNew(object sender, EventArgs e)
        {
            switch (mainPivot.SelectedIndex)
            {
                case 0:
                    NavigationService.Navigate(new Uri("/AddTodo.xaml?today", UriKind.Relative));
                    break;

                case 1:
                    NavigationService.Navigate(new Uri("/AddTodo.xaml?tomorrow", UriKind.Relative));
                    break;

                case 2:
                    NavigationService.Navigate(new Uri("/AddTodo.xaml?someday", UriKind.Relative));
                    break;
            }
            NavigationService.Navigate(new Uri("/AddTodo.xaml", UriKind.Relative));
        }

        private void ClearCompleted(object sender, EventArgs e)
        {
            var completed = (from Todo t in db.todos where t.Completed == true select t).ToList();
            if (completed.Count > 0)
            {
                db.todos.DeleteAllOnSubmit(completed);
                this.RefreshData();
            }
        }

        private void TodoTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Todo item = (sender as Grid).DataContext as Todo;
        }
    }
}