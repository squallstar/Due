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
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace Due
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Context db;

        ObservableCollection<Todo> todayList;
        ObservableCollection<Todo> tomorrowList;
        ObservableCollection<Todo> somedayList;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            this.db = Context.Current;

            todayList = new ObservableCollection<Todo>();
            tomorrowList = new ObservableCollection<Todo>();
            somedayList = new ObservableCollection<Todo>();
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!IsolatedStorageSettings.ApplicationSettings.Contains("setup"))
            {
                NavigationService.Navigate(new Uri("/Tutorial.xaml", UriKind.Relative));
            }

            this.RefreshData();
        }

        private void RefreshData()
        {
            var collection = (from Todo s in this.db.todos orderby s.Completed, s.DateInsert select s).ToList();

            DateTime today = DateTime.Today;
            DateTime tomorrow = DateTime.Today.AddDays(1);

            todayList.Clear();
            tomorrowList.Clear();
            somedayList.Clear();

            foreach (var todo in collection)
            {
                if (todo.Overdue) todayList.Add(todo);
                else if (todo.DueDate == tomorrow) tomorrowList.Add(todo);
                else somedayList.Add(todo);
            }

            todayItems.ItemsSource = todayList;
            tomorrowItems.ItemsSource = tomorrowList;
            somedayItems.ItemsSource = somedayList;

            foreach (var obj in todayItems.Items)
            {
                System.Diagnostics.Debug.WriteLine(obj.GetType());
            }
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
                db.SubmitChanges();
                this.RefreshData();
            }
        }

        private void About(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void TodoTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Todo item = (sender as Grid).DataContext as Todo;

            if (item.Completed)
            {
                if (todayList.Contains(item)) todayList.Remove(item);
                if (tomorrowList.Contains(item)) tomorrowList.Remove(item);
                if (somedayList.Contains(item)) somedayList.Remove(item);

                this.db.todos.DeleteOnSubmit(item);
                this.db.SubmitChanges();
            }
            else
            {
                (Application.Current as App).state = item;

                NavigationService.Navigate(new Uri("/ViewTodo.xaml", UriKind.Relative));
            }
        }
    }
}