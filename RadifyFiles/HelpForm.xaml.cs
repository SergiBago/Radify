using PGTA_WPF;
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

    public partial class HelpForm : Page
    {
        
        ViewHelp viewhelp = new ViewHelp();
        LoadHelp help = new LoadHelp();

        /// <summary>
        /// Start the page
        /// </summary>
        public HelpForm()
        {
            InitializeComponent();
            DeactivateButtons();
            MapHelpUnderBut.Visibility = Visibility.Visible;
            MapHelp maphelp = new MapHelp();
            MapHelpLabel.FontWeight = FontWeights.Bold;
            PanelChildForm.Navigate(maphelp);
        }

        /// <summary>
        /// Click on map help button
        /// </summary>
        private void MapHelpClick(object sender, MouseButtonEventArgs e)
        {
            DeactivateButtons();
            MapHelpUnderBut.Visibility = Visibility.Visible;
            MapHelpLabel.FontWeight = FontWeights.Bold;
            MapHelp maphelp = new MapHelp();
            PanelChildForm.Navigate(maphelp);
        }


        /// <summary>
        /// Click on List help button
        /// </summary>
        private void ListHelpClick(object sender, MouseButtonEventArgs e)
        {
            DeactivateButtons();
            ListHelpUnderBut.Visibility = Visibility.Visible;
            ListHelpLabel.FontWeight = FontWeights.Bold;
            PanelChildForm.Navigate(viewhelp);
        }

        /// <summary>
        /// Click on Load help button
        /// </summary>
        private void LoadHelpClick(object sender, MouseButtonEventArgs e)
        {
            DeactivateButtons();
            LoadHelpUnderBut.Visibility = Visibility.Visible;
            LoadHelpLabel.FontWeight = FontWeights.Bold;
            PanelChildForm.Navigate(help);
        }

        /// <summary>
        /// Deactivate all buttons
        /// </summary>
        private void DeactivateButtons()
        {
            MapHelpUnderBut.Visibility = Visibility.Hidden;
            ListHelpUnderBut.Visibility = Visibility.Hidden;
            LoadHelpUnderBut.Visibility = Visibility.Hidden;
            MapHelpLabel.FontWeight = FontWeights.Normal;
            ListHelpLabel.FontWeight = FontWeights.Normal;
            LoadHelpLabel.FontWeight = FontWeights.Normal;
        }
    }
}
