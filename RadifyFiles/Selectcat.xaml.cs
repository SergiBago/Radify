using PGTAWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PGTA_WPF
{
    /// <summary>
    /// Lógica de interacción para Selectcat.xaml
    /// </summary>
    public partial class Selectcat : Window
    {
        public Selectcat()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            AlertVisible(false);
            CheckBoxAll.IsChecked = true;
            CheckBoxCAT10.IsChecked = true;
            CheckBoxCAT21v21.IsChecked = true;
            CheckBoxCAT21v23.IsChecked = true;
            CheckBoxCAT62.IsChecked = true;
        }

        /// <summary>
        /// When clicking in load all, if checkbox load all is checked all others must be checked. If load all checkbox is unchecked others must be unchecked
        /// </summary>
        LoadFiles form;
        private void LoadAllCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxAll.IsChecked == true)
            {
                CheckBoxCAT10.IsChecked = true;
                CheckBoxCAT21v21.IsChecked = true;
                CheckBoxCAT21v23.IsChecked = true;
                CheckBoxCAT62.IsChecked = true;

            }
            else
            {
                CheckBoxCAT10.IsChecked = false;
                CheckBoxCAT21v21.IsChecked = false;
                CheckBoxCAT21v23.IsChecked = false;
                CheckBoxCAT62.IsChecked = false;

            }
        }

        /// <summary>
        /// If Load cat 10 is unchecked load all must be unchecked too. 
        /// </summary>
        private void LoadCAT10CheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxCAT10.IsChecked == false) {  
                CheckBoxAll.IsChecked = false;
            }
        }

        /// <summary>
        /// If Load cat 21 v. 0.23 is unchecked load all must be unchecked too. 
        /// </summary>
        private void LoadCAT21v23CheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxCAT21v23.IsChecked == false) { 
                CheckBoxAll.IsChecked = false;
            }
        }

        private void LoadCAT62CheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxCAT62.IsChecked == false)
            {
                CheckBoxAll.IsChecked = false;
            }
        }

        /// <summary>
        /// If Load cat 21 v. 2.1 is unchecked load all must be unchecked too. 
        /// </summary>
        private void LoadCAT21v21CheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (CheckBoxCAT21v21.IsChecked == false) { CheckBoxAll.IsChecked = false; }
        }

        /// <summary>
        /// Clicking on load button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadClick(object sender, RoutedEventArgs e)
        {
            AlertVisible(false);
            if (CheckBoxCAT10.IsChecked == false && CheckBoxCAT21v21.IsChecked == false && CheckBoxCAT21v23.IsChecked == false && CheckBoxCAT62.IsChecked == false) { //If all are unchecked throw an alert
                AlertVisible(true); 
            }
            else
            {
                /*We will create a list of booleans, where each boolean indicates
                 * whether or not that category should be loaded. The first position 
                 * corresponds to cat10, the second to cat 21 v. 0.23 and the third to cat 21 v. 2.1*/
                List<bool> selcats = new List<bool>(); 
                if (CheckBoxCAT10.IsChecked == true) { selcats.Add(true); }
                else { selcats.Add(false); }
                if (CheckBoxCAT21v23.IsChecked == true) { selcats.Add(true); }
                else { selcats.Add(false); }
                if (CheckBoxCAT21v21.IsChecked == true) { selcats.Add(true); }
                else { selcats.Add(false); }
                if (CheckBoxCAT62.IsChecked == true) { selcats.Add(true); }
                else { selcats.Add(false); }
                form.GetSelectedCats(selcats);
                this.Close();
            }
        }

        /// <summary>
        /// Shows or hides the alert message
        /// </summary>
        private void AlertVisible(bool i)
        {
            if (i == true)
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

        public void GetLoadForm(LoadFiles form)
        {
            this.form = form;
        }

        /// <summary>
        /// Drags the window on click and drag in the top bar
        /// </summary>
        private void TopBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void Close_click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }


    }
}
