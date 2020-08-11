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

namespace PGTA_WPF
{
    /// <summary>
    /// Lógica de interacción para ViewHelp.xaml
    /// </summary>
    public partial class ViewHelp : Page
    {
        public ViewHelp()
        {
            InitializeComponent();
        }

        private void ExportCSVClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "EXPORT CSV: This button allows you to export the table to a csv file so that you can see the data from another program, such as Microsoft Excel or Libre Office.";
        }

        private void ShowOnMapClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "SHOW ON MAP: Shows the selected message on the map. When accessing the map from this option, the map has this vehicle selected by default. Please note that there are messages that are not position reports, so not all messages can use this feature!";
        }

        private void SearchClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "SEARCH: Here you can search for a specific message or flight. If you enter only numbers the program automatically detects that you are looking for a message by its number and shows you that message and sorts the list by message number. If you enter letters and numbers the program detects that you are looking for a Target Identification and searches in that parameter the one that corresponds to your search. If it finds it, it orders the table in alphabetical order of Target Identification and shows you the messages that correspond to that identification. If you want to search by Track number you must write 'TR:' followed by the Track Number and the program will order the list by numerical order of Track Number and will show you the flights that correspond to your search. You can also search by Target Address, typing 'TA:' followed by the target adress. Also, the search is not case sensitive, so you can type the search both ways and the program will find it anyway!";
        }

        private void HelpClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "SHOW HELP: It shows you a very brief explanation on how to use the search engine";
        }

        private void TargetIDClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "TARGET ID: Shows the Target Id (Callsign) of the selected message.";
        }

        private void SelectedMessageClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "SELECTED MESSAGE: Shows the number of the selected message.";
        }

        private void TableClick(object sender, MouseButtonEventArgs e)
        {
            ExplanationLabel.Text = "TABLE: In the table you can see all the messages. By clicking on the header of any column you can sort the table automatically by that column, and in both directions, ascending and descending. In parameters where the information is large, the cells are compressed to take up less space. Clicking on any cell where it says 'Click to expand' the cell will expand to show all the information. Clicking on another cell the previous one will return to its original size. To select a message simply click on any of its cells, and you will see how the Selected Message and Target ID labels change.";
        }
    }
}
