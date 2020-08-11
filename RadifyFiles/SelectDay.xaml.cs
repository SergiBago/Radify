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
using System.Windows.Shapes;

namespace PGTAWPF
{
    /// <summary>
    /// Lógica de interacción para SelectDay.xaml
    /// </summary>
    public partial class SelectDay : Window
    {
        public SelectDay()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        LoadFiles Form;

        private void Close_click(object sender, MouseButtonEventArgs e)
        {
            Form.GetParameters(-950400);
            this.Close();
        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            AlertVisibel(false);
            if (Days.SelectedItem==null || Position.SelectedItem== null ) { AlertVisibel(true); }
            else
            {
                int seconds = Convert.ToInt32(Days.SelectedValue.ToString()) * 86400;
                if (Convert.ToString(Position.SelectedValue.ToString()) == "Before") { seconds = -seconds; }
               // int code = GetAirporteCode(Convert.ToString(Airport.SelectedValue.ToString())); 
                Form.GetParameters(seconds);
                this.Close();
            }
        }

        public void GetForm( LoadFiles Form)
        {
            this.Form = Form;
        }


        private void AlertVisibel(bool i)
        {
            if (i==true)
            {
                AlertLabel.Visibility = Visibility.Visible;
                AlertIco.Visibility = Visibility.Visible;
            }
            else 
            {
                AlertLabel.Visibility = Visibility.Hidden;
                AlertIco.Visibility = Visibility.Hidden;
            }
        }

        private void LoadForm(object sender, RoutedEventArgs e)
        {
            AlertVisibel(false);
            this.Days.Items.Add("0");
            this.Days.Items.Add("1");
            this.Days.Items.Add("2");
            this.Days.Items.Add("3");
            this.Days.Items.Add("4");
            this.Days.Items.Add("5");
            this.Days.Items.Add("6");
            this.Days.Items.Add("7");
            this.Days.Items.Add("8");
            this.Days.Items.Add("9");
            this.Days.Items.Add("10");
            this.Position.Items.Add("Before"); 
            this.Position.Items.Add("After");
            //this.Airport.Items.Add("Barcelona");
            //this.Airport.Items.Add("Asturias");
            //this.Airport.Items.Add("Palma");
            //this.Airport.Items.Add("Santiago");
            //this.Airport.Items.Add("Barajas Norte");
            //this.Airport.Items.Add("Barajas Sud");
            //this.Airport.Items.Add("Tenerife");
            //this.Airport.Items.Add("Malaga");
        }

        private int GetAirporteCode(string name)
        {
            if (name=="Barcelona") { return 0; }
            else if (name == "Asturias") { return 1; }
            else if (name == "Palma") { return 2; }
            else if (name == "Santiago") { return 3; }
            else if (name == "Barajas Norte") { return 4; }
            else if (name == "Barajas Sud") { return 5; }
            else if (name == "Tenerife") { return 6; }
            else { return 7; }

        }

        private void DragPanel(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
