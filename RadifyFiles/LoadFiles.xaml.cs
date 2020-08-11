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
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Animation;
using PGTA_WPF;
using System.Windows.Threading;

namespace PGTAWPF
{
    /// <summary>
    /// Lógica de interacción para LoadFiles.xaml
    /// </summary>
    public partial class LoadFiles : Page
    {
        ReadFiles Archivo;
        MainWindow Form;
        int result;
        int seconds = 0;
        int duplicated = 0;
        List<bool> SelectedCats;
        bool correctcats = false;
        DispatcherTimer timer = new DispatcherTimer();

        public LoadFiles()
        {
            InitializeComponent();
            
            //Set Timer parameters (timer is used to update loading status label)
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += timer_Tick;

            // Create storyboard for rotating image when program is loading
            Image rotateImage = new Image()
            {
                Stretch = Stretch.Uniform,
                Source = new BitmapImage(new Uri(@"Loading.png", UriKind.Relative)),
                RenderTransform = new RotateTransform()

            };
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(10.0));
            DoubleAnimation rotateAnimation = new DoubleAnimation()
            {
                From = 0,
                To = 360,
                Duration = storyboard.Duration
            };

            Storyboard.SetTarget(rotateAnimation, rotateImage);
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            storyboard.Children.Add(rotateAnimation);
        }

        /// <summary>
        /// Start loading image rotation
        /// </summary>
        private void Rotateimage()
        {
            ((Storyboard)Resources["Storyboard"]).Begin();
        }

        /// <summary>
        /// Stop loading image rotation
        /// </summary>
        private void StopRotation()
        {
            ((Storyboard)Resources["Storyboard"]).Stop();
        }

        private void Load_click(object sender, MouseButtonEventArgs e)
        {
            load(sender, e);
        }

        private void LoadFirsClick(object sender, MouseButtonEventArgs e)
        {
            load(sender, e);
        }

        /// <summary>
        /// Start loading proces
        /// </summary>
        private void load(object sender, MouseButtonEventArgs e)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            correctcats = false;
            Error(false);
            string filePath;
            duplicated = 0;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt*|ast files (*.ast)|*.ast*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            bool repeted = false;
            bool CorrectParameters = true;
            if (openFileDialog.ShowDialog() == true && openFileDialog.SafeFileName != null) //Check if selected file is valid
            {
                foreach (string name in Archivo.names) //Check if selected file is already loaded
                {
                    if (name == openFileDialog.FileName)
                    {
                        repeted = true; 
                    }
                }
                if (repeted == true)
                {
                    RepedetFileDialog file = new RepedetFileDialog(); //If selected file is already loaded ask if we want to load it again
                    file.GetLoadform(this);
                    file.ShowDialog();
                }
                if (duplicated == 0) 
                {
                    if (Archivo.names.Count > 0) //If we have already any file loaded ask days to order files correctly
                    {
                        CorrectParameters = false;
                        SelectDay day = new SelectDay();
                        day.GetForm(this);
                        day.ShowDialog();
                        if (seconds != -950400)
                        {
                            CorrectParameters = true;
                        }
                    }

                    //Ask which categories to load
                    Selectcat selcat = new Selectcat();
                    selcat.GetLoadForm(this);
                    selcat.ShowDialog();


                    if (CorrectParameters == true && correctcats==true)
                    {
                        Form.LoadDisableButtons(); //Disable all buttons 
                        Startloading(); //set this page to loading page
                        new Thread(() => //load in a new thread so we can still do other things while loading
                        {
                            Thread.CurrentThread.IsBackground = true; 
                            filePath = openFileDialog.FileName; 
                          
                            this.result = Archivo.Leer(filePath, seconds,SelectedCats); //Load file in readfiles class 

                            if (result == 1) //if result 1 means everything went ok
                            {
                                this.Dispatcher.Invoke(() => //change this page controls 
                                {
                                    Form.Getfichero(Archivo); //sent load file to main window
                                    this.Stoploading(); //stop loading 
                                    this.Load_load(sender, e); //put this page in normal mode, not loading mode
                                    Form.LoadActiveButtons(); //Active all buttons
                                });
                            }
                            else //anything went wrong
                            {
                                this.Dispatcher.Invoke(() => 
                                {
                                    this.Load_load(sender, e); //put this page in normal mode, not loading mode
                                    this.Stoploading(); //stop loading
                                    Form.LoadActiveButtons(); //Active buttons
                                    Error(true); //show somwthing went wrong
                                });
                            }
                        }).Start(); //start thread
                    }
                }
            }
        }

        /// <summary>
        /// Get which selected cats sould be loaded
        /// </summary>
        /// <param name="Selectedcats">A list of bools indicating if each cat should be loaded</param>
        public void GetSelectedCats(List<bool> Selectedcats)
        {
            this.SelectedCats = Selectedcats;
            correctcats = true;
        }

        /// <summary>
        /// Delete all files click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_click(object sender, MouseButtonEventArgs e)
        {
            Archivo.ResetData(); //Clears all loaded data
            Form.Getfichero(Archivo);
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            this.Load_load(sender, e);

        }

        /// <summary>
        /// Load this page
        /// </summary>
        private void Load_load(object sender, RoutedEventArgs e)
        {
            Error(false);
            ProcessLabel.Visibility = Visibility.Hidden;
            
            RotateImage.Visibility = Visibility.Hidden;
            StopRotation();

            /*Depending on whether there is a file loaded or not,
             * the page configuration will be different*/
            if (Archivo.numficheros == 0) 
            { 
                LoadFirst.Visibility = Visibility.Visible;
                TitleLab.Text = "Therere are no loaded files, plesase load one to start!";
                TitleLab.FontStyle = FontStyles.Normal;
                LoadSecond.Visibility = Visibility.Hidden;
                LoadFirst.Visibility = Visibility.Visible;
                DeleteIcon.Visibility = Visibility.Hidden;
                FilesList.Visibility = Visibility.Hidden;
            } 
            else
            {
                string filenames = "";
                foreach (string name in Archivo.names)
                {
                    string[] parts = name.Split('\\');
                    filenames = filenames + "-" + parts[parts.Length - 1] + "\n";
                }
                TitleLab.Text = "Actually, there are these files loaded: ";
                TitleLab.FontStyle = FontStyles.Normal;
                FilesList.Text = filenames;
                FilesList.Visibility = Visibility.Visible;
                LoadSecond.Visibility = Visibility.Visible;
                LoadFirst.Visibility = Visibility.Hidden;
                DeleteIcon.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Function that shows some error ocurred
        /// </summary>
        private void Error(bool a)
        {
            if (a == true)
            {
                ErrorIcon.Visibility = Visibility.Visible;
                ErrorLabel.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorIcon.Visibility = Visibility.Hidden;
                ErrorLabel.Visibility = Visibility.Hidden;
            }
        }


        public void GetDuplicated(int i)
        {
            this.duplicated = i;
        }

        public void GetParameters(int time)
        {
            this.seconds = time;
        }

        public void GetForm(MainWindow Form)
        {
            this.Form = Form;
        }

        public void SetArchivo(ReadFiles Archivo)
        {
            this.Archivo = Archivo;
        }

        /// <summary>
        /// Puts this page into loading configuration
        /// </summary>
        private void Startloading()
        {
            TitleLab.Text = "Loading";
            timer.Start();
            ProcessLabel.Visibility = Visibility.Visible;
            TitleLab.FontStyle = FontStyles.Italic;
            LoadSecond.Visibility = Visibility.Hidden;
            DeleteIcon.Visibility = Visibility.Hidden;
            LoadFirst.Visibility = Visibility.Hidden;
            FilesList.Visibility = Visibility.Hidden;
            RotateImage.Visibility = Visibility.Visible;
            Rotateimage();
        }
        

        /// <summary>
        /// on every timer tick loading progress label updates
        /// </summary>
        void timer_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ProcessLabel.Text = Archivo.process;
            });
        }

        /// <summary>
        /// Stops loading
        /// </summary>
        private void Stoploading()
        {
            StopRotation();
            timer.Stop();
        }
    }
}
