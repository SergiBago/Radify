using System;
using GMap.NET;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET.MapProviders;
using System.Data;

namespace PGTA_WPF
{
    /// <summary>
    /// Lógica de interacción para MapHelp.xaml
    /// </summary>
    public partial class MapHelp : Page
    {
        DataTable InfoMarker = new DataTable();

        public MapHelp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load the map and set the settings so that the user cannot move the location or zoom
        /// </summary>
        private void mapLoaded(object sender, RoutedEventArgs e)
        {
            gMapControl1.CanDragMap = false;
            gMapControl1.DragButton = MouseButton.Left;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.MinZoom = 14;
            gMapControl1.MaxZoom = 14;
            gMapControl1.Zoom = 14;
            gMapControl1.Position= new PointLatLng(41.29561833, 2.095114167);
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            gMapControl1.ShowCenter = false;
        }

        /// <summary>
        /// Loads the datatable with a vehicle info
        /// </summary>
        private void DatatableLoaded(object sender, RoutedEventArgs e)
        {
            MarkerInfoView.CanUserAddRows = false;
            InfoMarker.Columns.Clear();
            InfoMarker.Columns.Add("Target\nId");
            InfoMarker.Columns.Add("Target\nAddress");
            InfoMarker.Columns.Add("Track\nNumber");
            InfoMarker.Columns.Add("CAT");
            InfoMarker.Columns.Add("SIC");
            InfoMarker.Columns.Add("SAC");
            InfoMarker.Columns.Add("Detection\nMode");
            InfoMarker.Columns.Add("Time");
            InfoMarker.Columns.Add("Flight\nLevel");
            MarkerInfoView.IsReadOnly = true;
            MarkerInfoView.CanUserResizeColumns = false;
            InfoMarker.Rows.Clear();
            var row = InfoMarker.NewRow();
            row["CAT"] = "21 v.2.1";
            row["SAC"] = "20"; 
            row["SIC"] = "219"; 
            row["Target\nId"] = "RYR5MP"; 
            row["Detection\nMode"] = "ADS-B";
            row["Target\nAddress"] ="4CA849"; 
            row["Track\nNumber"] = "237"; 
            row["Time"] = "09:56:56";
            row["Flight\nLevel"] = "FL. 32"; 
            InfoMarker.Rows.Add(row);
            MarkerInfoView.ItemsSource = InfoMarker.DefaultView;
            MarkerInfoView.DataContext = InfoMarker.DefaultView;
            MarkerInfoView.UpdateLayout();
        }

        private void ChangeViewClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "CHANGE MAP STYLE: With this button you can choose the map style that you like the most.";

        }

        private void CenterButClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "CENTER MAP: These buttons allow you to center the map on the most interesting points. If all the files you have are from the same airport, the buttons adjust to show you that airport, its city and its province. If you have files from more than one airport you will only see a single button that will allow you to focus your eyes on the peninsula. Anyway we remind you that by zooming or dragging the map you can go wherever you want!";

        }

        private void SpeedClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "SPEED ADJUSTMENT: It is used to choose the speed at which we want the simulation to go.The x1 means that one second of reality equals one second of the simulation. From there the speed will be multiplied by the number indicated on each button. The selected button is shown with a darker backcolor so we can know at what speed we are going. If you have the Show Old Positions parameter selected it is possible that the program will take longer to make the calculations and will not be able to reach the highest speeds. If you want to go at speeds of x10 or x20 we recommend that you deselect that option and select it when you have reached the desired time or select only the option to Show Vehicle History, much lighter for the program.";

        }

        private void ShowOnListClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "SHOW ON LIST: This option will show us the selected marker in the list of its corresponding category, so we can see the entire message with all its parameters. By default, if the marker has callsign, when displaying the list, it will order it alphabetically by that column so that we will see all the messages of that vehicle ordered. If it does not have callsign they will be ordered by target address, and if it does not have target adress, they will be ordered by track number. This applies to all messages, current and previous, so if you want to see a previous bookmark, select it and click on this button to see all its information. Remember that changing tabs only pauses the map but does not restart it, so you can go to the table without concern and when you return everything will be exactly as you left it!";

        }

        private void ShowOnMapClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "SHOW VEHICLE: Clicking this option will show us the position of the selected marker. If we have the zoom too low, the program will augment it a little by default to show us best the vehicle. If, on the other hand, we have a lot of zoom, the program will not decrease it, since we will be able to find the vehicle better in areas where there are many markers. If we select this option with the simulation running, the program will start to follow the vehicle automatically. You can freely zoom while following a vehicle, and the program will continue to track it! If you get tired of following a vehicle, just drag the map a little and this option will be disabled.";

        }

        private void DataGridClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "MARKER INFORMATION: In this table you can see the most relevant information of the selected marker. If you have selected a marker while the simulation is running, the information will be updated with the data of the new markers of that same vehicle. If you want to see the previous data, simply pause the simulation and click on a previous marker.";
        }

        private void ExportKMLClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "EXPORT KML: This button opens a tab where you can choose the parameters to export the markers to a kml file, which can be opened with Google Earth. In the tab you can choose what type of markers you want to export based on their detection method, and also, until what time you want to export.";

        }


        private void RestartClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "RESTART: Restarts the simulation, erases all the markers from the map and sets the time to the time of the first message.";

        }

        private void TimeClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "TIME: This tab shows the current time of the simulation. By default the map is loaded at the time of the first message received, but you can set the time you want.These labels show the current time in the simulation, and the time it started from. When you change the current time, the simulation automatically advances to that time without loading the old markers, so you don't have to wait. If you want to see the position history you can change the 'from:' time and the program will load all the markers from that time to the current time. Anyway, we remind you that the history of a vehicle is automatically loaded when you click on it, so that you will not have to change that time to see the position history of the selected vehicle. If after changing the time you press enter, the program navigates at that time, but the simulation is still paused. If, on the other hand, you hit play, the program will advance until that time and the simulation will start running. In order to change the time, you must pause the simulation first. Realize that, you have four parameters to change in every time label. Those do not correspond to hours, minutes, seconds and milliseconds as one might think, but correspond to days, hours, minutes and seconds. The parameter to choose the day will only be visible if you have files of more than one day loaded.";
        }

        private void TimeInfoClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "TIME INFORMATION: It shows you a small indication on how to use the time parameter.";
        }

        private void PlayClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "PLAY / PAUSE: Used to pause, start or continue the simulation. If the simulation is paused it will have a play icon and will be used to start or continue it. If the simulation is running it will have a pause icon and will be used to pause it. If you change the time and hit play it may take a little while depending on the computer you have and the number of messages in the file. Be patient! The simulation automatically pauses every time you switch windows, but does not restart. So you can move all you want through the program without losing what you are looking at!";

        }

        private void MapClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "MAP: On the map you can see all the markers. The current markers are in black, and the previous ones are in gray. Furthermore, the map differentiates between airplanes and surface vehicles. If the message does not specify whether the vehicle is an airplane or a surface vehicle, it's shown as a dot. Each marker has its callsing indicated, if it is unknown, it's target address, and if it is also unknown, the track number. If it is the target address, an 'A:' appears in front of the address, and if it is the track number, a 'N:' appears. If you have selected the Show Vehicle History option but not the Show Old Positions option, the previous markers are left without identification, since their identification will be the same as the selected marker, and thus we avoid unnecessary information. In addition, the program automatically calculates the direction of each vehicle at each point based on the following and previous points and places the markers pointing towards that direction. Furthermore, when loading a file, the program automatically calculates the detection ratio that each radar has configured. In this way, the markers that are detected normally, within the ratio established on the radar, appear black. If a marker is not detected when it should, that is, the radar fails to detect it, the marker will turn red to indicate that the vehicle should be there but the radar has not detected it. If the radar fails to detect it twice in a row, the marker disappears.";

        }

        private void ShowSMRClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "SHOW SMR: Shows the vehicles that have been detected by this type of radar.This filter also applies when showing old positions, so that the previous positions of this type of marker will also disappear. If you have selected a marker of this type, it will still be shown even if this option is unchecked.";

        }

        private void ShowMLATClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "SHOW MLAT Shows the vehicles that have been detected by this type of radar.This filter also applies when showing old positions, so that the previous positions of this type of marker will also disappear. If you have selected a marker of this type, it will still be shown even if this option is unchecked.";

        }

        private void ShowADSBClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "SHOW ADS-B: Shows the vehicles that have been detected by this type of radar.This filter also applies when showing old positions, so that the previous positions of this type of marker will also disappear. If you have selected a marker of this type, it will still be shown even if this option is unchecked.";

        }

        private void ShowOldPositionsClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "SHOW OLD POSITIONS: By selecting this option you will be able to see the previous positions of all the vehicles (even those that are no longer there!) During the simulation, this makes the program to use many more resources, so if you notice that it is not fluent, we recommend that you deactivate this option and activate it at the specific time you want. We remind you that you can go to a specific moment by changing the time and you will still be able to see all the previous positions. If you only want to see the position history of a vehicle we recommend that you select that vehicle and activate the option to show vehicle history instead of showing all positions.";

        }

        private void DataGridClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "MARKER INFORMATION: In this table you can see the most relevant information of the selected marker. If you have selected a marker while the simulation is running, the information will be updated with the data of the new markers of that same vehicle. If you want to see the previous data, simply pause the simulation and click on a previous marker.";
        }

        private void DataGridClick(object sender, DataGridSortingEventArgs e)
        {
            ExplanationLabel.Text = "MARKER INFORMATION: In this table you can see the most relevant information of the selected marker. If you have selected a marker while the simulation is running, the information will be updated with the data of the new markers of that same vehicle. If you want to see the previous data, simply pause the simulation and click on a previous marker.";

        }

        private void ShowVeicleHistoryClick(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "SHOW VEHICLE HISTORY: This option allows us to see the position history of a specific vehicle. As with the Show Old Positions option, it doesn't matter how you get to that vehicle or that time, the previous positions are always available to you!";
        }

        private void Measure_click(object sender, RoutedEventArgs e)
        {
            ExplanationLabel.Text = "MEASURE: This option allows you to measure distances and angles between vehicles and specific points or vehicles. To use it, you must first select the marker you want as a reference. Once you have selected the marker, click this button, and you will see that the mouse turns into a cross. It will allow you to click anywhere on the map. If you click on any point, a line will appear, with a label that will indicate the distance of the marker to that point in meters, and the angle between the marker and the point, in degrees and referenced to the north pole. If the simulation is running, the program will search for the markers of that same vehicle, and will update the line, so that the real distance and angle between the marker and the point are displayed at all times. You can also add lines between different markers. Once you have the first marker selected, click on the button and then click on the second marker. A line will appear between both markers, which will also be updated in real time to be able to see at all times the distance and angle between the vehicles. You can add as many lines as you want, and referenced in as many markers as you want! To delete a line just right click on the line or label and it will disappear.";
        }
    }
}
