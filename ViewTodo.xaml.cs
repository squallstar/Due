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
    }
}