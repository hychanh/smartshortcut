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
using System.Collections.Generic;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Smart_Shortcut
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WayFinding : Page
    {
        private void ViewSwitch_But_Click(object sender, RoutedEventArgs e)
        {
            if (ViewSwitch_But.Content.ToString() == "EARTH")
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
        public WayFinding()
        {
            this.InitializeComponent();
        }
        public MapIcon MyLocationIcon = new MapIcon();
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        private async void MyLocation_But_Click(object sender, RoutedEventArgs e)
        {
            MyLocation_But.IsEnabled = false;
            map1.MapElements.Clear();
            try
            {
                Geolocator geolocator = new Geolocator();
                Geoposition geoposition = await geolocator.GetGeopositionAsync();
                setPositionIcon(MyLocationIcon, geoposition.Coordinate.Point, "ms-appx:///Pic_Sources/MyLocation_Pin.png", "You here");
                string search = geoposition.Coordinate.Point.Position.Latitude.ToString() + "," + geoposition.Coordinate.Point.Position.Longitude.ToString();
                ShowTraffic(search, geoposition.Coordinate.Point);
                A_Pos_Box.Text = "[My Location]";
            }
            catch (UnauthorizedAccessException)
            {
                MessageDialog msgbox = new MessageDialog("LOCATION OFF");
                await msgbox.ShowAsync();
            }
            MyLocation_But.IsEnabled = true;
        }

        private void Back_But_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        private void setPositionIcon(MapIcon mapIcon, Geopoint center, string iconpath, string nameofPos)
        {
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri(iconpath));
            mapIcon.Title = nameofPos;
            mapIcon.Location = center;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            map1.MapElements.Add(mapIcon);
        }

        private async void ShowTraffic(string search_string, Geopoint local)
        {

            string link1 = MainPage.ServerLink + "traficjam.php?local=" + search_string;
            HttpClient httpClient1 = new HttpClient();
            if (httpClient1.GetStringAsync(link1).IsFaulted)
            {
                MessageDialog msgbox = new MessageDialog("FIND FAILED!");
                await msgbox.ShowAsync();
                return;
            }
            string HTMLCode1 = await httpClient1.GetStringAsync(link1);

            if (HTMLCode1 == "1" || HTMLCode1 == "")
            {
                MessageDialog msgbox = new MessageDialog("FIND FAILED");
                await msgbox.ShowAsync();
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
                        setPositionIcon(mapIcon, trafic, "ms-appx:///Pic_Sources/TrafficJam_Pin.png", "");
                    }
                    else
                    {
                        setPositionIcon(mapIcon, trafic, "ms-appx:///Pic_Sources/Help_Pin.png", "");
                    }
                }

            }
            await map1.TrySetViewAsync(local, 20, 0, 0, MapAnimationKind.Bow);
        }
        private async void Find_But_Click(object sender, RoutedEventArgs e)
        {
            Find_But.IsEnabled = false;
            if (B_Pos_Box.Text != "" && A_Pos_Box.Text.CompareTo("[My Location]") == 0)
            {

                string search_string = B_Pos_Box.Text;
                string link = MainPage.ServerLink + "search.php?place=" + search_string;
                HttpClient httpClient = new HttpClient();
                if (httpClient.GetStringAsync(link).IsFaulted)
                {
                    MessageDialog msgbox = new MessageDialog("FIND FAILED!");
                    await msgbox.ShowAsync();
                    Find_But.IsEnabled = true;
                    return;
                }
                string HTMLCode = await httpClient.GetStringAsync(link);
                if (HTMLCode == "1" || HTMLCode == "")
                {
                    MessageDialog msgbox = new MessageDialog("FIND FAILED!");
                    await msgbox.ShowAsync();
                }
                else
                {
                    JsonObject json = JsonObject.Parse(HTMLCode);
                    if ((json["resourceSets"].GetArray()[0].GetObject()["estimatedTotal"].GetNumber() == 1))
                    {
                        string x = Convert.ToString((json["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[0].GetNumber());
                        string y = Convert.ToString((json["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[1].GetNumber());
                        var Blocal = new Geopoint(new BasicGeoposition() { Latitude = float.Parse(x), Longitude = float.Parse(y) });

                        Geolocator geolocator = new Geolocator();
                        Geoposition mylocal = await geolocator.GetGeopositionAsync();
                        var Alocal = new Geopoint(new BasicGeoposition() { Latitude = float.Parse(Convert.ToString(mylocal.Coordinate.Point.Position.Latitude)), Longitude = float.Parse(Convert.ToString(mylocal.Coordinate.Point.Position.Longitude)) });

                        MapRouteFinderResult routeResult =
                        await MapRouteFinder.GetDrivingRouteAsync(
                        new Geopoint(Blocal.Position),
                        new Geopoint(Alocal.Position),
                        MapRouteOptimization.Time,
                        MapRouteRestrictions.None);
                        if (routeResult.Status == MapRouteFinderStatus.Success)
                        {
                            MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                            viewOfRoute.RouteColor = Colors.Yellow;
                            viewOfRoute.OutlineColor = Colors.Black;
                            map1.Routes.Add(viewOfRoute);
                            await map1.TrySetViewBoundsAsync(
                                  routeResult.Route.BoundingBox,
                                  null,
                                  Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
                        }
                    }
                }
                Find_But.IsEnabled = true;
            }
            else if (B_Pos_Box.Text != "" && A_Pos_Box.Text != "")
            {
                string search_string = A_Pos_Box.Text;
                string search_string1 = B_Pos_Box.Text;

                string link = MainPage.ServerLink + "search.php?place=" + search_string;
                string link1 = MainPage.ServerLink + "search.php?place=" + search_string1;

                HttpClient httpClient = new HttpClient();
                if (httpClient.GetStringAsync(link).IsFaulted)
                {
                    MessageDialog msgbox = new MessageDialog("FIND FAILED!");
                    await msgbox.ShowAsync();
                    Find_But.IsEnabled = true;
                    return;
                }
                string HTMLCode = await httpClient.GetStringAsync(link);
                string HTMLCode1 = await httpClient.GetStringAsync(link);
                if (HTMLCode == "1" || HTMLCode == "")
                {
                    MessageDialog msgbox = new MessageDialog("FIND FAILED!");
                    await msgbox.ShowAsync();
                }
                else
                {
                    JsonObject json = JsonObject.Parse(HTMLCode);
                    if ((json["resourceSets"].GetArray()[0].GetObject()["estimatedTotal"].GetNumber() == 1))
                    {
                        string x = Convert.ToString((json["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[0].GetNumber());
                        string y = Convert.ToString((json["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[1].GetNumber());
                        var Alocal = new Geopoint(new BasicGeoposition() { Latitude = float.Parse(x), Longitude = float.Parse(y) });
                        JsonObject json1 = JsonObject.Parse(HTMLCode1);
                        if ((json1["resourceSets"].GetArray()[0].GetObject()["estimatedTotal"].GetNumber() == 1))
                        {
                            string x1 = Convert.ToString((json1["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[0].GetNumber());
                            string y1 = Convert.ToString((json1["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"]).GetArray()[1].GetNumber());
                            var Blocal = new Geopoint(new BasicGeoposition() { Latitude = float.Parse(x), Longitude = float.Parse(y) });
                            MapRouteFinderResult routeResult =
                       await MapRouteFinder.GetDrivingRouteAsync(
                       new Geopoint(Blocal.Position),
                       new Geopoint(Alocal.Position),
                       MapRouteOptimization.Time,
                       MapRouteRestrictions.None);
                            if (routeResult.Status == MapRouteFinderStatus.Success)
                            {
                                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                                viewOfRoute.RouteColor = Colors.Yellow;
                                viewOfRoute.OutlineColor = Colors.Black;
                                map1.Routes.Add(viewOfRoute);
                                await map1.TrySetViewBoundsAsync(
                                      routeResult.Route.BoundingBox,
                                      null,
                                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
                            }
                        }
                    }
                }
            }
        }

    }
}
