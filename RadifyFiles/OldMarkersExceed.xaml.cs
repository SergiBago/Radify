using PGTAWPF;
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

namespace PGTA_WPF
{
    /// <summary>
    /// Lógica de interacción para OldMarkersExceed.xaml
    /// </summary>
    public partial class OldMarkersExceed : Window
    {
        public OldMarkersExceed()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        MapView form;
        string Hour;

        /// <summary>
        /// Window load. Sets the label indicating from which hour can we load
        /// </summary>
        private void FormLoad(object sender, RoutedEventArgs e)
        {
            Label.Text = "You entered a too big record.The selected history exceeds the limit of 50,000 bookmarks administered for the program. In order to not exceed this limit, we can load from " + Hour + ".Anyway, we remind you that the limit only applies to the complete history of all flights, and does not affect the single flight history, which is always complete. Do you want to calculate the history from " + Hour + "?";
        }

        /// <summary>
        /// Cancel button click. It indicates to the map page that the history should not be loaded and closes this window
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            form.getMarkerresult(false);
            this.Close();
        }

        /// <summary>
        /// Calculate button click. It indicates to the map page that the history should be loaded and closes this window
        /// </summary>
        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            form.getMarkerresult(true);
            this.Close();
        }

        /// <summary>
        /// Close button click. It indicates to the map page that the history should not be loaded and closes this window
        /// </summary>
        private void Close_click(object sender, MouseButtonEventArgs e)
        {
            form.getMarkerresult(false);
            this.Close();
        }

        /// <summary>
        /// Drags the window on click and drag in the top bar
        /// </summary>
        private void TopBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }


        public void GetForm (MapView form)
        {
            this.form= form;
        }

        public void GetHour(string Hour)
        {
            this.Hour = Hour;
        }
    }
}
