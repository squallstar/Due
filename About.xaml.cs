using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Due
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void btnCollectorTry_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceDetailTask task = new MarketplaceDetailTask();
            task.ContentType = MarketplaceContentType.Applications;
            task.ContentIdentifier = "8864cfd0-bc46-4a72-a3bd-bc2156b967de";
            task.Show();
        }
    }
}