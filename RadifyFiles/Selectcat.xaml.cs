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

        }

        LoadFiles form;
        private void LoadAll(object sender, RoutedEventArgs e)
        {
            if (CheckBoxAll.IsChecked == true)
            {
                CheckBoxCAT10.IsChecked = true;
                CheckBoxCAT21v21.IsChecked = true;
                CheckBoxCAT21v23.IsChecked = true;
            }
            else
            {
                CheckBoxCAT10.IsChecked = false;
                CheckBoxCAT21v21.IsChecked = false;
                CheckBoxCAT21v23.IsChecked = false;
            }
        }

        private void LoadCAT10(object sender, RoutedEventArgs e)
        {
            if (CheckBoxCAT10.IsChecked == false) { CheckBoxAll.IsChecked = false; }
        }

        private void LoadCAT21v23(object sender, RoutedEventArgs e)
        {
            if (CheckBoxCAT21v23.IsChecked == false) { CheckBoxAll.IsChecked = false; }
        }

        private void LoadCAT21v21(object sender, RoutedEventArgs e)
        {
            if (CheckBoxCAT21v21.IsChecked == false) { CheckBoxAll.IsChecked = false; }
        }


        private void LoadClick(object sender, RoutedEventArgs e)
        {
            AlertVisible(false);
            if (CheckBoxCAT10.IsChecked == false && CheckBoxCAT21v21.IsChecked == false && CheckBoxCAT21v23.IsChecked == false) { AlertVisible(true); }
            else
            {
                List<bool> selcats = new List<bool>();
                if (CheckBoxCAT10.IsChecked == true) { selcats.Add(true); }
                else { selcats.Add(false); }
                if (CheckBoxCAT21v23.IsChecked == true) { selcats.Add(true); }
                else { selcats.Add(false); }
                if (CheckBoxCAT21v21.IsChecked == true) { selcats.Add(true); }
                else { selcats.Add(false); }
                form.GetSelectedCats(selcats);
                this.Close();
            }
        }

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

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
