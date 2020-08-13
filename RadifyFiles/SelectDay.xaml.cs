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
        LoadFiles Form;

        public SelectDay()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Loads the window, starts the comboboxes
        /// </summary>
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
        }

 
        /// <summary>
        /// Closes the page. By returning the number -950400, indicates that the file must not be loaded, since user has not introduced a valid time parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_click(object sender, MouseButtonEventArgs e)
        {
            Form.GetParameters(-950400);
            this.Close();
        }

        /// <summary>
        /// Clicks on accept. COmputes the time, and sends it back to the load page. 
        /// </summary>
        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            AlertVisibel(false);
            if (Days.SelectedItem==null || Position.SelectedItem== null ) { AlertVisibel(true); } //If some of the parameters is not selected we throw an alert.
            else
            {
                int seconds = Convert.ToInt32(Days.SelectedValue.ToString()) * 86400;
                if (Convert.ToString(Position.SelectedValue.ToString()) == "Before") { seconds = -seconds; }
                Form.GetParameters(seconds);
                this.Close();
            }
        }

        public void GetForm( LoadFiles Form)
        {
            this.Form = Form;
        }

        /// <summary>
        /// Shows or hides the alert
        /// </summary>
        private void AlertVisibel(bool i)
        {
            if (i == true)
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

        /// <summary>
        /// Drags the window on drag in the top bar
        /// </summary>
        private void DragPanel(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
