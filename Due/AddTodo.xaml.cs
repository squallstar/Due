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
    public partial class AddTodo : PhoneApplicationPage
    {
        public AddTodo()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Page_Loaded);
            txtDescription.Text = "";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            txtDescription.Focus();

            GoogleAnalytics.EasyTracker.GetTracker().SendView("add-new");

            if (NavigationContext.QueryString.ContainsKey("today"))
            {
                PageTitle.Text = "DUE TODAY";
            }
            else if (NavigationContext.QueryString.ContainsKey("tomorrow"))
            {
                PageTitle.Text = "DUE TOMORROW";
            }
            else
            {
                PageTitle.Text = "DUE SOMEDAY";
            }
        }

        private void SaveItem(object sender, EventArgs e)
        {
            if (txtDescription.Text != "")
            {
                var item = new Todo
                {
                    Title = txtDescription.Text.Trim(),
                    DateInsert = DateTime.Now
                };

                if (NavigationContext.QueryString.ContainsKey("today"))
                {
                    item.DueDate = DateTime.Today;
                }
                else if (NavigationContext.QueryString.ContainsKey("tomorrow"))
                {
                    item.DueDate = DateTime.Today.AddDays(1);
                }
                else
                {
                    item.DueDate = Utilities.Someday;
                }

                var db = Context.Current;

                db.todos.InsertOnSubmit(item);
                db.SubmitChanges();

                NavigationService.GoBack();
            }            
        }
    }
}