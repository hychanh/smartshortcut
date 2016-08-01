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
using System.Net.Http;
using Windows.Storage;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Windows.Storage.Streams;

namespace Smart_Shortcut
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MemberPage : Page
    {
        /*  
          public class UserDetail
           {
               public string Username;
               public string Password;
           }

            public static UserDetail LastUser = new UserDetail();

           public static async Task UserWriteFile()
           {
               StorageFile userdetailsfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("LastUserInfo.dat",
               CreationCollisionOption.ReplaceExisting);
               IRandomAccessStream raStream = await userdetailsfile.OpenAsync(FileAccessMode.ReadWrite);
               using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
               {
                   // Serialize the Session State.
                   DataContractSerializer serializer = new DataContractSerializer(typeof(UserDetail));
                   serializer.WriteObject(outStream.AsStreamForWrite(), LastUser);
                   await outStream.FlushAsync();
               }

           }

           public static async void UserReadFromFile()
           {
               StorageFile file=null;
               file = await ApplicationData.Current.LocalFolder.GetFileAsync("LastUserInfo.dat");
               if (file==null) return;
              IRandomAccessStream inStream = await file.OpenReadAsync();
               // Deserialize the Session State.
               DataContractSerializer serializer = new DataContractSerializer(typeof(UserDetail));
               LastUser = (UserDetail)serializer.ReadObject(inStream.AsStreamForRead());
               inStream.Dispose();
           }*/
        public static string UserTemp_Username;
        public static string UserTemp_Password;

        public MemberPage()
        {
           InitializeComponent();

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
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void SIGNUP_Switch_Click(object sender, RoutedEventArgs e)
        {
            Username_in.Text = "";
            Password_In.Password = "";
            SignInBox.Visibility = Visibility.Collapsed;
            SignUpBox.Visibility = Visibility.Visible;
        }

        private void SIGNIN_SWITCH_Click(object sender, RoutedEventArgs e)
        {
            Username2_in.Text = "";
            Password2_In.Password = "";
            SignInBox.Visibility = Visibility.Visible;
            SignUpBox.Visibility = Visibility.Collapsed;
        }


        private async void RetypeBut_Click(object sender, RoutedEventArgs e)
        {
            RetypeBut.IsEnabled = false;
            string user = Username2_in.Text;
            string pass = Password2_In.Password;
            string repass = PassRetypeBox.Password;
          if (pass != repass)
            {
              Password2_In.Password = "";
              PassRetypeBox.Password = "";

                MessageDialog msgbox = new MessageDialog("Your password and retype password is not match!");// pass k giong nhau
                await msgbox.ShowAsync();
                ReEnterPassPage.Visibility = Visibility.Collapsed;
            }
            else
            {
                string link = MainPage.ServerLink+ "signup.php?user=" + user + "&pass=" + pass;
                HttpClient httpClient = new HttpClient();
                string HTMLCode;
                if (httpClient.GetStringAsync(link).IsFaulted)
                {
                    MessageDialog msgbox = new MessageDialog("Lost connection");
                    await msgbox.ShowAsync();
                }
                else
                {
                    HTMLCode = await httpClient.GetStringAsync(link);
                    if (HTMLCode == "100")
                    {
                        MessageDialog msgbox = new MessageDialog("Sign up successed!");
                        await msgbox.ShowAsync();
                        Username2_in.Text = "";
                        Password2_In.Password = "";
                        PassRetypeBox.Password = "";
                        SignUpBox.Visibility = Visibility.Collapsed;
                        SignInBox.Visibility = Visibility.Visible;
                    }
                    else if (HTMLCode == "1" || HTMLCode == "")
                    {
                        MessageDialog msgbox = new MessageDialog("Lost connection and we can't touch server!");
                        await msgbox.ShowAsync();
                    }
                    else
                    {
                        MessageDialog msgbox = new MessageDialog("Username is existed, Try another!");
                        await msgbox.ShowAsync();
                        PassRetypeBox.Password = "";
                        Password2_In.Password = "";
                        ReEnterPassPage.Visibility = Visibility.Collapsed;
                    }
                }
            }
            RetypeBut.IsEnabled = true;
            ReEnterPassPage.Visibility = Visibility.Collapsed;
        }

        private async void SignIn_But_Click(object sender, RoutedEventArgs e)
        {
                SignIn_But.IsEnabled = false;
            if (Username_in.Text == "" || Password_In.Password == "")
            {
                MessageDialog msgbox = new MessageDialog("Username or Password is empty");
                await msgbox.ShowAsync();
            }
            else
            {
                string user = Username_in.Text;
                string pass = Password_In.Password;
                string link = MainPage.ServerLink + "signin.php?user=" + user + "&pass=" + pass;
                HttpClient httpClient = new HttpClient();
                string HTMLCode;
                if (httpClient.GetStringAsync(link).IsFaulted)
                {
                    MessageDialog msgbox = new MessageDialog("Lost connection");
                    await msgbox.ShowAsync();
                }
                else
                {
                    HTMLCode = await httpClient.GetStringAsync(link);
                    if (HTMLCode == "100")
                    {
                    /*    if (Remember_Checkbox.IsChecked == true)
                        {
                            LastUser.Username = Username_in.Text;
                            LastUser.Password = Password_In.Password;
                            await UserWriteFile();
                        }*/
                        SignIn_But.IsEnabled = true;
                        await WriteFile(MainPage.UserCheckedFile, "TRUE");
                        UserTemp_Username = Username_in.Text;
                        UserTemp_Password = Password_In.Password;
                        Frame.Navigate(typeof(MemberMenu));
                    }
                    else if (HTMLCode == "1" || HTMLCode == "0")
                    {
                        MessageDialog msgbox = new MessageDialog("Your password is wrong or username is not exist!");
                        await msgbox.ShowAsync();
                    }
                }
                
            }
            SignIn_But.IsEnabled = true;
        }

        private async void SignUp_But_Click(object sender, RoutedEventArgs e)
        {

            string user = Username2_in.Text;
            string pass = Password2_In.Password;
            if (user == "" || user.Length < 6)
            {
                MessageDialog msgbox = new MessageDialog("Enter your username larger than 6 characters");// chua nhap username
                await msgbox.ShowAsync();
            }
            else if (pass == "" || pass.Length < 6)
            {
                MessageDialog msgbox = new MessageDialog("Enter your password larger than 6 characters");// password chua du
                await msgbox.ShowAsync();
            }
            else ReEnterPassPage.Visibility = Visibility.Visible;
        }

/*      private async void CheckLastUser()
        {
            LastUser.Username = "";
            LastUser.Password = "";
            UserReadFromFile();
            if (LastUser.Username != "") Remember_Checkbox.IsChecked = true;
            Username_in.Text = LastUser.Username;
            Password_In.Password= LastUser.Password;
            
        }*/

        private void Back_But_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
    
}
