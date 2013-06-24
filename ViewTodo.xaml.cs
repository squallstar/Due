using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Due.Data;

namespace Due
{
    public partial class ViewTodo : PhoneApplicationPage
    {
        private Todo item;

        public ViewTodo()
        {
            InitializeComponent();

            this.item = (Application.Current as App).state as Todo;

            this.todoTitle.Text = this.item.Title;
            this.todoAddedDate.Text = this.item.TimeAgo;

            if (item.Overdue)
            {
                btnToday.Visibility = Visibility.Collapsed;
                btnTomorrowUp.Visibility = Visibility.Collapsed;
            }
            else if (item.DueDate == DateTime.Today.AddDays(1))
            {
                btnTomorrowDown.Visibility = Visibility.Collapsed;
                btnTomorrowUp.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnSomeday.Visibility = Visibility.Collapsed;
                btnTomorrowDown.Visibility = Visibility.Collapsed;
            }

            DateTime today = DateTime.Today;
            if (item.ManualDueDate == null || item.ManualDueDate == false)
            {
                if (item.DueDate == today || item.DueDate == today.AddDays(1))
                {
                    datePicker.Value = item.DueDate;
                }
                else
                {
                    datePicker.Value = today;
                }
            }
            else
            {
                datePicker.Value = item.DueDate;
                this.datePicker.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void DeleteTap(object sender, EventArgs e)
        {
            (Application.Current as App).state = null;

            Context.Current.todos.DeleteOnSubmit(this.item);
            Context.Current.SubmitChanges();

            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }

        private void btnMarkCompleted_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.item.Completed = true;
            (Application.Current as App).db.SubmitChanges();

            NavigationService.GoBack();
        }

        private void btnToday_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.item.DueDate = DateTime.Today;
            this.item.ManualDueDate = false;
            (Application.Current as App).db.SubmitChanges();

            NavigationService.GoBack();
        }

        private void btnTomorrow_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.item.DueDate = DateTime.Today.AddDays(1);
            this.item.ManualDueDate = false;
            (Application.Current as App).db.SubmitChanges();

            NavigationService.GoBack();
        }

        private void btnSomeday_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.item.DueDate = Utilities.Someday;
            this.item.ManualDueDate = false;
            (Application.Current as App).db.SubmitChanges();

            NavigationService.GoBack();
        }

        private void btnSchedule_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.datePicker.Visibility = Visibility.Visible;
            this.datePicker.Focus();
        }

        private void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (e.NewDateTime == item.DueDate) return;

            if (e.NewDateTime != null)
            {
                this.item.DueDate = (DateTime)e.NewDateTime;
                this.item.ManualDueDate = true;
                (Application.Current as App).db.SubmitChanges();

                NavigationService.GoBack();
            }
        }
    }
}