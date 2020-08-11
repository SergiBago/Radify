using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace PGTAWPF
{
    /// <summary>
    /// Lógica de interacción para ExportKML.xaml
    /// </summary>
    public partial class ExportKML : Window
    {
        public ExportKML()
        {
            InitializeComponent();
        }


        MapView mapform = new MapView();
        int maptime;
        int time;
        int Min_First_time;
        int Last_time;
        string FileName;
        StringBuilder KMLbuilder = new StringBuilder();
        List<CATALL> List = new List<CATALL>();
        //List<CustomActualGmapMarker> ActualMarkers = new List<CustomActualGmapMarker>();
        //List<CustomOldGmapMarker> OldMarkers = new List<CustomOldGmapMarker>();
        //List<CustomOldGmapMarker> ListFlight = new List<CustomOldGmapMarker>();
        List<Trajectories> SMRTrajectories = new List<Trajectories>();
        List<Trajectories> MLATTrajectories = new List<Trajectories>();
        List<Trajectories> ADSBTrajectories = new List<Trajectories>();


        /// <summary>
        /// Close window function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Load window function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportKML_Load(object sender, RoutedEventArgs e)
        {
            this.time = mapform.time;
            this.Last_time = mapform.Last_time;
            this.maptime = mapform.time;
            this.Min_First_time = mapform.Min_First_time;
            AlertVisible(false);
            this.List = mapform.List;
            EnableHourSetting(false);
            Showhour();
        }


        /// <summary>
        /// Shows an alert if someone of the parameters are wrong
        /// </summary>
        /// <param name="i"></param>
        private void AlertVisible(bool i)
        {
            if (i==true)
            {
                AlertIco.Visibility = Visibility.Visible;
                AlertLabel.Visibility = Visibility.Visible;
            }
            else
            {
                AlertIco.Visibility = Visibility.Hidden;
                AlertLabel.Visibility = Visibility.Hidden;
            }
        }

        public void GetMapForm(MapView form)
        {
            this.mapform = form;
        }


        /// <summary>
        /// checks if introduced data is correct, anf if yes exports the trajectories to a kml file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButtonClick(object sender, RoutedEventArgs e)
        {
            showDetectionAlert(false);
            showHourAlert(false);
            showcantExportAlert(false);
            showNomessagesExported(false);
            bool Correct = true;
            if (CheckBoxSMR.IsChecked == false && CheckBoxMLAT.IsChecked == false && CheckBoxADSB.IsChecked == false) //At least one type of vehciles must be selected
            {
                showDetectionAlert(true);
            }
            else
            {
                
                if (CheckCustomTime.IsChecked == true)
                {
                    try
                    {
                        int min = Convert.ToInt32(minutes.Text);
                        int hours = Convert.ToInt32(Hours.Text);
                        int sec = Convert.ToInt32(seconds.Text);
                        if (min > 60 || hours > 24 || sec > 60) { Showhour(); showHourAlert(true); Correct = false; }
                        else { time = hours * 3600 + min * 60 + sec; }
                        if (time < Min_First_time) { time += 86400; }
                    }
                    catch { showHourAlert(true); Correct = false; }
                }
                else if (CheckSimulationTime.IsChecked == true)
                {
                }
                else
                {
                    time = mapform.Last_time;
                }
                if (Correct == true) //If selected type and time are correct then proceed to export.
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "kml files (*.kml*)|*.kml*";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == true && saveFileDialog1.SafeFileName != null) //File name introduced is okey
                    {
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                        string path0 = saveFileDialog1.FileName;
                        string path = path0 + ".kml";
                        string[] splitpath = path0.Split(System.IO.Path.DirectorySeparatorChar);
                        FileName = splitpath[splitpath.Count() - 1];
                        if (File.Exists(path)) { File.Delete(path); }
                        CreateTrajectoriesLists(); //Creates trajectoires for all vehicles

                        if (SMRTrajectories.Count == 0 && MLATTrajectories.Count == 0 && ADSBTrajectories.Count == 0) //At least one trajectory must be created to export something
                        {
                            showNomessagesExported(true); 
                        }
                        else
                        {
                            CreateKMLFile(); //Creates all the text 
                            File.WriteAllText(path, KMLbuilder.ToString()); //saves text to kml file
                            Mouse.OverrideCursor = null; 
                            this.Close(); 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates trajectories for all vehicles. Each trajectory is a list contaning all the messages from a vehicle 
        /// 
        /// Trajectories are not returned. The changes in trajectories are applied in the trajectories list of this class. Every time function is called trajectories are reseted
        /// </summary>
        private void CreateTrajectoriesLists()
        {
            SMRTrajectories = new List<Trajectories>();
            MLATTrajectories = new List<Trajectories>();
            ADSBTrajectories = new List<Trajectories>();
            int i = 0;
            try
            {
                while (List[i].Time_Of_day <= time)
                {
                    CATALL message = List[i];
                    if (message.Latitude_in_WGS_84 != -200 && message.Longitude_in_WGS_84 != -200)
                    {
                        if (message.DetectionMode == "SMR" && CheckBoxSMR.IsChecked == true)
                        {
                            if (message.Target_Identification != null)
                            {
                                if (SMRTrajectories.Exists(x => x.Target_Identification == message.Target_Identification)) { SMRTrajectories.Find(x => x.Target_Identification == message.Target_Identification).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    SMRTrajectories.Add(traj);
                                }
                            }
                            else if (message.Target_Address != null)
                            {
                                if (SMRTrajectories.Exists(x => x.Target_Address == message.Target_Address)) { SMRTrajectories.Find(x => x.Target_Address == message.Target_Address).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    SMRTrajectories.Add(traj);
                                }
                            }
                            else if (message.Track_number != null)
                            {
                                if (SMRTrajectories.Exists(x => x.Track_number == message.Track_number)) { SMRTrajectories.Find(x => x.Track_number == message.Track_number).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    SMRTrajectories.Add(traj);
                                }
                            }
                        }
                        else if (message.DetectionMode == "MLAT" && CheckBoxMLAT.IsChecked == true)
                        {
                            if (message.Target_Identification != null)
                            {
                                if (MLATTrajectories.Exists(x => x.Target_Identification == message.Target_Identification)) { MLATTrajectories.Find(x => x.Target_Identification == message.Target_Identification).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    MLATTrajectories.Add(traj);
                                }
                            }
                            else if (message.Target_Address != null)
                            {
                                if (MLATTrajectories.Exists(x => x.Target_Address == message.Target_Address)) { MLATTrajectories.Find(x => x.Target_Address == message.Target_Address).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    MLATTrajectories.Add(traj);
                                }
                            }
                            else if (message.Track_number != null)
                            {
                                if (MLATTrajectories.Exists(x => x.Track_number == message.Track_number)) { MLATTrajectories.Find(x => x.Track_number == message.Track_number).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    MLATTrajectories.Add(traj);
                                }
                            }
                        }
                        else if (message.DetectionMode == "ADSB" && CheckBoxADSB.IsChecked == true)
                        {
                            if (message.Target_Identification != null)
                            {
                                if (ADSBTrajectories.Exists(x => x.Target_Identification == message.Target_Identification)) { ADSBTrajectories.Find(x => x.Target_Identification == message.Target_Identification).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    ADSBTrajectories.Add(traj);
                                }
                            }
                            else if (message.Target_Address != null)
                            {
                                if (ADSBTrajectories.Exists(x => x.Target_Address == message.Target_Address)) { ADSBTrajectories.Find(x => x.Target_Address == message.Target_Address).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    ADSBTrajectories.Add(traj);
                                }
                            }
                            else if (message.Track_number != null)
                            {
                                if (ADSBTrajectories.Exists(x => x.Track_number == message.Track_number)) { ADSBTrajectories.Find(x => x.Track_number == message.Track_number).AddPoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                                else
                                {
                                    Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                    ADSBTrajectories.Add(traj);
                                }
                            }
                        }
                    }
                    i++;
                }
            }
            catch { showHourAlert(true); }
        }


        /// <summary>
        /// Creates the full KML file with the folders and all the trajectories
        /// </summary>
        private void CreateKMLFile()
        {
            StartKMLFile();
            if (CheckBoxSMR.IsChecked == true && SMRTrajectories.Count > 0) { CreateSMRFolder(); }
            if (CheckBoxMLAT.IsChecked == true && MLATTrajectories.Count > 0) { CreateMLATFolder(); }
            if (CheckBoxADSB.IsChecked == true && ADSBTrajectories.Count > 0) { CreateADSBFolder(); }
            EndKMLFile();
        }


        /// <summary>
        /// Inserts into the KMLbuilder the heather of the document
        /// </summary>
        private void StartKMLFile()
        {
            KMLbuilder = new StringBuilder();
            KMLbuilder.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
            KMLbuilder.AppendLine("<kml xmlns='http://www.opengis.net/kml/2.2'>");
            KMLbuilder.AppendLine("<Document>");
        }

        /// <summary>
        /// Inserts the final of the kml document
        /// </summary>
        private void EndKMLFile()
        {
            KMLbuilder.Append("</Document>");
            KMLbuilder.AppendLine("</kml>");
        }


        /// <summary>
        /// Creates a SMR folder with all the trajectories of that type
        /// </summary>
        private void CreateSMRFolder()
        {
            KMLbuilder.AppendLine("<Folder><name>SMR</name><open>1</open>");
            foreach (Trajectories tra in SMRTrajectories)
            {
                KMLbuilder.AppendLine(tra.GetTrajectorieKML());
            }
            KMLbuilder.AppendLine("</Folder>");
        }

        /// <summary>
        /// Creates a MLAT folder with all the trajectories of that type
        /// </summary>
        private void CreateMLATFolder()
        {
            KMLbuilder.AppendLine("<Folder><name>MLAT</name><open>1</open>");
            foreach (Trajectories tra in MLATTrajectories)
            {
                KMLbuilder.AppendLine(tra.GetTrajectorieKML());
            }
            KMLbuilder.AppendLine("</Folder>");
        }

        /// <summary>
        /// Creates an ADSB folder with all the trajectories of that type
        /// </summary>
        private void CreateADSBFolder()
        {
            KMLbuilder.AppendLine("<Folder><name>ADS-B</name><open>1</open>");
            foreach (Trajectories tra in ADSBTrajectories)
            {
                KMLbuilder.AppendLine(tra.GetTrajectorieKML());
            }
            KMLbuilder.AppendLine("</Folder>");
        }

        /// <summary>
        /// Enables or disables de custom hour controls in the page
        /// </summary>
        /// <param name="hourenabled"></param>
        private void EnableHourSetting(bool hourenabled)
        {
            if (hourenabled == true)
            {
                Hours.IsEnabled = true;
                minutes.IsEnabled = true;
                seconds.IsEnabled = true;
                twodots1.Foreground = new SolidColorBrush(Color.FromRgb(228, 187, 151));
                twodots2.Foreground = new SolidColorBrush(Color.FromRgb(228, 187, 151));
            }
            else
            {
                Hours.IsEnabled = false;
                minutes.IsEnabled = false;
                seconds.IsEnabled = false;
                twodots1.Foreground = new SolidColorBrush(Color.FromRgb(114, 95, 75));
                twodots2.Foreground = new SolidColorBrush(Color.FromRgb(114, 95, 75));
            }
        }

        /// <summary>
        /// Shows or hides the alert
        /// </summary>
        private void showDetectionAlert(bool i)
        {
            if (i == true)
            {
                AlertVisible(true);
                AlertLabel.Text = "Select at least one type of detection.";
                AlertIco.Margin = new Thickness(-205, 0, 0, 0);
            }
            else
            {
                AlertVisible(false);
            }
        }

        /// <summary>
        /// Shows or hides the alert
        /// </summary>
        private void showcantExportAlert(bool i)
        {
            if (i == true)
            {
                AlertVisible(true);
                AlertLabel.Text = "There was a problem exporting your file.";
                AlertIco.Margin = new Thickness(-220, 0, 0, 0);
            }
            else
            {
                AlertVisible(false);
            }
        }

        /// <summary>
        /// Shows or hides the alert
        /// </summary>
        private void showNomessagesExported(bool i)
        {
            if (i == true)
            {
                AlertVisible(true);
                AlertLabel.Text = "There were no markers in the indicated time.";
                AlertIco.Margin =new Thickness(-240,0,0,0);
            }
            else
            {
                AlertVisible(false);
            }
        }

        /// <summary>
        /// Shows or hides the alert
        /// </summary>
        private void showHourAlert(bool i)
        {
            if (i == true)
            {
                AlertVisible(true);
                AlertLabel.Text = "Inserted time not valid.";
                AlertIco.Margin = new Thickness(-130, 0, 0, 0);
            }
            else
            {
                AlertVisible(false);
            }
        }

        /// <summary>
        /// Shows the hour into the custom hour controls
        /// </summary>
        private void Showhour()
        {
            int showingtime;
            if (time > 86400) { showingtime = time - 86400; }
            else
            {
                showingtime = time;
            }
            int hour = Convert.ToInt32(Math.Truncate(Convert.ToDouble(showingtime / 3600)));
            int min = Convert.ToInt32(Math.Truncate(Convert.ToDouble((showingtime - (hour * 3600)) / 60)));
            int sec = Convert.ToInt32(Math.Truncate(Convert.ToDouble((showingtime - ((hour * 3600) + (min * 60))))));
            Hours.Text = Convert.ToString(hour).PadLeft(2, '0');
            minutes.Text = Convert.ToString(min).PadLeft(2, '0');
            seconds.Text = Convert.ToString(sec).PadLeft(2, '0');
        }

        /// <summary>
        /// Drags the window on click and drag in the top bar
        /// </summary>
        private void MouseLeftButtonDownClick(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        /// <summary>
        /// If click on Export all, all others checkboxs got Checked/unchecked 
        /// </summary>
        private void ExportAllClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxAll.IsChecked == true)
            {
                CheckBoxADSB.IsChecked = true;
                CheckBoxMLAT.IsChecked = true;
                CheckBoxSMR.IsChecked = true;
            }
            else
            {
                CheckBoxADSB.IsChecked = false;
                CheckBoxMLAT.IsChecked = false;
                CheckBoxSMR.IsChecked = false;
            }
        }


        private void ExportSMRClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxSMR.IsChecked == false) { CheckBoxAll.IsChecked = false; }

        }

        private void ExportADSBClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxADSB.IsChecked == false) { CheckBoxAll.IsChecked = false; }

        }

        private void ExportMLATClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxMLAT.IsChecked == false) { CheckBoxAll.IsChecked = false; }

        }


        private void ExportCustomTimeClick(object sender, RoutedEventArgs e)
        {
            if (CheckCustomTime.IsChecked == true)
            {
                CheckAllTime.IsChecked = !CheckCustomTime.IsChecked;
                CheckSimulationTime.IsChecked = !CheckCustomTime.IsChecked;
                EnableHourSetting(true);
            }

        }

        private void ExportUntilSimulationClick(object sender, RoutedEventArgs e)
        {
            if (CheckSimulationTime.IsChecked == true)
            {
                CheckAllTime.IsChecked = !CheckSimulationTime.IsChecked;
                CheckCustomTime.IsChecked = !CheckSimulationTime.IsChecked;
                EnableHourSetting(false);
            }

        }

        private void ExportAllTime_Click(object sender, RoutedEventArgs e)
        {
            if (CheckAllTime.IsChecked == true)
            {
                CheckCustomTime.IsChecked = !CheckAllTime.IsChecked; ;
                CheckSimulationTime.IsChecked = !CheckAllTime.IsChecked; ;
                EnableHourSetting(false);
            }
            else { CheckAllTime.IsChecked = true; }

        }
    }

}
