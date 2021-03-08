using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Drawing = System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.TextFormatting;
using System.Data;
using System.Runtime.InteropServices;
using GMap.NET;
using System.IO;
using System.Reflection;

namespace PGTAWPF
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<System.Windows.Controls.Image> ActiveButtons;
        List<System.Windows.Controls.Image> UnActiveButtons;
        List<Border> ListPanels;
        List<TextBlock> ListLabels;

        bool mapstarted = false;
        bool CAT10started = false;
        bool CAT21v21started = false;
        bool CAT21v23started = false;
        bool CAT62started = false;
        bool CATAllstarted = false;
        bool Loadstarted = false;
        ReadFiles Archivo = new ReadFiles();

        List<CAT10> listaCAT10 = new List<CAT10>();
        List<CAT21vs21> listaCAT21v21 = new List<CAT21vs21>();
        List<CAT21vs23> listaCAT21v23 = new List<CAT21vs23>();
        List<CAT62> listaCAT62 = new List<CAT62>();
        List<CATALL> listaCATALL = new List<CATALL>();
        
        DataTable TableCat10 = new DataTable();
        DataTable TableCat21v21 = new DataTable();
        DataTable TableCat21v23 = new DataTable();
        DataTable TableCat62 = new DataTable();
        DataTable TableAll = new DataTable();

        MapView mapview = new MapView();
        LoadFiles load;
        View viewCat10;
        View viewCat23;
        View viewCat21;
        View viewCat62;
        View viewAll;
        MapView mapform;
        HelpForm help;


        public MainWindow()
        {
            InitializeComponent();
            ActiveButtons = new List<System.Windows.Controls.Image> { HomeIco2, LoadIco2, ListIco2, seeIco12, SeeIco22, SeeIco32, SeeIco42, SeeIco52, MapIco2,HelpIco2};
            UnActiveButtons = new List<System.Windows.Controls.Image> { HomeIco1, LoadIco1, ListIco1, seeIco11, SeeIco21, SeeIco31, SeeIco41, SeeIco51 ,MapIco1,HelpIco1};
            ListPanels = new List<Border> { HomePanel, LoadPanel, ListPanel, MapPanel, HelpPanel };
            ListLabels = new List<TextBlock> { HomeLabel, LoadFilesLabel, ListLabel, SeeCat10Label, SeeCat21v23Label, SeeCat21v21Label, SeeCat62Label, SeeAllLabel, MapLabel,HelpLabel };
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        
        private void Main_Load(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            HomeLabel.HorizontalAlignment = HorizontalAlignment.Right;
            HomeIco1.Visibility = Visibility.Hidden;
            HomeIco2.Visibility = Visibility.Visible;
            HomePanel.Background = new SolidColorBrush(RGBColors.color1);
            FormTitle.Text = "Home";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color1);
            HomeLabel.Foreground = new SolidColorBrush(RGBColors.color1);
            FormIco.Source = new BitmapImage(new Uri(@"images/Casa Color.png", UriKind.Relative));
            Intro Presentation = new Intro();
            Presentation.GetMainWindow(this);
            PanelChildForm.Navigate(Presentation);
        }


        /// <summary>
        /// Disable all active buttons
        /// </summary>
        private void DisableButtons()
        {
            foreach (System.Windows.Controls.Image im in ActiveButtons) { im.Visibility = Visibility.Hidden; }
            foreach (System.Windows.Controls.Image im in UnActiveButtons) { im.Visibility = Visibility.Visible; }
            foreach (Border bor in ListPanels) { bor.Background = new SolidColorBrush(RGBColors.color6); }
            foreach (TextBlock text in ListLabels) { text.HorizontalAlignment = HorizontalAlignment.Left;  text.Foreground = new SolidColorBrush(RGBColors.color7); }
        }

        /// <summary>
        /// Show a panel in front of buttons to make them inaccessible
        /// </summary>
        public void LoadDisableButtons()
        {
            PanelDisableButons.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide the panel in front of buttons so they are accesible
        /// </summary>
        public void LoadActiveButtons()
        {
            PanelDisableButons.Visibility = Visibility.Hidden;
        }


        /// <summary>
        /// Create RGB colors
        /// </summary>
        private struct RGBColors
        {
            public static Color color1 = Color.FromRgb(194, 184, 178);
            public static Color color2 = Color.FromRgb(176, 190, 169);
            public static Color color3 = Color.FromRgb(250, 248, 212);
            public static Color color4 = Color.FromRgb(228, 187, 151);
            public static Color color5 = Color.FromRgb(214, 158, 107);
            public static Color color6 = Color.FromRgb(70, 70, 70);
            public static Color color7 = Color.FromRgb(255, 255, 255);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            HomeLabel.HorizontalAlignment = HorizontalAlignment.Right;
            HomeIco1.Visibility = Visibility.Hidden;
            HomeIco2.Visibility = Visibility.Visible;
            HomePanel.Background = new SolidColorBrush(RGBColors.color1);
            FormTitle.Text = "Home";
            FormTitle.Foreground= new SolidColorBrush(RGBColors.color1);
            HomeLabel.Foreground = new SolidColorBrush(RGBColors.color1);
            FormIco.Source = new BitmapImage(new Uri(@"images/Casa Color.png", UriKind.Relative));
            if (mapview.started == true) { mapform.Pause(); }
            Intro Presentation = new Intro();
            Presentation.GetMainWindow(this);
            PanelChildForm.Navigate(Presentation);

        }

        /// <summary>
        /// Click on load Button. 
        /// </summary>
        private void Load_Click(object sender, MouseButtonEventArgs e)
        {
            DisableButtons();
            LoadFilesLabel.HorizontalAlignment = HorizontalAlignment.Right;
            LoadIco1.Visibility = Visibility.Hidden;
            LoadIco2.Visibility = Visibility.Visible;
            LoadPanel.Background = new SolidColorBrush(RGBColors.color2);
            FormTitle.Text = "Load FIles";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color2);
            LoadFilesLabel.Foreground = new SolidColorBrush(RGBColors.color2);
            FormIco.Source = new BitmapImage(new Uri(@"images/File Color.png", UriKind.Relative));
            if (mapview.started == true) { mapform.Pause(); }
            if (Loadstarted == false)
            {
                load = new LoadFiles();
                load.SetArchivo(Archivo);
                load.GetForm(this);
                Loadstarted = true;
            }
            PanelChildForm.Navigate(load);
        }

        /// <summary>
        /// Open load from another side other than the button
        /// </summary>
        public void OpenLoad()
        {
            DisableButtons();
            LoadFilesLabel.HorizontalAlignment = HorizontalAlignment.Right;
            LoadIco1.Visibility = Visibility.Hidden;
            LoadIco2.Visibility = Visibility.Visible;
            LoadPanel.Background = new SolidColorBrush(RGBColors.color2);
            FormTitle.Text = "Load Files";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color2);
            LoadFilesLabel.Foreground = new SolidColorBrush(RGBColors.color2);
            FormIco.Source = new BitmapImage(new Uri(@"images/File Color.png", UriKind.Relative));
            if (mapview.started == true) { mapform.Pause(); }
            if (Loadstarted == false)
            {
                load = new LoadFiles();
                load.SetArchivo(Archivo);
                load.GetForm(this);
                Loadstarted = true;
            }
            PanelChildForm.Navigate(load);
        }

        /// <summary>
        /// Open the see table category 10 page
        /// </summary>
        private void SeeCat10_Click(object sender, MouseButtonEventArgs e)
        {         
            activeSeecat10button();
            if (mapview.started == true) { mapform.Pause(); } //If simulation is running in map tab, pause the simulation before opening this tab
            if (listaCAT10.Count > 0) //check if there are CAT10 messages loaded
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; 
                if (CAT10started == false) //If it's first time we open the see cat 10 tab we create the page and load it, otherwise, we show it directily
                {
                    viewCat10 = new View();
                    viewCat10.GetType(0);
                    viewCat10.GetAll(listaCAT10,null, null,null, listaCATALL, TableCat10, null, null,null, null, TableCat10.Copy());

                   // viewCat10.GetAll(listaCAT10, listaCAT21v21, listaCAT21v23, listaCATALL, TableCat10, TableCat21v23, TableCat21v21, TableAll, TableCat10.Copy());
                    viewCat10.GetForm(this);
                    CAT10started = true;
                }
                PanelChildForm.Navigate(viewCat10);
            }
            else
            { //If no CAT10 messages loaded we show the page indicating there is no cat10 messages loaded
                NoMessages panel = new NoMessages();
                panel.GetType(1);
                panel.GetForm(this);
                PanelChildForm.Navigate(panel);
            }
        }


        /// <summary>
        /// Activate the See cat 10 button and customize program heather
        /// </summary>
        private void activeSeecat10button()
        {
            DisableButtons();
            ListLabel.HorizontalAlignment = HorizontalAlignment.Right;
            SeeCat10Label.HorizontalAlignment = HorizontalAlignment.Right;
            seeIco11.Visibility = Visibility.Hidden;
            seeIco12.Visibility = Visibility.Visible;
            ListIco1.Visibility = Visibility.Hidden;
            ListIco2.Visibility = Visibility.Visible;
            ListPanel.Background = new SolidColorBrush(RGBColors.color3);
            FormTitle.Text = "List View";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color3);
            ListLabel.Foreground = new SolidColorBrush(RGBColors.color3);
            SeeCat10Label.Foreground = new SolidColorBrush(RGBColors.color3);
            FormIco.Source = new BitmapImage(new Uri(@"images/Lista Color.png", UriKind.Relative));
        }


        /// <summary>
        /// Open the see table category 21 v 0.23 page
        /// </summary>
        private void SeeCat21v23Clicl(object sender, MouseButtonEventArgs e)
        {       
            activeSeecat23button();  
            if (mapview.started == true) { mapform.Pause(); }  //If simulation is running in map tab, pause the simulation before opening this tab
            if (listaCAT21v23.Count > 0) //check if there are CAT21 v.2.3 messages loaded
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (CAT21v23started == false) //If it's first time we open the see cat 21 v.2.3 tab we create the page and load it, otherwise, we show it directily
                {
                    viewCat23 = new View();
                    viewCat23.GetType(2);
                    viewCat23.GetAll(null,null, listaCAT21v23, null,listaCATALL, null, TableCat21v23, null, null,null, TableCat21v23.Copy());
                    viewCat23.GetForm(this);
                    CAT21v23started = true;
                }
                PanelChildForm.Navigate(viewCat23);
            }
            else //If no CAT 21 v0.23 messages loaded we show the page indicating there is no cat10 messages loaded
            {
                NoMessages panel = new NoMessages();
                panel.GetType(2);
                panel.GetForm(this);
                PanelChildForm.Navigate(panel);
            }
        }

        /// <summary>
        /// Activate the See cat 21 v.0.23 button and customize program heather
        /// </summary>
        private void activeSeecat23button()
        {
           
            DisableButtons();
            ListLabel.HorizontalAlignment = HorizontalAlignment.Right;
            SeeCat21v23Label.HorizontalAlignment = HorizontalAlignment.Right;
            SeeIco21.Visibility = Visibility.Hidden;
            SeeIco22.Visibility = Visibility.Visible;
            ListIco1.Visibility = Visibility.Hidden;
            ListIco2.Visibility = Visibility.Visible;
            ListPanel.Background = new SolidColorBrush(RGBColors.color3);
            FormTitle.Text = "List View";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color3);
            ListLabel.Foreground = new SolidColorBrush(RGBColors.color3);
            SeeCat21v23Label.Foreground = new SolidColorBrush(RGBColors.color3);
            FormIco.Source = new BitmapImage(new Uri(@"images/Lista Color.png", UriKind.Relative));

        }


        /// <summary>
        /// Open the see table category 21 v 2.1 page
        /// </summary>
        private void SeeCat21v21Click(object sender, MouseButtonEventArgs e)
        {
            
            activeSeeCat21button();
            if (mapview.started == true) { mapform.Pause(); }  //If simulation is running in map tab, pause the simulation before opening this tab
            if (listaCAT21v21.Count > 0) //check if there are CAT21 v. 2.1 messages loaded
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (CAT21v21started == false) //If it's first time we open the see cat 21 v2.1 tab we create the page and load it, otherwise, we show it directily
                {
                    viewCat21 = new View();
                    viewCat21.GetType(1);
                    viewCat21.GetAll(null, listaCAT21v21,null,null, listaCATALL, null, null, TableCat21v21, null,null, TableCat21v21.Copy());
                    viewCat21.GetForm(this);
                    CAT21v21started = true;
                }
                PanelChildForm.Navigate(viewCat21);
            }
            else //If no CAT 21 v.2.1 messages loaded we show the page indicating there is no cat10 messages loaded
            {
                NoMessages panel = new NoMessages();
                panel.GetType(3);
                panel.GetForm(this);
                PanelChildForm.Navigate(panel);
            }
        }

        /// <summary>
        /// Activate the See cat 21 v.2.1 button and customize program heather
        /// </summary>
        private void activeSeeCat21button()
        {
            
            DisableButtons();
            ListLabel.HorizontalAlignment = HorizontalAlignment.Right;
            SeeCat21v21Label.HorizontalAlignment = HorizontalAlignment.Right;
            SeeIco31.Visibility = Visibility.Hidden;
            SeeIco32.Visibility = Visibility.Visible;
            ListIco1.Visibility = Visibility.Hidden;
            ListIco2.Visibility = Visibility.Visible;
            ListPanel.Background = new SolidColorBrush(RGBColors.color3);
            FormTitle.Text = "List View";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color3);
            ListLabel.Foreground = new SolidColorBrush(RGBColors.color3);
            SeeCat21v21Label.Foreground = new SolidColorBrush(RGBColors.color3);
            FormIco.Source = new BitmapImage(new Uri(@"images/Lista Color.png", UriKind.Relative));
        }


        private void SeeCat62Click(object sender, MouseButtonEventArgs e)
        {
            activeSeeCat62button();
            if (mapview.started == true) { mapform.Pause(); }  //If simulation is running in map tab, pause the simulation before opening this tab
            if (listaCAT62.Count > 0) //check if there are CAT62 messages loaded
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (CAT62started == false) //If it's first time we open the see cat 21 v.2.3 tab we create the page and load it, otherwise, we show it directily
                {
                    viewCat62 = new View();
                    viewCat62.GetType(4);
                    viewCat62.GetAll(null, null,null,  listaCAT62, listaCATALL, null,null, TableCat62, null, null, TableCat62.Copy());
                    viewCat62.GetForm(this);
                    CAT62started = true;
                }
                PanelChildForm.Navigate(viewCat23);
            }
            else //If no CAT 21 v0.23 messages loaded we show the page indicating there is no cat10 messages loaded
            {
                NoMessages panel = new NoMessages();
                panel.GetType(4);
                panel.GetForm(this);
                PanelChildForm.Navigate(panel);
            }
        }

        private void activeSeeCat62button()
        {

            DisableButtons();
            ListLabel.HorizontalAlignment = HorizontalAlignment.Right;
            SeeCat62Label.HorizontalAlignment = HorizontalAlignment.Right;
            SeeIco51.Visibility = Visibility.Hidden;
            SeeIco52.Visibility = Visibility.Visible;
            ListIco1.Visibility = Visibility.Hidden;
            ListIco2.Visibility = Visibility.Visible;
            ListPanel.Background = new SolidColorBrush(RGBColors.color3);
            FormTitle.Text = "List View";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color3);
            ListLabel.Foreground = new SolidColorBrush(RGBColors.color3);
            SeeCat62Label.Foreground = new SolidColorBrush(RGBColors.color3);
            FormIco.Source = new BitmapImage(new Uri(@"images/Lista Color.png", UriKind.Relative));
        }


        /// <summary>
        /// Open the see table All page
        /// </summary>
        private void SeeAll_Click(object sender, MouseButtonEventArgs e)
        {
            activeSeeAllButton();
            if (mapview.started == true) { mapform.Pause(); }  //If simulation is running in map tab, pause the simulation before opening this tab
            if (listaCATALL.Count > 0) //check if there are messages loaded
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                if (CATAllstarted == false) //If it's first time we open the see cat all tab we create the page and load it, otherwise, we show it directily
                {
                    viewAll = new View();
                    viewAll.GetType(3);
                    viewAll.GetAll(listaCAT10, listaCAT21v21, listaCAT21v23, listaCAT62,listaCATALL, null, null, null, null,TableAll, TableAll.Copy());
                    viewAll.GetForm(this);
                    CATAllstarted = true;
                }
                PanelChildForm.Navigate(viewAll);
            }
            else //If no CAT All messages loaded we show the page indicating there is no cat10 messages loaded
            {
                NoMessages panel = new NoMessages();
                panel.GetType(4);
                panel.GetForm(this);
                PanelChildForm.Navigate(panel);
            }
        }

        /// <summary>
        /// Activate the See cat all button and customize program heather
        /// </summary>
        private void activeSeeAllButton()
        {
            DisableButtons();
            ListLabel.HorizontalAlignment = HorizontalAlignment.Right;
            SeeAllLabel.HorizontalAlignment = HorizontalAlignment.Right;
            SeeIco41.Visibility = Visibility.Hidden;
            SeeIco42.Visibility = Visibility.Visible;
            ListIco1.Visibility = Visibility.Hidden;
            ListIco2.Visibility = Visibility.Visible;
            ListPanel.Background = new SolidColorBrush(RGBColors.color3);
            FormTitle.Text = "List View";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color3);
            ListLabel.Foreground = new SolidColorBrush(RGBColors.color3);
            SeeAllLabel.Foreground = new SolidColorBrush(RGBColors.color3);
            FormIco.Source = new BitmapImage(new Uri(@"images/Lista Color.png", UriKind.Relative));

        }

        /// <summary>
        /// Open the map tab
        /// </summary>
        private void MapView_Click(object sender, MouseButtonEventArgs e)
        {
            ActiveMapButton();
            if (listaCATALL.Count > 0)
            {
                if (mapstarted == true) { PanelChildForm.Navigate(this.mapform); }
                else
                {
                    mapform = new MapView();
                    mapform.GetForm(this);
                    mapform.GetList(listaCATALL, Archivo.AirportCodesList);
                    mapstarted = true;
                    PanelChildForm.Navigate(mapform);
                }
            }
            else
            {
                NoMessages panel = new NoMessages();
                panel.GetForm(this);
                panel.GetType(5);
                PanelChildForm.Navigate(panel);
            }
        }

        /// <summary>
        /// Activate the map button and customize program heather
        /// </summary>
        private void ActiveMapButton()
        {
            DisableButtons();
            MapLabel.HorizontalAlignment = HorizontalAlignment.Right;
            MapIco1.Visibility = Visibility.Hidden;
            MapIco2.Visibility = Visibility.Visible;
            FormTitle.Text = "Map View";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color4);
            MapLabel.Foreground = new SolidColorBrush(RGBColors.color4);
            MapPanel.Background = new SolidColorBrush(RGBColors.color4);
            FormIco.Source = new BitmapImage(new Uri(@"images/Mapa Color.png", UriKind.Relative));
        }

        /// <summary>
        /// Open the help tab
        /// </summary>
        private void Help_Click(object sender, MouseButtonEventArgs e)
        {
            DisableButtons();
            HelpLabel.HorizontalAlignment = HorizontalAlignment.Right;
            HelpIco1.Visibility = Visibility.Hidden;
            HelpIco2.Visibility = Visibility.Visible;
            HelpPanel.Background = new SolidColorBrush(RGBColors.color5);
            FormTitle.Text = "Help";
            FormTitle.Foreground = new SolidColorBrush(RGBColors.color5);
            HelpLabel.Foreground = new SolidColorBrush(RGBColors.color5);
            FormIco.Source = new BitmapImage(new Uri(@"images/Help Color2.png", UriKind.Relative));
            if (mapview.started == true) { mapform.Pause(); }
            help = new HelpForm();
            PanelChildForm.Navigate(help);
        }

        /// <summary>
        /// We create an event when the content is rendered in the child form panel, and we set the navigation bar hidden
        /// </summary>
        private void PanelChildForm_ContentRendered(object sender, EventArgs e)
        {
            PanelChildForm.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
        }

        /// <summary>
        /// Get all file data and close all veiw/map tabs, so when we open them we refresh their data with the new data
        /// </summary>
        public void Getfichero(ReadFiles file)
        {
            this.Archivo = file;
            listaCAT10 = Archivo.GetListCAT10();
            listaCAT21v21 = Archivo.GetListCAT21v21();
            listaCAT21v23 = Archivo.GetListCAT21v23();
            listaCAT62 = Archivo.GetListCAT62();
            listaCATALL = Archivo.GetListCATALL();
            this.TableCat10 = Archivo.GetTablaCAT10();
            this.TableCat21v21 = Archivo.GetTablaCAT21v21();
            this.TableCat21v23 = Archivo.GetTablaCAT21v23();
            this.TableCat62 = Archivo.GetTablaCAT62();
            this.TableAll = Archivo.GetTablaAll();
            if (mapstarted == true) { mapform = new MapView(); mapstarted = false; }
            if (CAT10started == true) { viewCat10 = new View(); CAT10started = false; }
            if (CAT21v21started == true) { viewCat21= new View(); CAT21v21started = false; }
            if (CAT21v23started == true) { viewCat23 = new View(); CAT21v23started = false; }
            if (CAT62started == true) { viewCat62 = new View(); CAT62started = false; }

            if (CATAllstarted == true) { viewAll = new View(); CATAllstarted = false; }
        }

        /// <summary>
        /// When clicking on show on list in map page, map page calls this function to open the tables page and search for that message
        /// </summary>
        public void OpenFlightInList(CATALL message)
        {
            string Cat = message.CAT;
            if (Cat == "10")
            {
                activeSeecat10button();
                viewCat10 = new View();
                viewCat10.GetSearching(0, message);
                viewCat10.GetAll(listaCAT10, null, null,null, listaCATALL, TableCat10, null, null, null, null,TableCat10.Copy());
                viewCat10.GetForm(this);
                PanelChildForm.Navigate(viewCat10);
            }
            if (Cat == "21 v. 2.1")
            {
                activeSeeCat21button();
                viewCat21 = new View();
                viewCat21.GetSearching(1, message);
                viewCat21.GetAll(null, listaCAT21v21, null, null,listaCATALL,null, null, TableCat21v21,null,null, TableCat21v21.Copy());
                viewCat21.GetForm(this);
                PanelChildForm.Navigate(viewCat21);
            }
            if (Cat == "21 v. 0.23" || Cat=="21 v. 0.26")
            {
                activeSeecat23button(); 
                viewCat23 = new View();
                viewCat23.GetSearching(2, message);
                viewCat23.GetAll(null, null, listaCAT21v23,null, listaCATALL, null, TableCat21v23, null, null, null,TableCat21v23.Copy());
                viewCat23.GetForm(this);
                PanelChildForm.Navigate(viewCat23);
            }
            if (Cat == "62")
            {
                activeSeecat23button();
                viewCat23 = new View();
                viewCat23.GetSearching(4, message);
                viewCat23.GetAll(null, null, null, listaCAT62, listaCATALL, null, null, null, null,TableCat62, TableCat21v23.Copy());
                viewCat23.GetForm(this);
                PanelChildForm.Navigate(viewCat23);
            }
        }

        /// <summary>
        /// When clicking on show on map on tables pages, tables pages calls this function to open the map and search for that message
        /// </summary>
        public void OpenFlightInMap(CATALL flight)
        {
            if (listaCATALL.Exists(x => x.num == Convert.ToInt32(flight.num))) { }
            else { listaCATALL.Add(flight); }
            ActiveMapButton();
            mapform = new MapView();
            mapform.GetForm(this);
            mapform.GetList(listaCATALL,Archivo.AirportCodesList);
            mapform.SearchFlightInMap(flight);
            mapstarted = true;
           PanelChildForm.Navigate(mapform);
        }

        /// <summary>
        /// Event that moves the window by dragging the top bar
        /// </summary>
        private void PanelLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            base.OnMouseLeftButtonDown(e);
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                Point mousepos= Mouse.GetPosition(this);
                Application.Current.MainWindow.Left = System.Windows.Forms.Cursor.Position.X-mousepos.X;
                Application.Current.MainWindow.Top= System.Windows.Forms.Cursor.Position.Y-mousepos.Y;
            }
            this.DragMove();

        }


        /// <summary>
        /// Close app button
        /// </summary>
        private void Close_click(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            this.Close();
        }

        /// <summary>
        /// Maximize click button. If window is maximized returns to normal state, otherwise, its maximized
        /// </summary>
        private void MaximizeClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        //Minimizes the window
        private void Minimize_click(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


    }
}
