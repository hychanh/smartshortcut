using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Devices.Geolocation;
using System.Net.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Smart_Shortcut
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MemberMenu : Page
    {

        public MemberMenu()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public async Task WriteFile(string fileName, string content)
        {
            byte[] data = System.Text.Encoding.Unicode.GetBytes(content);
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var s = await file.OpenStreamForWriteAsync())
            {
                await s.WriteAsync(data, 0, data.Length);
            }
        }

        public async Task<string> ReadFile(string fileName)
        {
            var folder = ApplicationData.Current.LocalFolder;

            try
            {
                var file = await folder.OpenStreamForReadAsync(fileName);

                using (var streamReader = new StreamReader(file))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void Scan_But_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Maps));
        }

        private async void Needhelp_But_Click(object sender, RoutedEventArgs e)
        {
            Needhelp_But.IsEnabled = false;
            try
            {
                Geolocator geolocator = new Geolocator();
                Geoposition geoposition = await geolocator.GetGeopositionAsync();
                string user = MemberPage.UserTemp_Username;
                string pass = MemberPage.UserTemp_Password;

                string link = MainPage.ServerLink + "savelocal.php?lat=" + geoposition.Coordinate.Point.Position.Latitude.ToString() + "&loc=" + geoposition.Coordinate.Point.Position.Longitude.ToString()+ "&user=" + user + "&pass=" + pass+"&needhelp=yes";
                HttpClient httpClient = new HttpClient();
                if (httpClient.GetStringAsync(link).IsFaulted)
                {
                    MessageDialog msgbox = new MessageDialog("Lost connection"); //lost connection
                    await msgbox.ShowAsync();
                    Needhelp_But.IsEnabled =  true;
                    return;
                }
                string HTMLCode = await httpClient.GetStringAsync(link);
                if (HTMLCode == "100")
                {
                    MessageDialog msgbox = new MessageDialog("Requestion accepted");//Luu thanh cong!
                    await msgbox.ShowAsync();
                }
                else if (HTMLCode == "1" || HTMLCode == "0" || HTMLCode == "")
                {
                    MessageDialog msgbox = new MessageDialog("Requestion failed");//password or username is wrong or can't connect to server
                    await msgbox.ShowAsync();
                }
                else if (HTMLCode == "50")
                {
                    MessageDialog msgbox = new MessageDialog("Requestion updated");
                    await msgbox.ShowAsync();
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageDialog msgbox = new MessageDialog("Location service is turned off!");
                await msgbox.ShowAsync();
            }
            Needhelp_But.IsEnabled = true;
        }

        private async void More_But_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgbox = new MessageDialog("Function Building");
            await msgbox.ShowAsync();
        }

        private void MemberMenuPage1_Loaded(object sender, RoutedEventArgs e)
        {
            Hello_Text.Text = "Hello, " + MemberPage.UserTemp_Username;
        }

        private async void Logout_But_Click(object sender, RoutedEventArgs e)
        {
            await WriteFile(MainPage.UserCheckedFile, "FALSE");
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}
