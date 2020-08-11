using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace PGTAWPF
{
    /// <summary>
    /// Lógica de interacción para Load.xaml
    /// </summary>
    public partial class Load : Page
    {
        Ficheros Archivo;
        MainWindow Form;
        DispatcherTimer timer = new DispatcherTimer();
        int result;

        public Load()
        {
            InitializeComponent();
        }

        private void Load_Load(object sender, RoutedEventArgs e)
        {
            Error(false);
            if (Archivo.numficheros == 0) { IsNoFiles(true); }
            else { IsNoFiles(false); }           
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            LoadFirst.Margin = new Thickness(0, 0, 0, 0);
            LoadFirst.Height = 180;

        }

        private void IsNoFiles(bool a)
        {
            if (a == true)
            {
                LoadFirst.Source= new BitmapImage(new Uri(@"Upload File.png", UriKind.Relative));
                TitleLab.Text = "Therere are no loaded files, plesase load one to start";
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

        private void Load_Click(object sender, MouseButtonEventArgs e)
        {
           // CheckForIllegalCrossThreadCalls = false;
            Error(false);
            string filePath;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //{
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt*|ast files (*.ast)|*.ast*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == true && openFileDialog.SafeFileName!=null)
                {
                    this.Startloading();
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        filePath = openFileDialog.FileName;
                        this.result = Archivo.Leer(filePath);
                        Stoploading();
                        if (result == 1)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                Form.Getfichero(Archivo);
                                this.Load_Load(sender, e);                 
                            });   
                        }
                        else { Error(true); }

                    }).Start();
                    

            }
            
        }

        public void GetForm(MainWindow Form)
        {
            this.Form = Form;
        }

        public void SetArchivo(Ficheros Archivo)
        {
            this.Archivo = Archivo;
        }

        private void Delete_Click(object sender, MouseButtonEventArgs e)
        {
            Archivo.ResetData();
            Form.Getfichero(Archivo);
            this.Load_Load(sender, e);
        }

        private void Startloading()
        {
            TitleLab.Text = "Loading";
            TitleLab.FontStyle = FontStyles.Italic;
            LoadFirst.Margin = new Thickness(0, 0, 0, 0);
            LoadFirst.Source = new BitmapImage(new Uri(@"Loading.png", UriKind.Relative));
            LoadSecond.Visibility = Visibility.Hidden;
            DeleteIcon.Visibility = Visibility.Hidden;
            LoadFirst.Visibility = Visibility.Visible;
            LoadFirst.Height = 150;
            timer.Start();
        }

        private void Stoploading()
        {
            timer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //TransformedBitmap TempImage = new TransformedBitmap();

            //TempImage.BeginInit();
            //TempImage.Source = new BitmapImage(new Uri(@"Loading.png", UriKind.Relative)); // MyImageSource of type BitmapImage
            //RotateTransform transform = new RotateTransform(90);
            //TempImage.Transform = transform;
            //TempImage.EndInit();
            //image1.Source = TempImage;
            //RotateTransform rotateTransform = new RotateTransform(1);
            //BitmapImage im = new Uri(@"Loading.png", UriKind.Relative);
            //im.RenderTransform = rotateTransform;
            //LoadFirst.Source = new BitmapImage(RenderTransform = rotateTransform;
        }
    }
}
