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

        private void CloseTutorial(object sender, System.Windows.Input.GestureEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Add("setup", 1);
            IsolatedStorageSettings.ApplicationSettings.Save();
            NavigationService.GoBack();
        }
    }
}