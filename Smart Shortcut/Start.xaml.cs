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
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Smart_Shortcut
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Start : Page
    {
        public Start()
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


        private void Member_But_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MemberPage));
        }

        private void Guess_But_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Maps));
        }

        private async void StartPage1_Loaded(object sender, RoutedEventArgs e)
        {
            await WriteFile(MainPage.UserCheckedFile, "FALSE");
        }

        private void Exit_But_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
