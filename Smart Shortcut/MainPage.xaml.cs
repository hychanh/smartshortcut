using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Shapes;
using Windows.Services.Maps;
using System.Net.Http;
using Windows.Data.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace Smart_Shortcut
{
  
    public sealed partial class MainPage : Page
    {

       
        public static string ServerLink = "http://smartshortcut.hol.es/SmartShortcutServer/";
        public static string UserCheckedFile = "MemberChecked.ssf";
        public MainPage()
        {
            //Ar52ZO_55DYdvAGxHND6F5dTaRTLxTSOZa0-pR16w3HpVkuz2FYtSIxsz-PGJhEA
            InitializeComponent();
         
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string PolicyChecked = await ReadFile(Policy.PolicyCheckedFile);
            if (PolicyChecked.CompareTo("TRUE") == 0) Frame.Navigate(typeof(Start));
            else Frame.Navigate(typeof(Policy));
        }
    }
}
