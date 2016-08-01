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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;



namespace Smart_Shortcut
{
    public sealed partial class Maps : Page
    {
        public MapIcon MyLocationIcon = new MapIcon();
        public Maps()
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

        private void setPositionIcon(MapIcon mapIcon,Geopoint center, string iconpath, string nameofPos)
        {
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri(iconpath));
            mapIcon.Title = nameofPos;
            mapIcon.Location = center;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            map1.MapElements.Add(mapIcon);
        }
        

        //----------------
        private async void Find_But_Click(object sender, RoutedEventArgs e)
        {
            Find_But.IsEnabled = false;
            Status_Text.Text = "FINDING...";
            map1.MapElements.Clear();
            if (FindName_Box.Text == "") return;
            string search_string = FindName_Box.Text;
            string link = MainPage.ServerLink + "search.php?place=" + search_string;
            HttpClient httpClient = new HttpClient();
            string HTMLCode = await httpClient.GetStringAsync(link);
            if (HTMLCode == "1" || HTMLCode == "")
            {
                Status_Text.Text = "FIND FAILED";
            }
            else
            {
                JsonObject json = JsonObject.Parse(HTMLCode);
                if ((json["resourceSets"].GetArray()[0].GetObject()["estimatedTotal"].GetNumber() == 1))
                {
                    string x = Convert.ToString((json["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[0].GetNumber());
                    string y = Convert.ToString((json["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[1].GetNumber());
                    var local = new Geopoint(new BasicGeoposition() { Latitude = double.Parse(x), Longitude = double.Parse(y) });

                    ShowTraffic(search_string, local);
                }
                else
                {
                    Status_Text.Text = "NOT FOUND";
                }
            }
            Find_But.IsEnabled = true;
        }

        private async void ShowTraffic(string search_string, Geopoint local)
        {

                    string link1 = MainPage.ServerLink + "traficjam.php?local=" + search_string;
                    HttpClient httpClient1 = new HttpClient();
                    if (httpClient1.GetStringAsync(link1).IsFaulted)
                    {
                        Status_Text.Text = "FIND FAILED";
                        return;
                    }
                    string HTMLCode1 = await httpClient1.GetStringAsync(link1);

                    if (HTMLCode1 == "1" || HTMLCode1 == "")
                    {
                Status_Text.Text = "FIND FAILED";
            }
                    else
                    {
                        List<double> lat = new List<double>();
                        List<double> lon = new List<double>();
                        List<string> time = new List<string>();
                        string code = HTMLCode1;
                        while (code.IndexOf("|") > 0)
                        {
                            lat.Add(double.Parse(code.Substring(0, code.IndexOf("_"))));
                            lon.Add(double.Parse(code.Substring(code.IndexOf("_") + 1, code.IndexOf("=") - code.IndexOf("_") - 1)));
                            time.Add(Convert.ToString(code.Substring(code.IndexOf("=") + 1, code.IndexOf("|") - code.IndexOf("=") - 1)));
                            code = code.Substring(code.IndexOf("|") + 1, code.Length - code.IndexOf("|") - 1);
                        }
                        for (int j = 0; j < lat.Count; j++)
                        {
                            var trafic = new Geopoint(new BasicGeoposition() { Latitude = lat[j], Longitude = lon[j] });
                            var mapIcon = new MapIcon();

                            if (time[j] != "needhelp")
                            {
                                setPositionIcon(mapIcon, trafic, "ms-appx:///Pic_Sources/TrafficJam_Pin.png","");
                            }
                            else
                            {
                                setPositionIcon(mapIcon, trafic, "ms-appx:///Pic_Sources/Help_Pin.png","");
                            }
                        }

                    }
                    await map1.TrySetViewAsync(local, 20, 0, 0, MapAnimationKind.Bow);
                    Status_Text.Text = "COMPLETED";
        }
               
        private void ViewSwitch_But_Click(object sender, RoutedEventArgs e)
        {
            if (ViewSwitch_But.Content.ToString()=="EARTH")
            {
                ViewSwitch_But.Content = "MAP";
                map1.Style = MapStyle.AerialWithRoads;
            }
            else
            {
                ViewSwitch_But.Content = "EARTH";
                map1.Style = MapStyle.Road;
            }

        }
        private void DeleteMapIcon(MapIcon mapIcon)
        {
            map1.MapElements.Remove(mapIcon);
        }
        private async void MyLocation_But_Click(object sender, RoutedEventArgs e)
        {
            MyLocation_But.IsEnabled = false;
            map1.MapElements.Clear();
            Status_Text.Text = "SCANNING...";
            try
            {
                Geolocator geolocator = new Geolocator();
                Geoposition geoposition = await geolocator.GetGeopositionAsync();
                setPositionIcon(MyLocationIcon, geoposition.Coordinate.Point, "ms-appx:///Pic_Sources/MyLocation_Pin.png", "You here");
                string search = geoposition.Coordinate.Point.Position.Latitude.ToString() + "," + geoposition.Coordinate.Point.Position.Longitude.ToString();
                ShowTraffic(search, geoposition.Coordinate.Point);
            }
            catch (UnauthorizedAccessException)
            {
                Status_Text.Text = "LOCATION OFF";
            }
            MyLocation_But.IsEnabled = true;
        }

        private async void Submit_But_Click(object sender, RoutedEventArgs e)
        {
            Status_Text.Text = "SUBMITTING...";
            Submit_But.IsEnabled = false;
            try
            {
                Geolocator geolocator = new Geolocator();
                Geoposition geoposition = await geolocator.GetGeopositionAsync();

                string  user = MemberPage.UserTemp_Username;
                string pass = MemberPage.UserTemp_Password;

                string link = MainPage.ServerLink + "savelocal.php?lat=" + geoposition.Coordinate.Point.Position.Latitude.ToString() + "&loc=" + geoposition.Coordinate.Point.Position.Longitude.ToString() + "&user=" + user + "&pass=" + pass+ "&needhelp=no";
                HttpClient httpClient = new HttpClient();
                if (httpClient.GetStringAsync(link).IsFaulted)
                {
                    Status_Text.Text = "REQUEST FAILED";
                    Submit_But.IsEnabled = true;
                    return;
                }
                string HTMLCode = await httpClient.GetStringAsync(link);
                if (HTMLCode == "100")
                {
                    Status_Text.Text = "COMPLETED";
                }
                else
                {
                    Status_Text.Text = "REQUEST FAILED";
                }


            }
            catch (UnauthorizedAccessException)
            {
                Status_Text.Text = "LOCATION OFF";
            }
            Submit_But.IsEnabled = true;
        }

        private async void MapsPage1_Loaded(object sender, RoutedEventArgs e)
        {
            string Member = await ReadFile(MainPage.UserCheckedFile);
            if (Member.CompareTo("TRUE") != 0)
            {
                Submit_But.Visibility = Visibility.Collapsed;
                WayFinding_But.Visibility = Visibility.Collapsed;
            }
        }

        private void Back_But_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void WayFinding_But_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WayFinding));
        }
   }
}
