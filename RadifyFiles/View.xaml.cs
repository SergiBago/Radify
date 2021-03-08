using System;
using System.Data;
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
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;
//using System.Windows.Forms;

namespace PGTAWPF
{
    /// <summary>
    /// Lógica de interacción para View.xaml
    /// </summary>
    public partial class View : Page
    {
        bool HelpLabelVisible = false;
        List<CAT10> listaCAT10 = new List<CAT10>();
        List<CAT21vs21> listaCAT21v21 = new List<CAT21vs21>();
        List<CAT21vs23> listaCAT21v23 = new List<CAT21vs23>();
        List<CAT62> listaCAT62 = new List<CAT62>();
        List<CATALL> listaCATALL = new List<CATALL>();
        DataTable TableCat10 = new DataTable();
        DataTable TableCat21v21 = new DataTable();
        DataTable TableCat21v23 = new DataTable();
        DataTable TableCat62 = new DataTable();
        DataTable TableAll = new DataTable();
        DataTable Datatable = new DataTable();
        MainWindow Form;
        int type; //0=cat 10, 1=cat21v21, 2=cat21v23, 3=all
        CATALL SearchingMessage = new CATALL();
        bool searching = false;
        bool cell = false;
        bool started = false;


        public View()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get all data
        /// </summary>
        public void GetAll(List<CAT10> list10, List<CAT21vs21> list21v21, List<CAT21vs23> list21v23, List<CAT62> list62, List<CATALL> listCATAll, DataTable tableCat10, DataTable tableCat21v23, DataTable tableCat21v21,DataTable tableCat62, DataTable tableCatAll, DataTable table)
        {
            this.listaCAT10 = list10;
            this.listaCAT21v21 = list21v21;
            this.listaCAT21v23 = list21v23;
            this.listaCAT62 = listaCAT62;
            this.listaCATALL = listCATAll;
            this.Datatable = table;
            this.Datatable.Columns.RemoveAt(1);
            this.TableCat10 = tableCat10;
            this.TableCat21v23 = tableCat21v23;
            this.TableCat21v21 = tableCat21v21;
            this.TableCat62 = tableCat62;
            this.TableAll = tableCatAll;
            started = false;
        }

        /// <summary>
        /// Gets data type (know if we are looking cat 10, 21 v2.1 21 v0.23 or all)
        /// </summary>
        /// <param name="i">int indicating which cat we are looking at (0=cat 10, 1=cat21v21, 2=cat21v23, 3=all, 4= cat62)</param>
        public void GetType(int i)
        {
            this.type = i;
        }

        public void GetForm(MainWindow Form)
        {
            this.Form = Form;
        }

        /// <summary>
        /// Stablishes if we are looking for a message or not.
        /// </summary>
        /// <param name="i">int indicating which cat we are looking at (0=cat 10, 1=cat21v21, 2=cat21v23, 3=all, 4=cat62)</param>
        /// <param name="message">message we are looking for</param>
        public void GetSearching(int i, CATALL message)
        {
            this.type = i;
            this.SearchingMessage = message;
            searching = true;
        }

        /// <summary>
        /// Page load
        /// </summary>
        private void View_loaded(object sender, RoutedEventArgs e)
        {
            if (started == false) //If page is already started we can just show the page and avoid all this
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; 

                //Hides all alerts
                Alertvisible(false);
                AlertNoMessageSelected(false);
                AlertMessageNotValid(false);
                WaterMarkActive();
                SeeHelp(false);

                //Stablished the datagrid source
                DatagridView.ItemsSource = Datatable.DefaultView;
                DatagridView.DataContext = Datatable;
                DatagridView.Items.Refresh();

                //Adjusts all columns to content width
                foreach (DataGridColumn col in DatagridView.Columns)
                {
                    col.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
                }


                this.DatagridView.CanUserResizeColumns = false;
                this.DatagridView.CanUserResizeRows = false;
                DatagridView.IsReadOnly = true;

                //If page openmed while searching a flight from a map we search for that flight
                if (searching == true)
                {
                    Searchflightfrommap(SearchingMessage);
                }

                /*Calculate the width that the table should have and if it is smaller than the width 
                 * of the page it establishes it. In this way the table has the necessary width,
                 * and not the entire page, which would leave very large cells */
                double tableWidth = 0;
                for (int i = 0; i < DatagridView.Columns.Count(); i++)
                {
                    tableWidth += DatagridView.Columns[i].ActualWidth;
                }
                double pagewidth = Grid.Width;
                tableWidth += 20;

                if (pagewidth > tableWidth)
                {
                    DatagridView.Width = tableWidth;
                }
                started = true;
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// When textbox gots focus we disables the textbox watermark
        /// </summary>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            WaterMarkDisabled();
        }

        /// <summary>
        /// When textbox losts focus we activate the watermark
        /// </summary>
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            search = SearchBox.Text; // We save the text entered in the textbox by the user, since the watermark will change it to another
            WaterMarkActive();
        }

        string search;
        private void WaterMarkActive()
        {
            search = SearchBox.Text; // We save the text entered in the textbox by the user, since the watermark will change it to another
            this.SearchBox.Text = "Ex: VLG22PPY";
            this.SearchBox.Foreground =  new SolidColorBrush(Color.FromRgb(60, 60, 60));
            Alertvisible(false);
        }


        private void WaterMarkDisabled()
        {

            this.SearchBox.Text = null;
            this.SearchBox.Foreground = new SolidColorBrush(Color.FromRgb(10,10,10));
        }

        /// <summary>
        /// When pressing a key we check if it's enter key. If its, we search
        /// </summary>
        private void EnterPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Functionsearch(SearchBox.Text);
            }
        }

        /// <summary>
        /// When clicking in search button we call the search funcion
        /// </summary>
        private void SearchButton_click(object sender, MouseButtonEventArgs e)
        {
            search = SearchBox.Text;
            this.Functionsearch(search);
        }


        /// <summary>
        /// Search for a message
        /// </summary>
        /// <param name="search"> parameter we are searching</param>
        private void Functionsearch(string search)
        {
            Alertvisible(false);
            bool found = false;
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            try
            {
                int searchnum = Convert.ToInt32(search); //Try converting the search string to an int, if possible, we will probably be looking for a message number
                for (int s = 0; s < DatagridView.Items.Count; s++)
                {
                    if (searchnum == Convert.ToInt32((DatagridView.Items[s] as DataRowView).Row.ItemArray[0])) 
                    {
                        SortDataGrid(DatagridView, 0, ListSortDirection.Ascending); //If found, we order the table for the number column
                        for (int i = 0; i < DatagridView.Items.Count; i++) 
                        {
                            if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == searchnum)
                            {
                                /*We deselect all the messages that may be selected, we select the message, 
                                 * we change the text that indicates the number and ID of the message,
                                 * we move the table so that the selected message is displayed first and
                                 * we stop going through the table and search*/
                                DatagridView.UnselectAll();
                                DatagridView.SelectedItem = DatagridView.Items[i];
                                SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                                SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);

                                /*We move the table first to the last message and then to the one found because this way we make
                                 * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                                DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count-1], DatagridView.Columns[0]); 
                                DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);
                                i = DatagridView.Items.Count;
                                s = DatagridView.Items.Count;
                                found = true;
                            }
                        }
                    }
                }
                if (found == false) { Alertvisible(true); }
            }
            catch
            {
                try
                {
                    if (search.Count() > 3 && search.Substring(0, 3).ToUpper() == "TR:") //We check if we are looking for a track number
                    {
                        if (type == 2) { Alertvisible(true); found = true; } //If we are in type 2 and searching for a track number we throw an alert, so type 2 messages doesn't have track number
                        else
                        {
                            search = search.Substring(3, (search.Length - 3)); //If searching for a track number, we delete the "TR:" from the search string 
                            for (int s = 0; s < DatagridView.Items.Count; s++) 
                            {
                                try
                                {
                                    if (Convert.ToInt32(search) == Convert.ToInt32((DatagridView.Items[s] as DataRowView).Row.ItemArray[5]))
                                    {
                                        int num = Convert.ToInt32((DatagridView.Items[s] as DataRowView).Row.ItemArray[0]);
                                        SortDataGrid(DatagridView, 5, ListSortDirection.Descending);
                                        for (int i = 0; i < DatagridView.Items.Count; i++)
                                        {
                                            if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == num)
                                            {

                                                /*We deselect all the messages that may be selected, we select the message, 
                                                 * we change the text that indicates the number and ID of the message,
                                                 * we move the table so that the selected message is displayed first and
                                                 * we stop going through the table and search*/
                                                DatagridView.UnselectAll();
                                                DatagridView.SelectedItem = DatagridView.Items[i];
                                                SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                                                SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);

                                                /*We move the table first to the last message and then to the one found because this way we make
                                                 * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                                                DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count - 1], DatagridView.Columns[0]);
                                                DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);

                                                i = DatagridView.Items.Count;
                                                s = DatagridView.Items.Count;
                                                found = true;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                            if (found == false) { Alertvisible(true); }
                        }
                    }
                    else if (search.Count() >3 && search.Substring(0,3).ToUpper()=="TA:") // We check if we are looking for a target address
                    {
                        /*Depending on which category we are in, we will have to search in one column or another*/
                        int col = 0;
                        if (type==0) { col = 17; }
                        else if (type==1) { col = 13; }
                        else if (type ==2) { col = 14; }
                        else { col = 6; }

                        search = search.Substring(3, (search.Length - 3)).ToUpper(); //If searching for a target address, we delete the "TA:" from the search string 

                        for (int s = 0; s < DatagridView.Items.Count; s++)
                        {
                            try
                            {
                                if (search == Convert.ToString((DatagridView.Items[s] as DataRowView).Row.ItemArray[col]))
                                {
                                    int num = Convert.ToInt32((DatagridView.Items[s] as DataRowView).Row.ItemArray[0]);
                                    SortDataGrid(DatagridView, col, ListSortDirection.Descending);
                                    for (int i = 0; i < DatagridView.Items.Count; i++)
                                    {
                                        if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == num)
                                        {
                                            /*We deselect all the messages that may be selected, we select the message, 
                                             * we change the text that indicates the number and ID of the message,
                                             * we move the table so that the selected message is displayed first and
                                             * we stop going through the table and search*/
                                            DatagridView.UnselectAll();
                                            DatagridView.SelectedItem = DatagridView.Items[i];
                                            SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                                            SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);

                                            /*We move the table first to the last message and then to the one found because this way we make
                                             * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                                            DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count - 1], DatagridView.Columns[0]);
                                            DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);
                                            i = DatagridView.Items.Count;
                                            s = DatagridView.Items.Count;
                                            found = true;
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                        if (found == false) { Alertvisible(true); }

                    }
                    else
                    {
                        if (found == false)
                        {
                            for (int s = 0; s < DatagridView.Items.Count; s++)
                            {
                                if (search.ToUpper() == Convert.ToString((DatagridView.Items[s] as DataRowView).Row.ItemArray[4]))
                                {
                                    int num = Convert.ToInt32((DatagridView.Items[s] as DataRowView).Row.ItemArray[0]);
                                    SortDataGrid(DatagridView, 4, ListSortDirection.Ascending);
                                    for (int i = 0; i < DatagridView.Items.Count; i++)
                                    {
                                        if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == num)
                                        {
                                            /*We deselect all the messages that may be selected, we select the message, 
                                             * we change the text that indicates the number and ID of the message,
                                             * we move the table so that the selected message is displayed first and
                                             * we stop going through the table and search*/
                                            DatagridView.UnselectAll();
                                            DatagridView.SelectedItem = DatagridView.Items[i];
                                            SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                                            SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);

                                            /*We move the table first to the last message and then to the one found because this way we make
                                             * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                                            DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count - 1], DatagridView.Columns[0]);
                                            DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);
                                            i = DatagridView.Items.Count;
                                            s = DatagridView.Items.Count;
                                            found = true;
                                        }
                                    }
                                }
                            }
                            if (found == false) { Alertvisible(true); }
                        }
                    }                    
                }
                catch { Alertvisible(true); }
            }
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Function to search for a flight from the map
        /// </summary>
        /// <param name="message">message we are looking for</param>
        private void Searchflightfrommap(CATALL message)
        {

            if (message.Target_Identification != null)  //If message target identification is not null, we search the message for target identification
            {
                SortDataGrid(DatagridView,4 , ListSortDirection.Ascending);
                DatagridView.UnselectAll();
                for (int i = 0; i < DatagridView.Items.Count; i++)
                {
                    if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == message.num)
                    {
                        /*We deselect all the messages that may be selected, we select the message, 
                         * we change the text that indicates the number and ID of the message,
                         * we move the table so that the selected message is displayed first and
                         * we stop going through the table and search*/
                        DatagridView.SelectedItem= DatagridView.Items[i];
                        SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                        SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);


                        /*We move the table first to the last message and then to the one found because this way we make
                         * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                        DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count - 1], DatagridView.Columns[0]);
                        DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);
                        i = DatagridView.Items.Count;
                    }
                }

            }
            else if (message.Target_Address != null) //If target identification is null but target adress is no, we search the message for target address
            {

                int col = 0;
                if (type == 0) { col = 17; }
                else if (type == 1) { col = 13; }
                else if (type == 2) { col = 14; }
                else { col = 6; }

                SortDataGrid(DatagridView, col, ListSortDirection.Descending);
                DatagridView.UnselectAll();
                for (int i = 0; i < DatagridView.Items.Count; i++)
                {
                    if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == message.num)
                    {
                        /*We deselect all the messages that may be selected, we select the message, 
                         * we change the text that indicates the number and ID of the message,
                         * we move the table so that the selected message is displayed first and
                         * we stop going through the table and search*/
                        DatagridView.SelectedItem = DatagridView.Items[i];
                        SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                        SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);

                        /*We move the table first to the last message and then to the one found because this way we make
                         * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                        DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count - 1], DatagridView.Columns[0]);
                        DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);
                        i = DatagridView.Items.Count;
                    }
                }
            }
            else if (message.Track_number != null) //If target identification and target address are null but track number is no, we search the flight for track number
            {
                if (type != 2)
                {
                    SortDataGrid(DatagridView, 5, ListSortDirection.Descending);
                    DatagridView.UnselectAll();
                    for (int i = 0; i < DatagridView.Items.Count; i++)
                    {
                        if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == message.num)
                        {
                            /*We deselect all the messages that may be selected, we select the message, 
                             * we change the text that indicates the number and ID of the message,
                             * we move the table so that the selected message is displayed first and
                             * we stop going through the table and search*/
                            DatagridView.SelectedItem = DatagridView.Items[i];
                            SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                            SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);

                            /*We move the table first to the last message and then to the one found because this way we make
                             * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                            DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count - 1], DatagridView.Columns[0]);
                            DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);
                            i = DatagridView.Items.Count;
                        }
                    }
                }
            }
            else //If target address, target identification and track number are null, we search the message for number
            {
                SortDataGrid(DatagridView, 0, ListSortDirection.Descending);
                DatagridView.UnselectAll();
                for (int i = 0; i < DatagridView.Items.Count; i++)
                {
                    if (Convert.ToInt32((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]) == message.num)
                    {
                        /*We deselect all the messages that may be selected, we select the message, 
                         * we change the text that indicates the number and ID of the message,
                         * we move the table so that the selected message is displayed first and
                         * we stop going through the table and search*/
                        DatagridView.SelectedItem = DatagridView.Items[i];
                        SelectedNumber.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[0]);
                        SelectedID.Text = Convert.ToString((DatagridView.Items[i] as DataRowView).Row.ItemArray[4]);

                        /*We move the table first to the last message and then to the one found because this way we make
                         * sure that the message we are looking for is displayed in the first position of the table, and not in the last.*/
                        DatagridView.ScrollIntoView(DatagridView.Items[DatagridView.Items.Count - 1], DatagridView.Columns[0]);
                        DatagridView.ScrollIntoView(DatagridView.Items[i], DatagridView.Columns[0]);
                        i = DatagridView.Items.Count;
                    }
                }
            }
        }

        /// <summary>
        /// Sort de dataGrid
        /// </summary>
        /// <param name="dataGrid"> Datagrid to sort</param>
        /// <param name="columnIndex">Index of the column we want to sort by</param>
        /// <param name="sortDirection">direction of sorting</param>
        public void SortDataGrid(DataGrid dataGrid, int columnIndex , ListSortDirection sortDirection)
        {
            var column = dataGrid.Columns[columnIndex];
            dataGrid.Items.SortDescriptions.Clear();
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;
            DatagridView.UpdateLayout();
        }


        private void Help_Click(object sender, MouseButtonEventArgs e)
        {
            SeeHelp(true);
        }

        private void CloseHelp_click(object sender, MouseButtonEventArgs e)
        {
            SeeHelp(false);
        }

        /// <summary>
        /// Shows or hides the time help
        /// </summary>
        private void SeeHelp(bool a)
        {
            if (a== true)
            {
                HelpLabel.Text = "You can search by flight number or ID directly. If you want to search by the track number enter 'TR:' followed by the number. If you want to search by Target Adress enter 'TA:' followed by the address.";
                HelpLabelVisible = true;
                HelpLabel.Visibility = Visibility.Visible;
                MessageNotValidLabel.Visibility = Visibility.Hidden;
                AlertLabelIco.Visibility = Visibility.Hidden;
                CloseHelpIco.Visibility = Visibility.Visible;
                UnderBorder12.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                HelpRow.Height = new GridLength(1, GridUnitType.Auto);

            }
            else
            {
                HelpLabel.Text = "";
                HelpRow.Height = new GridLength(0);
                HelpLabelVisible = false;
                UnderBorder12.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            }
        }


        int Datarow;
        int Datacol;
        DataGridLength Columnwith;
        /// <summary>
        /// When clicking on a cell, if cell content is "Click to expand" expands the cell with the rest of the information. Also, if there was an extended cell, contracts it
        /// </summary>
        private void CellContent_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                AlertMessageNotValid(false);
                AlertNoMessageSelected(false);
                DataGridCellInfo selcell = DatagridView.CurrentCell;
                int columnindex = selcell.Column.DisplayIndex;
                int rowIndex = DatagridView.Items.IndexOf(selcell.Item);
                SelectedNumber.Text = Convert.ToString((DatagridView.Items[rowIndex] as DataRowView).Row.ItemArray[0]);
                SelectedID.Text = Convert.ToString((DatagridView.Items[rowIndex] as DataRowView).Row.ItemArray[4]);

                if (cell == true) //If there was an extended cell, changes it value to "Click to expand", and contracts it
                {
                    cell = false;
                    DataRowView rowView0 = (DatagridView.Items[Datarow] as DataRowView); 
                    rowView0.BeginEdit();
                    rowView0[Datacol] = "Click to expand";
                    rowView0.EndEdit();
                    DatagridView.Columns[Datacol].Width = Columnwith;
                }
                if (Convert.ToString((DatagridView.Items[rowIndex] as DataRowView).Row.ItemArray[columnindex]) == "Click to expand") //If selected cell value is "Click to expand" change it's value with all the data of that parameter, and expands the cell
                {
                    Datacol = columnindex;
                    Datarow = rowIndex;
                    Columnwith =DatagridView.Columns[Datacol].Width.DisplayValue;
                    string value = this.GetValues(columnindex, rowIndex); //Get all parameters value
                    DataRowView rowView = (DatagridView.Items[Datarow] as DataRowView); 
                    rowView.BeginEdit();
                    rowView[Datacol] = value;
                    rowView.EndEdit();
                    cell = true;
                }

                DatagridView.Columns[columnindex].Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto); //Resizes column to fit new content
                DatagridView.UpdateLayout();
            }
            catch { }
        }

        /// <summary>
        /// Given the row and the column of the selected cell, returns an string with the full parameters
        /// </summary>
        /// <param name="i">Column index</param>
        /// <param name="e">row index</param>
        /// <returns>string with the full parameters</returns>
        private string GetValues(int i, int e)
        {
            int num = Convert.ToInt32((DatagridView.Items[e] as DataRowView).Row.ItemArray[0]);

            if (type == 0)
            {
                CAT10 message = new CAT10();
                foreach (CAT10 mes in listaCAT10) { if (mes.num == num) { message = mes; }; }
                string nl = Environment.NewLine;
                string Value = "";
                if (i == 6)
                {
                    Value = "TYP: " + message.TYP + nl + message.DCR + nl + message.CHN + nl + message.GBS + nl + message.CRT;
                    if (message.SIM != null) { Value = Value + nl + message.SIM + nl + message.TST + nl + message.RAB + nl + message.LOP + nl + message.TOT; }
                    if (message.SPI != null) { Value = Value + nl + message.SPI; }
                }
                if (i == 10)
                {
                    Value = Value + message.CNF + nl + message.TRE + nl + message.CST + nl + message.MAH + nl + message.TCC + nl + message.STH;
                    if (message.TOM != null) { Value = Value + nl + message.TOM + nl + message.DOU + nl + message.MRS; }
                    if (message.GHO != null) { Value = Value + nl + message.GHO; }
                }
                if (i == 18)
                {
                    Value = message.NOGO + nl + message.OVL + nl + message.TSV + nl + message.DIV + nl + message.TIF;
                }

                if (i == 22)
                {
                    Value = message.V_Mode_3A + nl + message.G_Mode_3A + nl + message.L_Mode_3A + nl + message.Mode_3A;
                }
                if (i == 23)
                {
                    Value = Value + "Repetitions: " + message.modeS_rep;
                    for (int s = 0; s < message.modeS_rep; s++)
                    {
                        Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + "Mode S Comm B message data: " + message.MB_Data[s] + nl + "Comm B Data Buffer Store 1 Address: " + message.BDS1[s] + nl + "Comm B Data Buffer Store 2 Address: " + message.BDS2[s];
                    }
                }

                if (i == 24)
                {
                    Value = message.Deviation_X + nl + message.Deviation_Y + nl + message.Covariance_XY;
                }
                if (i == 25)
                {
                    Value = Value + "Repetitions: " + Convert.ToString(message.REP_Presence);
                    for (int s = 0; s < message.REP_Presence; s++)
                    {
                        Value = Value + nl + "Difference between the radial distance of the plot centre and that of the presence: " + message.DRHO[s] + nl + "Difference between the azimuth of the plot centre and that of the presence: " + message.DTHETA[s];
                    }
                }


                return Value;
            }
            else if (type == 1)
            {
                CAT21vs21 message = new CAT21vs21();
                foreach (CAT21vs21 mes in listaCAT21v21) { if (mes.num == num) { message = mes; }; }
                string nl = Environment.NewLine;
                string Value = "";
                if (i == 6)
                {
                    Value = " Address Type: " + message.ATP + nl + "Altitude Reporting Capability: " + message.ARC + nl + "Range Check: " + message.RC + nl + "Report Type: " + message.RAB;
                    if (message.DCR != null)
                    {
                        Value = Value + nl + "Differential Correction: " + message.DCR + nl + "Ground Bit Setting: " + message.GBS + nl + "Simulated Target: " + message.SIM + nl + "Test Target: " + message.TST + nl + "Selected Altitude Available: " + message.SAA + nl + "Confidence Level: " + message.CL;
                        if (message.IPC != null)
                        {
                            Value = Value + nl + "Independent Position Check: " + message.IPC + nl + "No-go Bit Status: " + message.NOGO + nl + "Compact Posiotion Reporting: " + message.CPR + nl + "Local Decoding Position Jump: " + message.LDPJ + nl + "Range Check: " + message.RCF;
                        }
                    }
                }
                if (i == 19)
                {
                    Value = "NUCr or NACv: " + message.NUCr_NACv + nl + "NUCp or NIC: " + message.NUCp_NIC;
                    if (message.NICbaro != null)
                    {
                        Value = Value + nl + "Navigation Integrity Category for Barometric Altitude: " + message.NICbaro + nl + "Surveillance or Source  Integrity Level: " + message.SIL + nl + "Navigation Accuracy Category for Position: " + message.NACp;
                        if (message.SILS != null)
                        {
                            Value = Value + nl + "SIL-Supplement: " + message.SILS + nl + "Horizontal Position System Design Assurance Level: " + message.SDA + nl + "Geometric Altitude Accuracy: " + message.GVA;
                            if (message.ICB != null) { Value = Value + nl + "Position Integrity Category:" + Convert.ToString(message.PIC)+ nl + "Integrity Containment Bound" + message.ICB + nl + "NUCp: " + message.NUCp + nl + "NIC: " + message.NIC; }
                        }
                    }
                }
                if (i == 25)
                {
                    Value = "Intent Change Flag: " + message.ICF + nl + "LNAV Mode: " + message.LNAV + nl + "Priority Status: " + message.PS + nl + "Surveillance Status: " + message.SS;
                }
                if (i == 31)
                {
                    if (message.Wind_Speed != null) { Value = Value + "Wind Speed: " + message.Wind_Speed; }
                    if (message.Wind_Direction != null) { Value = Value + nl + "Wind Direction: " + message.Wind_Direction; }
                    if (message.Temperature != null) { Value = Value + nl + "Temperature: " + message.Temperature; }
                    if (message.Turbulence != null) { Value = Value + nl + "Turbulence: " + message.Turbulence; }
                }
                if (i == 33)
                {
                    Value = Value + "Manage Vertical Mode: " + message.MV + nl + "Altitude Hold Mode: " + message.AH + "Approach Mode: " + message.AM + nl + "Altitude: " + message.Final_State_Altitude;
                }
                if (i == 34)
                {
                    if (message.NAV != null)
                    {
                        Value = Value + message.NAV + nl + message.NVB;
                    }
                    if (message.REP != 0)
                    {
                        Value = Value + nl + "Repetitions: " + Convert.ToString(message.REP);
                        for (int s = 0; s < message.REP; s++)
                        {
                            Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + message.TCA[s] + nl + message.NC[s] + nl + "Trajectory Change Point number: " + message.TCP[s] + nl + "Altitude: " + message.Altitude[s] + nl + "Latitude: " + message.Latitude[s] + nl + "Longitude: " + message.Longitude[s] + nl + "Point Type: " + message.Point_Type[s] + nl;
                            Value = Value + nl + "TD: " + message.TD[s] + nl + "Turn Radius Availabilty" + message.TRA[s] + nl + message.TOA[s] + nl + "Time Over Point: " + message.TOV[s] + nl + "TCP Turn radius: " + message.TTR[s];
                        }
                    }
                }
                if (i == 36)
                {
                    Value = Value + message.RA + nl + "Target Trajectory Change Report Capability: " + message.TC + nl + "Target State Report Capability: " + message.TS + nl + "Air-Referenced Velocity Report Capability: " + message.ARV + nl + "Cockpit Display of Traffic Information airborne: " + message.CDTIA + nl + "TCAS System Status: " + message.Not_TCAS + nl + message.SA;
                }
                if (i == 37)
                {
                    Value = Value + message.POA + nl + message.CDTIS + nl + message.B2_low + nl + message.RAS + nl + message.IDENT;
                    if (message.LengthandWidth != null) { Value = Value + nl + message.LengthandWidth; }
                }
                if (i == 39)
                {
                    Value = Value + "Repetitions: " + message.modeS_rep;
                    for (int s = 0; s < message.modeS_rep; s++)
                    {
                        Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + "Mode S Comm B message data: " + message.MB_Data[s] + nl + "Comm B Data Buffer Store 1 Address: " + message.BDS1[s] + nl + "Comm B Data Buffer Store 2 Address: " + message.BDS2[s];
                    }
                }
                if (i == 40)
                {
                    Value = Value + "Message Type: " + message.TYP + nl + "Message Sub-type: " + message.STYP + nl + "Active Resolution Advisories: " + message.ARA + nl + "RAC(RA Complement) Record: " + message.RAC + nl + "RA Terminated: " + message.RAT + nl + "Multiple Threat Encounter: " + message.MTE + nl + "Threat Type Indicator: " + message.TTI + nl + "Threat Identity Data: " + message.TID;
                }
                if (i == 42)
                {
                    if (message.AOS != null) { Value = Value + "Age of the latest received information transmitted in item I021 / 008: " + message.AOS; }
                    if (message.TRD != null) { Value = Value + nl + "Age of the last update of the Target Report Descriptor: " + message.TRD; }
                    if (message.M3A != null) { Value = Value + nl + "Age of the last update of the Mode 3 / A Code: " + message.M3A; }
                    if (message.QI != null) { Value = Value + nl + "Age of the latest information received to update the Quality Indicators: " + message.QI; }
                    if (message.TI != null) { Value = Value + nl + "Age of the last update of the Trajectory Intent: " + message.TI; }
                    if (message.MAM != null) { Value = Value + nl + "Age of the latest measurement of the message amplitude: " + message.MAM; }
                    if (message.GH != null) { Value = Value + nl + "Age of the information contained in item 021 / 140: " + message.GH; }
                    if (message.FL != null) { Value = Value + nl + "Age of the Flight Level information: " + message.FL; }
                    if (message.ISA != null) { Value = Value + nl + "Age of the Intermediate State Selected Altitude: " + message.ISA; }
                    if (message.FSA != null) { Value = Value + nl + "Age of the Final State Selected Altitude: " + message.FSA; }
                    if (message.AS != null) { Value = Value + nl + "Age of the Air Speed: " + message.AS; }
                    if (message.TAS != null) { Value = Value + nl + "Age of the value for the True Air Speed: " + message.TAS; }
                    if (message.MH != null) { Value = Value + nl + "Age of the value for the Magnetic Heading: " + message.MH; }
                    if (message.BVR != null) { Value = Value + nl + "Age of the Barometric Vertical Rate: " + message.BVR; }
                    if (message.GVR != null) { Value = Value + nl + "Age of the Geometric Vertical Rate: " + message.GVR; }
                    if (message.GV != null) { Value = Value + nl + "Age of the Ground Vector: " + message.GV; }
                    if (message.TAR != null) { Value = Value + nl + "Age of item I021/165 Track Angle Rate: " + message.TAR; }
                    if (message.TI_DataAge != null) { Value = Value + nl + "Age of the Target Identification: " + message.TI_DataAge; }
                    if (message.TS_DataAge != null) { Value = Value + nl + "Age of the Target Status: " + message.TS_DataAge; }
                    if (message.MET != null) { Value = Value + nl + "Age of the Meteorological: " + message.MET; }
                    if (message.ROA != null) { Value = Value + nl + "Age of the Roll Angle value: " + message.ROA; }
                    if (message.ARA_DataAge != null) { Value = Value + nl + "Age of the latest update of an active ACAS Resolution Advisory: " + message.ARA_DataAge; }
                    if (message.SCC != null) { Value = Value + nl + "Age of the latest information received on the surface capabilities and characteristics of the respective target: " + message.SCC; }
                }
                return Value;
            }
            else if (type == 2)
            {
                CAT21vs23 message = new CAT21vs23();
                foreach (CAT21vs23 mes in listaCAT21v23) { if (mes.num == num) { message = mes; }; }
                string nl = Environment.NewLine;
                string Value = "";
                if (i == 5)
                {
                    Value = Value + message.DCR + nl + message.GBS + nl + message.SIM + nl + "Test Target: " + message.TST + nl + message.RAB + nl + message.SAA + nl + message.SPI + nl + "ATP: " + message.ATP + nl + "ARC: " + message.ARC;
                }

                if (i == 9)
                {
                    Value = Value + "AC: " + message.AC + nl + "MN: " + message.MN + nl + "DC: " + message.DC + nl + "Position Accuracy: " + message.PA;
                }

                if (i == 10)
                {
                    Value = Value + "Cockpit Display of Traffic Information: " + message.DTI + nl + "Mode-S Extended Squitter: " + message.MDS + nl + "UAT: " + message.UAT + nl + "VDL Mode 4: " + message.VDL + nl + "Other Technology: " + message.OTR;
                }

                if (i == 21)
                {
                    Value = "Intent Change Flag: " + message.ICF + nl + "LNAV Mode: " + message.LNAV + nl + "Priority Status: " + message.PS + nl + "Surveillance Status: " + message.SS;
                }
                if (i == 26)
                {
                    if (message.Wind_Speed != null) { Value = Value + "Wind Speed: " + message.Wind_Speed; }
                    if (message.Wind_Direction != null) { Value = Value + nl + "Wind Direction: " + message.Wind_Direction; }
                    if (message.Temperature != null) { Value = Value + nl + "Temperature: " + message.Temperature; }
                    if (message.Turbulence != null) { Value = Value + nl + "Turbulence: " + message.Turbulence; }
                }
                if (i == 28)
                {
                    Value = Value + "Manage Vertical Mode: " + message.MV + nl + "Altitude Hold Mode: " + message.AH + "Approach Mode: " + message.AM + nl + "Altitude: " + message.Final_State_Altitude;
                }
                if (i == 29)
                {
                    if (message.NAV != null)
                    {
                        Value = Value + message.NAV + nl + message.NVB;
                    }
                    if (message.REP != 0)
                    {
                        Value = Value + nl + "Repetitions: " + Convert.ToString(message.REP);
                        for (int s = 0; s < message.REP; s++)
                        {
                            Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + message.TCA[s] + nl + message.NC[s] + nl + "Trajectory Change Point number: " + message.TCP[s] + nl + "Altitude: " + message.Altitude[s] + nl + "Latitude: " + message.Latitude[s] + nl + "Longitude: " + message.Longitude[s] + nl + "Point Type: " + message.Point_Type[s] + nl;
                            Value = Value + nl + "TD: " + message.TD[s] + nl + "Turn Radius Availabilty" + message.TRA[s] + nl + message.TOA[s] + nl + "Time Over Point: " + message.TOV[s] + nl + "TCP Turn radius: " + message.TTR[s];
                        }
                    }
                }

                return Value;
            }
            else if (type == 3)
            {
                string nl = Environment.NewLine;
                string Value = "";
                string CAT = Convert.ToString((DatagridView.Items[e] as DataRowView).Row.ItemArray[1]);
                if (CAT == "10")
                {
                    CAT10 message = new CAT10();
                    foreach (CAT10 mes in listaCAT10) { if (mes.num == num) { message = mes; }; }
                    Value = "TYP: " + message.TYP + nl + message.DCR + nl + message.CHN + nl + message.GBS + nl + message.CRT;
                    if (message.SIM != null) { Value = Value + nl + message.SIM + nl + message.TST + nl + message.RAB + nl + message.LOP + nl + message.TOT; }
                    if (message.SPI != null) { Value = Value + nl + message.SPI; }
                }
                if (CAT == "21 v. 2.1")
                {
                    CAT21vs21 message = new CAT21vs21();
                    foreach (CAT21vs21 mes in listaCAT21v21) { if (mes.num == num) { message = mes; }; }
                    Value = " Address Type: " + message.ATP + nl + "Altitude Reporting Capability: " + message.ARC + nl + "Range Check: " + message.RC + nl + "Report Type: " + message.RAB;
                    if (message.DCR != null)
                    {
                        Value = Value + nl + "Differential Correction: " + message.DCR + nl + "Ground Bit Setting: " + message.GBS + nl + "Simulated Target: " + message.SIM + nl + "Test Target: " + message.TST + nl + "Selected Altitude Available: " + message.SAA + nl + "Confidence Level: " + message.CL;
                        if (message.IPC != null)
                        {
                            Value = Value + nl + "Independent Position Check: " + message.IPC + nl + "No-go Bit Status: " + message.NOGO + nl + "Compact Posiotion Reporting: " + message.CPR + nl + "Local Decoding Position Jump: " + message.LDPJ + nl + "Range Check: " + message.RCF;
                        }
                    }
                }
                if (CAT == "21 v. 0.23" || CAT== "21 v. 0.26")
                {
                    CAT21vs23 message = new CAT21vs23();
                    foreach (CAT21vs23 mes in listaCAT21v23) { if (mes.num == num) { message = mes; }; }
                    Value = Value + message.DCR + nl + message.GBS + nl + message.SIM + nl + "Test Target: " + message.TST + nl + message.RAB + nl + message.SAA + nl + message.SPI + nl + "ATP: " + message.ATP + nl + "ARC: " + message.ARC;
                }
                return Value;
            }
            else { return "No Data"; }
        }

        /// <summary>
        /// Shows or hides the alert showing that the search string introduced is not valid
        /// </summary>
        /// <param name="i"></param>
        private void Alertvisible(bool i)
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

        /// <summary>
        /// Show flight on map click
        /// </summary>
        private void ShowOnMap_Click(object sender, MouseButtonEventArgs e)
        {
            if (SelectedNumber.Text != "Not Selected") //Checks if there is a selected flight
            {
                CATALL FlightSelected = listaCATALL.Find(x => x.num == Convert.ToInt32(SelectedNumber.Text)); //Finds the selected flight in the cat all list
                if (FlightSelected.Latitude_in_WGS_84 != -200 || FlightSelected.Longitude_in_WGS_84 != -200) //Checks if the parameters are valid
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    Form.OpenFlightInMap(FlightSelected);
                }
                else
                {
                    AlertMessageNotValid(true);
                }
            }
            else
            {
                AlertNoMessageSelected(true);
            }
        }

        /// <summary>
        /// Shows or hides an alert showing that there is no selected message
        /// </summary>
        private void AlertNoMessageSelected(bool a)
        {
            if (a == true)
            {
                SelectedNumber.Foreground = new SolidColorBrush(Color.FromRgb(194, 0, 0));
                SelectedID.Foreground = new SolidColorBrush(Color.FromRgb(194, 0, 0));
            }
            else
            {
                SelectedNumber.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
                SelectedID.Foreground = new SolidColorBrush(Color.FromRgb(250, 248, 212));
            }
        }


        /// <summary>
        /// Shows or hides an alert showing that the selected message is not valid
        /// </summary>
        private void AlertMessageNotValid(bool a)
        {
            if (a == true)
            {
                HelpLabel.Text = "";
                HelpLabelVisible = false;
                HelpLabel.Visibility = Visibility.Hidden;
                MessageNotValidLabel.Visibility = Visibility.Visible;
                AlertLabelIco.Visibility = Visibility.Visible;
                CloseHelpIco.Visibility = Visibility.Hidden;
                UnderBorder12.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                HelpRow.Height = new GridLength(1, GridUnitType.Auto);
            }
            else
            {
                HelpRow.Height = new GridLength(0);
                UnderBorder12.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            }
        }

        /// <summary>
        /// Exports all the table to CSV format
        /// </summary>>
        private void ExportCSV_Click(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog1.Filter = "csv files (*.csv*)|*.csv*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true && saveFileDialog1.SafeFileName != null)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                string path0 = saveFileDialog1.FileName;
                string path = path0 + ".csv";
                StringBuilder sb = new StringBuilder();
                if (File.Exists(path)) { File.Delete(path); }

                if (type == 0)
                {
                    StringBuilder ColumnsNames = new StringBuilder();
                    foreach (DataColumn col in TableCat10.Columns)
                    {
                        if (col.ColumnName != "CAT number")
                        {
                            string Name = col.ColumnName.Replace('\n', ' ');
                            ColumnsNames.Append(Name + ",");
                        }
                    }
                    string ColNames = ColumnsNames.ToString();
                    ColNames = ColNames.TrimEnd(',');
                    sb.AppendLine(ColNames);
                    foreach (DataRow row in TableCat10.Rows) //cat10
                    {
                        string nl = "; ";
                        StringBuilder RowData = new StringBuilder();
                        int number = Convert.ToInt32(row[1].ToString());
                        CAT10 message = listaCAT10[number];
                        foreach (DataColumn column in TableCat10.Columns)
                        {
                            string Value = "";
                            if (column.ColumnName != "CAT number")
                            {
                                string data = row[column].ToString();
                                if (data == "Click to expand")
                                {
                                    if (column.ColumnName == "Target\nReport\nDescriptor")
                                    {
                                        Value = "TYP: " + message.TYP + nl + message.DCR + nl + message.CHN + nl + message.GBS + nl + message.CRT;
                                        if (message.SIM != null) { Value = Value + nl + message.SIM + nl + message.TST + nl + message.RAB + nl + message.LOP + nl + message.TOT; }
                                        if (message.SPI != null) { Value = Value + nl + message.SPI; }
                                    }
                                    if (column.ColumnName == "Track Status")
                                    {
                                        Value = Value + message.CNF + nl + message.TRE + nl + message.CST + nl + message.MAH + nl + message.TCC + nl + message.STH;
                                        if (message.TOM != null) { Value = Value + nl + message.TOM + nl + message.DOU + nl + message.MRS; }
                                        if (message.GHO != null) { Value = Value + nl + message.GHO; }
                                    }
                                    if (column.ColumnName == "System\nStatus")
                                    {
                                        Value = message.NOGO + nl + message.OVL + nl + message.TSV + nl + message.DIV + nl + message.TIF;
                                    }

                                    if (column.ColumnName == "Mode-3A\nCode")
                                    {
                                        Value = message.V_Mode_3A + nl + message.G_Mode_3A + nl + message.L_Mode_3A + nl + message.Mode_3A;
                                    }
                                    if (column.ColumnName == "Mode S MB\nData")
                                    {
                                        Value = Value + "Repetitions: " + message.modeS_rep;
                                        for (int s = 0; s < message.modeS_rep; s++)
                                        {
                                            Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + "Mode S Comm B message data: " + message.MB_Data[s] + nl + "Comm B Data Buffer Store 1 Address: " + message.BDS1[s] + nl + "Comm B Data Buffer Store 2 Address: " + message.BDS2[s];
                                        }
                                    }

                                    if (column.ColumnName == "Standard\nDeviation\nof Position")
                                    {
                                        Value = message.Deviation_X + nl + message.Deviation_Y + nl + message.Covariance_XY;
                                    }
                                    if (column.ColumnName == "Presence")
                                    {
                                        Value = Value + "Repetitions: " + Convert.ToString(message.REP_Presence);
                                        for (int s = 0; s < message.REP_Presence; s++)
                                        {
                                            Value = Value + nl + "Difference between the radial distance of the plot centre and that of the presence: " + message.DRHO[s] + nl + "Difference between the azimuth of the plot centre and that of the presence: " + message.DTHETA[s];
                                        }
                                    }
                                    data = Value;

                                }
                                data = data.Replace(",", ".");
                                RowData.Append(data);
                                RowData.Append(",");
                            }
                        }
                        string RowDat = RowData.ToString();
                        RowDat = RowDat.TrimEnd(',');
                        sb.AppendLine(RowDat);
                    }
                }

                if (type == 1) //cat21v21
                {
                    StringBuilder ColumnsNames = new StringBuilder();
                    foreach (DataColumn col in TableCat21v21.Columns)
                    {
                        if (col.ColumnName != "CAT number")
                        {
                            string Name = col.ColumnName.Replace('\n',' ');
                            ColumnsNames.Append(Name + ",");
                        }
                    }
                    string ColNames = ColumnsNames.ToString();
                    ColNames = ColNames.TrimEnd(',');
                    sb.AppendLine(ColNames);
                    foreach (DataRow row in TableCat21v21.Rows) //cat10
                    {
                        string nl = "; ";
                        string Value = "";
                        StringBuilder RowData = new StringBuilder();
                        int number = Convert.ToInt32(row[1].ToString());
                        CAT21vs21 message = listaCAT21v21[number];
                        foreach (DataColumn column in TableCat21v21.Columns)
                        {
                            if (column.ColumnName != "CAT number")
                            {
                                string data = row[column].ToString();
                                Value = "";
                                if (data == "Click to expand")
                                {
                                    if (column.ColumnName == "Target\nReport\nDescriptor")
                                    {
                                        Value = " Address Type: " + message.ATP + nl + "Altitude Reporting Capability: " + message.ARC + nl + "Range Check: " + message.RC + nl + "Report Type: " + message.RAB;
                                        if (message.DCR != null)
                                        {
                                            Value = Value + nl + "Differential Correction: " + message.DCR + nl + "Ground Bit Setting: " + message.GBS + nl + "Simulated Target: " + message.SIM + nl + "Test Target: " + message.TST + nl + "Selected Altitude Available: " + message.SAA + nl + "Confidence Level: " + message.CL;
                                            if (message.IPC != null)
                                            {
                                                Value = Value + nl + "Independent Position Check: " + message.IPC + nl + "No-go Bit Status: " + message.NOGO + nl + "Compact Posiotion Reporting: " + message.CPR + nl + "Local Decoding Position Jump: " + message.LDPJ + nl + "Range Check: " + message.RCF;
                                            }
                                        }
                                    }
                                    if (column.ColumnName == "Quality\nIndicators")
                                    {
                                        Value = "NUCr or NACv: " + message.NUCr_NACv + nl + "NUCp or NIC: " + message.NUCp_NIC;
                                        if (message.NICbaro != null)
                                        {
                                            Value = Value + nl + "Navigation Integrity Category for Barometric Altitude: " + message.NICbaro + nl + "Surveillance or Source  Integrity Level: " + message.SIL + nl + "Navigation Accuracy Category for Position: " + message.NACp;
                                            if (message.SILS != null)
                                            {
                                                Value = Value + nl + "SIL-Supplement: " + message.SILS + nl + "Horizontal Position System Design Assurance Level: " + message.SDA + nl + "Geometric Altitude Accuracy: " + message.GVA;
                                                if (message.ICB != null) { Value = Value + nl + "Position Integrity Category:" + nl + "Integrity Containment Bound" + message.ICB + nl + "NUCp: " + message.NUCp + nl + "NIC: " + message.NIC; }
                                            }
                                        }
                                    }
                                    if (column.ColumnName == "Target\nStatus")
                                    {
                                        Value = "Intent Change Flag: " + message.ICF + nl + "LNAV Mode: " + message.LNAV + nl + "Priority Status: " + message.PS + nl + "Surveillance Status: " + message.SS;
                                    }
                                    if (column.ColumnName == "Met Information")
                                    {
                                        if (message.Wind_Speed != null) { Value = Value + "Wind Speed: " + message.Wind_Speed; }
                                        if (message.Wind_Direction != null) { Value = Value + nl + "Wind Direction: " + message.Wind_Direction; }
                                        if (message.Temperature != null) { Value = Value + nl + "Temperature: " + message.Temperature; }
                                        if (message.Turbulence != null) { Value = Value + nl + "Turbulence: " + message.Turbulence; }
                                    }
                                    if (column.ColumnName == "Final\nState\nSelected\nAltitude")
                                    {
                                        Value = Value + "Manage Vertical Mode: " + message.MV + nl + "Altitude Hold Mode: " + message.AH + "Approach Mode: " + message.AM + nl + "Altitude: " + message.Final_State_Altitude;
                                    }
                                    if (column.ColumnName == "Trajectory\nIntent")
                                    {
                                        if (message.NAV != null)
                                        {
                                            Value = Value + message.NAV + nl + message.NVB;
                                        }
                                        if (message.REP != 0)
                                        {
                                            Value = Value + nl + "Repetitions: " + Convert.ToString(message.REP);
                                            for (int s = 0; s < message.REP; s++)
                                            {
                                                Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + message.TCA[s] + nl + message.NC[s] + nl + "Trajectory Change Point number: " + message.TCP[s] + nl + "Altitude: " + message.Altitude[s] + nl + "Latitude: " + message.Latitude[s] + nl + "Longitude: " + message.Longitude[s] + nl + "Point Type: " + message.Point_Type[s] + nl;
                                                Value = Value + nl + "TD: " + message.TD[s] + nl + "Turn Radius Availabilty" + message.TRA[s] + nl + message.TOA[s] + nl + "Time Over Point: " + message.TOV[s] + nl + "TCP Turn radius: " + message.TTR[s];
                                            }
                                        }
                                    }
                                    if (column.ColumnName == "Aircraft\nOperational\nStatus")
                                    {
                                        Value = Value + message.RA + nl + "Target Trajectory Change Report Capability: " + message.TC + nl + "Target State Report Capability: " + message.TS + nl + "Air-Referenced Velocity Report Capability: " + message.ARV + nl + "Cockpit Display of Traffic Information airborne: " + message.CDTIA + nl + "TCAS System Status: " + message.Not_TCAS + nl + message.SA;
                                    }
                                    if (column.ColumnName == "Surface\nCapabilities\nand\nCharacteristics")
                                    {
                                        Value = Value + message.POA + nl + message.CDTIS + nl + message.B2_low + nl + message.RAS + nl + message.IDENT;
                                        if (message.LengthandWidth != null) { Value = Value + nl + message.LengthandWidth; }
                                    }
                                    if (column.ColumnName == "Mode S MB Data")
                                    {
                                        Value = Value + "Repetitions: " + message.modeS_rep;
                                        for (int s = 0; s < message.modeS_rep; s++)
                                        {
                                            Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + "Mode S Comm B message data: " + message.MB_Data[s] + nl + "Comm B Data Buffer Store 1 Address: " + message.BDS1[s] + nl + "Comm B Data Buffer Store 2 Address: " + message.BDS2[s];
                                        }
                                    }
                                    if (column.ColumnName == "ACAS\nResolution\nAdvisory\nReport")
                                    {
                                        Value = Value + "Message Type: " + message.TYP + nl + "Message Sub-type: " + message.STYP + nl + "Active Resolution Advisories: " + message.ARA + nl + "RAC(RA Complement) Record: " + message.RAC + nl + "RA Terminated: " + message.RAT + nl + "Multiple Threat Encounter: " + message.MTE + nl + "Threat Type Indicator: " + message.TTI + nl + "Threat Identity Data: " + message.TID;
                                    }
                                    if (column.ColumnName == "Data Ages")
                                    {
                                        if (message.AOS != null) { Value = Value + "Age of the latest received information transmitted in item I021 / 008: " + message.AOS; }
                                        if (message.TRD != null) { Value = Value + nl + "Age of the last update of the Target Report Descriptor: " + message.TRD; }
                                        if (message.M3A != null) { Value = Value + nl + "Age of the last update of the Mode 3 / A Code: " + message.M3A; }
                                        if (message.QI != null) { Value = Value + nl + "Age of the latest information received to update the Quality Indicators: " + message.QI; }
                                        if (message.TI != null) { Value = Value + nl + "Age of the last update of the Trajectory Intent: " + message.TI; }
                                        if (message.MAM != null) { Value = Value + nl + "Age of the latest measurement of the message amplitude: " + message.MAM; }
                                        if (message.GH != null) { Value = Value + nl + "Age of the information contained in item 021 / 140: " + message.GH; }
                                        if (message.FL != null) { Value = Value + nl + "Age of the Flight Level information: " + message.FL; }
                                        if (message.ISA != null) { Value = Value + nl + "Age of the Intermediate State Selected Altitude: " + message.ISA; }
                                        if (message.FSA != null) { Value = Value + nl + "Age of the Final State Selected Altitude: " + message.FSA; }
                                        if (message.AS != null) { Value = Value + nl + "Age of the Air Speed: " + message.AS; }
                                        if (message.TAS != null) { Value = Value + nl + "Age of the value for the True Air Speed: " + message.TAS; }
                                        if (message.MH != null) { Value = Value + nl + "Age of the value for the Magnetic Heading: " + message.MH; }
                                        if (message.BVR != null) { Value = Value + nl + "Age of the Barometric Vertical Rate: " + message.BVR; }
                                        if (message.GVR != null) { Value = Value + nl + "Age of the Geometric Vertical Rate: " + message.GVR; }
                                        if (message.GV != null) { Value = Value + nl + "Age of the Ground Vector: " + message.GV; }
                                        if (message.TAR != null) { Value = Value + nl + "Age of item I021/165 Track Angle Rate: " + message.TAR; }
                                        if (message.TI_DataAge != null) { Value = Value + nl + "Age of the Target Identification: " + message.TI_DataAge; }
                                        if (message.TS_DataAge != null) { Value = Value + nl + "Age of the Target Status: " + message.TS_DataAge; }
                                        if (message.MET != null) { Value = Value + nl + "Age of the Meteorological: " + message.MET; }
                                        if (message.ROA != null) { Value = Value + nl + "Age of the Roll Angle value: " + message.ROA; }
                                        if (message.ARA_DataAge != null) { Value = Value + nl + "Age of the latest update of an active ACAS Resolution Advisory: " + message.ARA_DataAge; }
                                        if (message.SCC != null) { Value = Value + nl + "Age of the latest information received on the surface capabilities and characteristics of the respective target: " + message.SCC; }
                                    }
                                    data = Value;
                                }
                                data = data.Replace(",", ".");
                                RowData.Append(data);
                                RowData.Append(",");
                            }
                        }
                        string RowDat = RowData.ToString();
                        RowDat = RowDat.TrimEnd(',');
                        sb.AppendLine(RowDat);
                    }
                }

                if (type == 2) //cat21v23
                {
                    StringBuilder ColumnsNames = new StringBuilder();
                    foreach (DataColumn col in TableCat21v23.Columns)
                    {
                        if (col.ColumnName != "CAT number")
                        {
                            string Name = col.ColumnName.Replace('\n', ' ');

                            ColumnsNames.Append(Name + ",");
                        }
                    }
                    string ColNames = ColumnsNames.ToString();
                    ColNames = ColNames.TrimEnd(',');
                    sb.AppendLine(ColNames);
                    foreach (DataRow row in TableCat21v23.Rows)
                    {
                        string nl = "; ";
                        StringBuilder RowData = new StringBuilder();
                        int number = Convert.ToInt32(row[1].ToString());
                        CAT21vs23 message = listaCAT21v23[number];
                        foreach (DataColumn column in TableCat21v23.Columns)
                        {
                            if (column.ColumnName != "CAT number")
                            {
                                string data = row[column].ToString();
                                string Value = "";
                                if (data == "Click to expand")
                                {
                                    if (column.ColumnName == "Target\nReport\nDescriptor")
                                    {
                                        Value = Value + message.DCR + nl + message.GBS + nl + message.SIM + nl + "Test Target: " + message.TST + nl + message.RAB + nl + message.SAA + nl + message.SPI + nl + "ATP: " + message.ATP + nl + "ARC: " + message.ARC;
                                    }

                                    if (column.ColumnName == "Figure of\nMerit")
                                    {
                                        Value = Value + "AC: " + message.AC + nl + "MN: " + message.MN + nl + "DC: " + message.DC + nl + "Position Accuracy: " + message.PA;
                                    }

                                    if (column.ColumnName == "Link\nTechnology")
                                    {
                                        Value = Value + "Cockpit Display of Traffic Information: " + message.DTI + nl + "Mode-S Extended Squitter: " + message.MDS + nl + "UAT: " + message.UAT + nl + "VDL Mode 4: " + message.VDL + nl + "Other Technology: " + message.OTR;
                                    }

                                    if (column.ColumnName == "Target\nStatus")
                                    {
                                        Value = "Intent Change Flag: " + message.ICF + nl + "LNAV Mode: " + message.LNAV + nl + "Priority Status: " + message.PS + nl + "Surveillance Status: " + message.SS;
                                    }
                                    if (column.ColumnName == "Met\nInformation")
                                    {
                                        if (message.Wind_Speed != null) { Value = Value + "Wind Speed: " + message.Wind_Speed; }
                                        if (message.Wind_Direction != null) { Value = Value + nl + "Wind Direction: " + message.Wind_Direction; }
                                        if (message.Temperature != null) { Value = Value + nl + "Temperature: " + message.Temperature; }
                                        if (message.Turbulence != null) { Value = Value + nl + "Turbulence: " + message.Turbulence; }
                                    }
                                    if (column.ColumnName == "Final State\nSelected\nAltitude")
                                    {
                                        Value = Value + "Manage Vertical Mode: " + message.MV + nl + "Altitude Hold Mode: " + message.AH + "Approach Mode: " + message.AM + nl + "Altitude: " + message.Final_State_Altitude;
                                    }
                                    if (column.ColumnName == "Trajectory\nIntent")
                                    {
                                        if (message.NAV != null)
                                        {
                                            Value = Value + message.NAV + nl + message.NVB;
                                        }
                                        if (message.REP != 0)
                                        {
                                            Value = Value + nl + "Repetitions: " + Convert.ToString(message.REP);
                                            for (int s = 0; s < message.REP; s++)
                                            {
                                                Value = Value + nl + "Repetition: " + Convert.ToString(s) + nl + message.TCA[s] + nl + message.NC[s] + nl + "Trajectory Change Point number: " + message.TCP[s] + nl + "Altitude: " + message.Altitude[s] + nl + "Latitude: " + message.Latitude[s] + nl + "Longitude: " + message.Longitude[s] + nl + "Point Type: " + message.Point_Type[s] + nl;
                                                Value = Value + nl + "TD: " + message.TD[s] + nl + "Turn Radius Availabilty" + message.TRA[s] + nl + message.TOA[s] + nl + "Time Over Point: " + message.TOV[s] + nl + "TCP Turn radius: " + message.TTR[s];
                                            }
                                        }
                                    }

                                    data = Value;

                                }
                                data = data.Replace(",", ".");
                                RowData.Append(data);
                                RowData.Append(",");
                            }
                        }
                        string RowDat = RowData.ToString();
                        RowDat = RowDat.TrimEnd(',');
                        sb.AppendLine(RowDat);
                    }
                }
                if (type == 3) //all
                {
                    {
                        StringBuilder ColumnsNames = new StringBuilder();
                        foreach (DataColumn col in TableAll.Columns)
                        {
                            if (col.ColumnName != "CAT number")
                            {
                                string Name = col.ColumnName.Replace('\n', ' ');

                                ColumnsNames.Append(Name + ",");
                            }
                        }
                        string ColNames = ColumnsNames.ToString();
                        sb.AppendLine(ColNames);
                        foreach (DataRow row in TableAll.Rows) //cat10
                        {
                            string nl = "; ";
                            StringBuilder RowData = new StringBuilder();
                            int number = Convert.ToInt32(row[1].ToString());
                            string Cat = row[2].ToString();
                            foreach (DataColumn column in TableAll.Columns)
                            {
                                if (column.ColumnName != "CAT number")
                                {
                                    string data = row[column].ToString();
                                    string Value = "";
                                    if (data == "Click to expand")
                                    {
                                        if (Cat == "10")
                                        {
                                            CAT10 message10 = listaCAT10[number];
                                            Value = "TYP: " + message10.TYP + nl + message10.DCR + nl + message10.CHN + nl + message10.GBS + nl + message10.CRT;
                                            if (message10.SIM != null) { Value = Value + nl + message10.SIM + nl + message10.TST + nl + message10.RAB + nl + message10.LOP + nl + message10.TOT; }
                                            if (message10.SPI != null) { Value = Value + nl + message10.SPI; }
                                        }
                                        else if (Cat == "21 v. 2.1")
                                        {
                                            CAT21vs21 message21v21 = listaCAT21v21[number];
                                            Value = " Address Type: " + message21v21.ATP + nl + "Altitude Reporting Capability: " + message21v21.ARC + nl + "Range Check: " + message21v21.RC + nl + "Report Type: " + message21v21.RAB;
                                            if (message21v21.DCR != null)
                                            {
                                                Value = Value + nl + "Differential Correction: " + message21v21.DCR + nl + "Ground Bit Setting: " + message21v21.GBS + nl + "Simulated Target: " + message21v21.SIM + nl + "Test Target: " + message21v21.TST + nl + "Selected Altitude Available: " + message21v21.SAA + nl + "Confidence Level: " + message21v21.CL;
                                                if (message21v21.IPC != null)
                                                {
                                                    Value = Value + nl + "Independent Position Check: " + message21v21.IPC + nl + "No-go Bit Status: " + message21v21.NOGO + nl + "Compact Posiotion Reporting: " + message21v21.CPR + nl + "Local Decoding Position Jump: " + message21v21.LDPJ + nl + "Range Check: " + message21v21.RCF;
                                                }
                                            }

                                        }
                                        else if (Cat == "21 v. 0.23" || Cat== "21 v. 0.26")
                                        {
                                            CAT21vs23 message21v23 = listaCAT21v23[number];
                                            Value = Value + message21v23.DCR + nl + message21v23.GBS + nl + message21v23.SIM + nl + "Test Target: " + message21v23.TST + nl + message21v23.RAB + nl + message21v23.SAA + nl + message21v23.SPI + nl + "ATP: " + message21v23.ATP + nl + "ARC: " + message21v23.ARC;

                                        }
                                        else { MessageBox.Show(Convert.ToString(Cat)); }
                                        data = Value;
                                    }
                                    data = data.Replace(",", ".");
                                    RowData.Append(data);
                                    RowData.Append(",");
                                }
                            }
                            string RowDat = RowData.ToString();
                            RowDat = RowDat.TrimEnd(',');
                            sb.AppendLine(RowDat);
                        }
                    }
                }
                File.WriteAllText(path, sb.ToString());
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// On page resize, recomputes the datagrid needed size
        /// </summary>
        private void ResizeForm(object sender, SizeChangedEventArgs e)
        {
            double tableWidth = 0;
            for (int i = 0; i < DatagridView.Columns.Count(); i++)
            {
                tableWidth += DatagridView.Columns[i].ActualWidth;
            }
            tableWidth += 20;
            double pagewidth = Grid.Width;
            if (pagewidth > tableWidth)
            {
                DatagridView.Width = tableWidth;
            }
        }
    }
}
