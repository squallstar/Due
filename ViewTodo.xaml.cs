﻿using System;
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
            (Application.Current as App).db.SubmitChanges();

            NavigationService.GoBack();
        }

        private void btnTomorrow_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.item.DueDate = DateTime.Today.AddDays(1);
            (Application.Current as App).db.SubmitChanges();

            NavigationService.GoBack();
        }

        private void btnSomeday_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.item.DueDate = Utilities.Someday;
            (Application.Current as App).db.SubmitChanges();

            NavigationService.GoBack();
        }
    }
}