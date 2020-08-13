using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Lógica de interacción para RepedetFileDialog.xaml
    /// </summary>
    public partial class RepedetFileDialog : Window
    {
        public RepedetFileDialog()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        LoadFiles form;

        public void GetLoadform(LoadFiles form)
        {
            this.form = form;
        }

        /// <summary>
        /// Cancel button click. It indicates to the load page that the file should not be loaded and closes this window
        /// </summary>
        private void Cancle_click(object sender, RoutedEventArgs e)
        {
            form.GetDuplicated(1);
            this.Close();
        }


        /// <summary>
        /// Load button click. It indicates to the load page that the file should be loaded and closes this window
        /// </summary>
        private void Load_Clicl(object sender, RoutedEventArgs e)
        {

            form.GetDuplicated(0);
            this.Close();
        }


        /// <summary>
        /// close button click. It indicates to the load page that the file should not be loaded and closes this window
        /// </summary>
        private void Close_Click(object sender, MouseButtonEventArgs e)
        {

            form.GetDuplicated(1);
            this.Close();
        }


        /// <summary>
        /// Drags the window on click and drag in the top bar
        /// </summary>
        private void MouseLeftButtonDownClick(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
