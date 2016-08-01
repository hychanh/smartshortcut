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
using Windows.Storage;
using System.Threading.Tasks;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Smart_Shortcut
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Policy : Page
    {
        public static string PolicyCheckedFile = "PolicyCheckedFile.ssf";
        public string PolicyLink = "http://ssapartner.xyz/policy.html";
        public Policy()
        {
            this.InitializeComponent();
          
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void Start_But_Click(object sender, RoutedEventArgs e)
        {
            await WriteFile(PolicyCheckedFile, "TRUE");
            Frame.Navigate(typeof(Start));
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            Start_But.Visibility = Visibility.Visible;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Start_But.Visibility = Visibility.Collapsed;
        }

        private void PolicyPage1_Loaded(object sender, RoutedEventArgs e)
        {
            WebPage.Navigate(new Uri(PolicyLink));
        }

        private void WebPage_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            checkBox.Visibility = Visibility.Visible;
        }
    }
}
