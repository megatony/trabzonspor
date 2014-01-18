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

namespace sdkRSSReaderCS
{
    public partial class Page2 : PhoneApplicationPage
    {
        public Page2()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.


            // Create a new menu item with the localized string from AppResources.

            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("Haberler");
            ApplicationBarMenuItem appBarMenuItem1 = new ApplicationBarMenuItem("Kadromuz");

            ApplicationBar.MenuItems.Add(appBarMenuItem);
            ApplicationBar.MenuItems.Add(appBarMenuItem1);
            appBarMenuItem.Click += appBarMenuItem_Click;
            appBarMenuItem1.Click += appBarMenuItem1_Click;



        }

        void appBarMenuItem1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.Relative));
        }

        private void appBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}