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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PGTAWPF
{
    /// <summary>
    /// Lógica de interacción para NoMessages.xaml
    /// </summary>
    public partial class NoMessages : Page
    {
        int type; //1 Cat 10, 2 cat 21v23, 3 cat 21v21, 4 cat all, 5 map
        MainWindow Form;

        public NoMessages()
        {
            InitializeComponent();
        }

        public void GetType(int i)
        {
            this.type = i;
        }

        public void GetForm(MainWindow form)
        {
            this.Form = form;
        }

        /// <summary>
        /// Depending on the selected tab we customize this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoMessages_load(object sender, RoutedEventArgs e)
        {
            if (type==1)
            {
                Title.Text = "Currently there are no messages of category 10 loaded.";
                Title.Foreground= new SolidColorBrush(Color.FromRgb(250, 248, 212));
                TryLoading.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                Image.Source = new BitmapImage(new Uri(@"images/NoMessagesCAT10.png", UriKind.Relative));
                Image.Height = 250;
            }
            if (type == 2)
            {
                Title.Text = "Currently there are no messages of category 21 v.0.23 or v 0.26 loaded."; 
                Title.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                TryLoading.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                Image.Source = new BitmapImage(new Uri(@"images/NoMessagesCAT21v23.png", UriKind.Relative));
                Image.Height = 300;
            }
            if (type == 3)
            {
                
                Title.Text = "Currently there are no messages of category 21 v.2.1 loaded.";
                Title.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                TryLoading.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                Image.Source = new BitmapImage(new Uri(@"images/NoMessagesCAT21v21.png", UriKind.Relative));
                Image.Height = 300;
            }
            if (type == 4)
            {
                Title.Text = "Currently there are no messages loaded.";
                Title.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                TryLoading.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                Image.Source = new BitmapImage(new Uri(@"images/NoMessagesCatAll.png", UriKind.Relative));
                Image.Height = 300;
            }
            if (type == 5)
            {
                Title.Text = "Currently there are no messages loaded.";
                Title.Foreground = new SolidColorBrush(Color.FromRgb(228, 187, 151));
                TryLoading.Foreground= new SolidColorBrush(Color.FromRgb(228, 187, 151));
                Image.Source = new BitmapImage(new Uri(@"images/SateliteMap.png", UriKind.Relative));
                Image.Height = 280;
            }
        }

        private void LoadFileClick(object sender, MouseButtonEventArgs e)
        {
            Form.OpenLoad();
        }

    }
}
