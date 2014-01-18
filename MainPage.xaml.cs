using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Net.NetworkInformation;

namespace sdkRSSReaderCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            
                OylamaDenetleyici(null, null);
                InitializeComponent();
                BuildLocalizedApplicationBar();
            

        }

        // Click handler which runs when the 'Load Feed' or 'Refresh Feed' button is clicked. 
        private void loadFeedButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        // Event handler which runs after the feed is fully downloaded.
        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
                if (e.Error != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        // Showing the exact error message is useful for debugging. In a finalized application, 
                        // output a friendly and applicable string to the user instead. 
                        MessageBox.Show("Ýnternet baðlantýsý yok. Haberlerin çekilebilmesi için internet baðlantýsý gereklidir.");
                        //deneme.Text = "Ýnternet baðlantýsý yok. Haberlerin çekilebilmesi için internet baðlantýsý gereklidir.";
                    });
                }
                else
                {
                    // Save the feed into the State property in case the application is tombstoned. 
                    this.State["feed"] = e.Result;

                    UpdateFeedList(e.Result);
                }
            
           
        }

        // This method determines whether the user has navigated to the application after the application was tombstoned.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {

                // First, check whether the feed is already saved in the page state.
                if (this.State.ContainsKey("feed"))
                {
                    // Get the feed again only if the application was tombstoned, which means the ListBox will be empty.
                    // This is because the OnNavigatedTo method is also called when navigating between pages in your application.
                    // You would want to rebind only if your application was tombstoned and page state has been lost. 
                    if (feedListBox.Items.Count == 0)
                    {
                        UpdateFeedList(State["feed"] as string);
                    }
                }

                // WebClient is used instead of HttpWebRequest in this code sample because 
                // the implementation is simpler and easier to use, and we do not need to use 
                // advanced functionality that HttpWebRequest provides, such as the ability to send headers.
                WebClient webClient = new WebClient();

                // Subscribe to the DownloadStringCompleted event prior to downloading the RSS feed.
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);

                // Download the RSS feed. DownloadStringAsync was used instead of OpenStreamAsync because we do not need 
                // to leave a stream open, and we will not need to worry about closing the channel. 
                webClient.DownloadStringAsync(new System.Uri("http://rss.feedsportal.com/c/32892/f/530178/index.rss"));
            }
            else
                MessageBox.Show("Ýnternet baðlantýsý yok. Haberlerin çekilebilmesi için internet baðlantýsý gereklidir.");

        }
           

        // This method sets up the feed and binds it to our ListBox. 
        private void UpdateFeedList(string feedXML)
        {
            // Load the feed into a SyndicationFeed instance
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            // In Windows Phone OS 7.1, WebClient events are raised on the same type of thread they were called upon. 
            // For example, if WebClient was run on a background thread, the event would be raised on the background thread. 
            // While WebClient can raise an event on the UI thread if called from the UI thread, a best practice is to always 
            // use the Dispatcher to update the UI. This keeps the UI thread free from heavy processing.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                // Bind the list of SyndicationItems to our ListBox
                feedListBox.ItemsSource = feed.Items;

                //loadFeedButton.Content = "Refresh Feed";
            });

        }

        // The SelectionChanged handler for the feed items 
        private void feedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                // Get the SyndicationItem that was tapped.
                SyndicationItem sItem = (SyndicationItem)listBox.SelectedItem;

                // Set up the page navigation only if a link actually exists in the feed item.
                if (sItem.Links.Count > 0)
                {
                    // Get the associated URI of the feed item.
                    Uri uri = sItem.Links.FirstOrDefault().Uri;

                    // Create a new WebBrowserTask Launcher to navigate to the feed item. 
                    // An alternative solution would be to use a WebBrowser control, but WebBrowserTask is simpler to use. 
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = uri;
                    webBrowserTask.Show();
                }
            }
        }
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            

            // Create a new menu item with the localized string from AppResources.

            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem("Kadromuz");
            ApplicationBarMenuItem appBarMenuItem1 = new ApplicationBarMenuItem("Hakkýnda");

            ApplicationBar.MenuItems.Add(appBarMenuItem);
            ApplicationBar.MenuItems.Add(appBarMenuItem1);
            appBarMenuItem.Click += appBarMenuItem_Click;
            appBarMenuItem1.Click += appBarMenuItem1_Click;

            

        }

        void appBarMenuItem1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Page2.xaml", UriKind.Relative));
        }

        void appBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.Relative));
        }

        private void OylamaDenetleyici(object sender, LaunchingEventArgs e)
        {
            int AcilisSayisi = 0;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("ProgramAcildi"))
            {
                AcilisSayisi = (int)IsolatedStorageSettings.ApplicationSettings["ProgramAcildi"];
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["ProgramAcildi"] = 0;
            }

            AcilisSayisi++;

            IsolatedStorageSettings.ApplicationSettings["ProgramAcildi"] = AcilisSayisi;
            if (AcilisSayisi == 5)
            {
                var returnvalue = MessageBox.Show("merhabalar, uygulamayý kullandýðýnýz için çok teþekkürler. bu bir öðrenci projesi olduðundan olumlu ya da olumsuz her türlü görüþünüzü bana iletirseniz uygulamayý daha iyi bir hale getirebilirim. Þimdiden vakit ayýrdýðýn için teþekkür ederim.", "bir dakikaný alabilir miyim?", MessageBoxButton.OKCancel);
                if (returnvalue == MessageBoxResult.OK)
                {
                    var marketplaceReviewTask = new MarketplaceReviewTask();
                    marketplaceReviewTask.Show();
                }
            }
        }
    }

}
