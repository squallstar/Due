using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Due
{
    public partial class Tutorial : PhoneApplicationPage
    {
        public Tutorial()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GoogleAnalytics.EasyTracker.GetTracker().SendView("tutorial");

            if (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void CloseTutorial(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Add("setup", 1);
            IsolatedStorageSettings.ApplicationSettings.Save();

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}