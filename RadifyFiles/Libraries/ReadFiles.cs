using System.Collections.Generic;
using System.IO;
using System.Data;
using System;
using System.Windows;
using GMap.NET;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Threading.Tasks;

namespace PGTAWPF
{


    /// <summary>
    /// Class to read the files, decode them and work with them. It contains the necessary functions to create the lists and tables with the messages, calculation of trajectories, setting the times ...
    /// 
    /// The data of the messages decoded in the files will be stored in this class, in this way we will only have to share an instance of this class, and not all the lists between forms
    /// </summary>
    public class ReadFiles
    {

        readonly List<CAT10> listaCAT10 = new List<CAT10>(); //List with all of cat 10 message
        readonly List<CAT21vs21> listaCAT21v21 = new List<CAT21vs21>();// List with all of cat21 v2.1 messages
        readonly List<CAT21vs23> listaCAT21v23 = new List<CAT21vs23>(); //List with all of cat21 v0.23(or 0.26) messages
        readonly List<CAT62> listaCAT62 = new List<CAT62>(); //List with all of cat21 v0.23(or 0.26) messages

        List<CATALL> listaCATAll = new List<CATALL>();//List containing all messages in Cat all form

        public int numficheros = 0; //number of loaded files

        public int numero = 0; //number of loaded messages
        public int CAT10num = 0; //number of loaded cat 10 messages
        public int CAT21v23num = 0; //number of loaded cat21 v0.23 messages
        public int CAT21v21num = 0; //number of loaded cat21 v2.1 messages
        public int CAT62num = 0; //number of loaded cat21 v2.1 messages

        public List<string> names = new List<string>(); //List with loaded files names

        readonly LibreriaDecodificacion lib = new LibreriaDecodificacion(); //Instance of Decodification Library

        readonly DataTable tablaCAT10 = new DataTable(); //Datatable with all message cat 10 information
        readonly DataTable tablaCAT21v21 = new DataTable(); //Datatable with all cat21 v2.1 messages information
        readonly DataTable tablaCAT21v23 = new DataTable(); //Datatable with all cat21 v0.23 messages information
        readonly DataTable tablaCAT62 = new DataTable(); //Datatable with all cat21 v0.23 messages information


        readonly DataTable tablaAll = new DataTable(); //Datatable with all messages information

        public List<int> AirportCodesList = new List<int>(); //List of used airports. This list is useful to know which buttons to center on the map put

        public string process; //string showing the actual loading process

        List<Trajectories> SMRTrajectories = new List<Trajectories>(); //List with SMR trajectories
        List<Trajectories> MLATTrajectories = new List<Trajectories>(); //List with MLAT trajectories
        List<Trajectories> ADSBTrajectories = new List<Trajectories>(); //List with ADSB trajectories
        List<Trajectories> CAT62Trajectories = new List<Trajectories>(); //List with ADSB trajectories

        /// <summary>
        /// When creatinmg a new instance of this class we create all tables to fill them later
        /// </summary>
        public ReadFiles()
        {
            this.StartTable10();
            this.StartTable21vs21();
            this.StartTable21v23();
            this.StartTable62();
            this.StartTableAll();
        }

        /// <summary>
        /// Clear all data from this class
        /// </summary>
        public void ResetData()
        {
            listaCAT10.Clear();
            listaCAT21v21.Clear();
            listaCAT21v23.Clear();
            listaCAT62.Clear();
            listaCATAll.Clear();
            tablaCAT10.Clear();
            tablaCAT21v21.Clear();
            tablaCAT21v23.Clear();
            tablaCAT62.Clear();
            AirportCodesList.Clear();
            tablaAll.Clear();
            names.Clear();
            numficheros = 0;
            numero = 0;
            CAT10num = 0;
            CAT21v23num = 0;
            CAT21v21num = 0;
            CAT62num = 0;
        }


        public List<CAT10> GetListCAT10()
        {
            return listaCAT10;
        }
        public List<CAT21vs21> GetListCAT21v21()
        {
            return listaCAT21v21;
        }
        public List<CAT21vs23> GetListCAT21v23()
        {
            return listaCAT21v23;
        }

        public List<CAT62> GetListCAT62()
        {
            return listaCAT62;
        }


        public List<CATALL> GetListCATALL()
        {
            return listaCATAll;
        }

        /// <summary>
        /// Reads and decodifies a file, loading it into the lists and tables, aplying trajectory to all points and setting the correct time to match map days.
        /// </summary>
        /// <param name="path">Path from the file</param>
        /// <param name="first_time">Parameter that indicates the time 00:00:00 of the day of the message (if the message is from the second day the time will be 86400)</param>
        /// <param name="listchecked">List of bools which indicates which classes should be decodified and wchich ignored</param>
        /// <returns>returns a 1 if reading went well and 0 if something went wrong</returns>
        public int Leer(string path, int first_time, List<bool> listchecked)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true); //collect garbage from previous files to free up space
            //try
            //{
                /*We create a new cat all list. The cat all list contains values that are only used in that list for the map,
                 * such as the refresh rate, the direction, the total time including the day, ... In order not to calculate
                 * the entire list of all files each time, we will create a new list with those messages, we will do the 
                 * calculations and then we will put the messages in the general list.*/
                List<CATALL> newCatAll = new List<CATALL>();

                /*As said before, process is a string that identifies where the loading process is going. 
                 * This string will be displayed to the user. We will be modifying to update the loading process*/
                process = "Decodifying file..."; 

                bool first = true; //Identifies if a message is the first of a file or not. Usefull to set correct message timing
                int first_time_file = 0; 

                byte[] fileBytes = File.ReadAllBytes(path); //read all bytes from file 
                List<byte[]> listabyte = new List<byte[]>();

                /*We are going to divide the file into messages and pass it to a list of string [] where each value in the list will
                 * be a message, and each string in string [] will be an octet of the message. For more information on decoding
                 * Asterix messages please refer to the Eurocontrol documentation on the Asterix system.*/

                int i = 0;
                int contador = fileBytes[2] + (fileBytes[1] * 256);
                while (i < fileBytes.Length)
                {
                    byte[] array = new byte[contador];
                    for (int j = 0; j < array.Length; j++)
                    {
                        array[j] = fileBytes[i];
                        i++;
                    }
                    listabyte.Add(array);
                    if (i + 2 < fileBytes.Length)
                    {
                        contador = fileBytes[i + 2] + (fileBytes[i + 1] * 256);
                    }
                }

                List<string[]> listahex = new List<string[]>();
                for (int x = 0; x < listabyte.Count; x++)
                {
                    byte[] buffer = listabyte[x];
                    string[] arrayhex = new string[buffer.Length];
                    for (int y = 0; y < buffer.Length; y++)
                    {
                        arrayhex[y] = buffer[y].ToString("X");
                    }
                    listahex.Add(arrayhex);
                }

            /*Having the file divided into a list of messages and each 
             * message divided into octets, we will decode the messages and save them*/

            //   Parallel.ForEach(listahex, arraystring =>
            // {
            for (int q = 0; q < listahex.Count; q++)
            {

                process = "Loading message " + Convert.ToString(numero) + " of " + Convert.ToString(listahex.Count) + " messages...";
                string[] arraystring = listahex[q];
                int CAT = int.Parse(arraystring[0], System.Globalization.NumberStyles.HexNumber); //Get cat to know which decodification apply

                if (CAT == 10 && listchecked[0] == true) //If listcheked[0]== true because list checked indicates which cat of messages load. ListCheked[0] is a bool indicating if CAT10 must be loaded or not
                {
                    CAT10 newcat10 = new CAT10(arraystring, lib); //Create the CAT10 message from the string[] values
                    newcat10.num = numero; //Set number to message
                    newcat10.cat10num = this.CAT10num; //set cat10 number to message

                    numero++; //increase counter so numbers are unics
                    CAT10num++;

                    if (first == true)
                    {
                        /*Set the time of this file first message.  
                         * This is useful because a file will generally never last more than 24 hours, but it may take two days.
                         * In this way, if we have a message with a time less than the time of the first message, it will surely
                         * be the next day (file that goes from 23:00 to 01:00, the messages from 00:00 will be the next day,
                         * and We will know because they will have an h before the first message */
                        first_time_file = newcat10.Time_of_day_sec;
                        first = false;
                    }

                    CATALL newcatall = new CATALL(newcat10, first_time, first_time_file); //Create a CAT all message, usefull for map and for See Cat All tab.

                    /*We will add the messages to the lists and tables. It is interesting to add them to both, since the lists are very useful to search for
                     * messages and work with them, while the tables are interesting because then when loading the tabs it 
                     * loads faster than if all the information had to be passed from the list every time*/
                    listaCAT10.Add(newcat10);
                    newCatAll.Add(newcatall);

                    AddRowTable10(newcat10);
                    AddRowTableAllCat10(newcat10);

                    /*We will see if the airport of which the file is already in the list of used airports or not, 
                     * and if it is not, we will add it. This list is useful to know which buttons to center on the map put*/
                    bool exists = AirportCodesList.Contains(newcat10.airportCode);
                    if (exists == false) { AirportCodesList.Add(newcat10.airportCode); }
                }
                else if (CAT == 21)
                {

                    if (lib.GetVersion(arraystring) == 0 && listchecked[1] == true)  //If listcheked[1]== true because list checked indicates which cat of messages load. ListCheked[1] is a bool indicating if CAT21v0.23 must be loaded or not
                                                                                     //GetVersion indicates which version of cat 21 the message is. 0=version 0.23
                    {
                        CAT21vs23 newcat21v23 = new CAT21vs23(arraystring, lib); //Create the CAT21 v0.23 message from the string[] values

                        newcat21v23.num = numero; //Set number to message
                        newcat21v23.cat21v23num = CAT21v23num; //set cat10 number to message

                        numero++; //increase counter so numbers are unics
                        CAT21v23num++;

                        if (first == true)
                        {
                            /*Set the time of this file first message.  
                             * This is useful because a file will generally never last more than 24 hours, but it may take two days.
                             * In this way, if we have a message with a time less than the time of the first message, it will surely
                             * be the next day (file that goes from 23:00 to 01:00, the messages from 00:00 will be the next day,
                             * and We will know because they will have an h before the first message */
                            first_time_file = newcat21v23.Time_of_day_sec;
                            first = false;
                        }

                        CATALL newcatall = new CATALL(newcat21v23, first_time, first_time_file); //Create a CAT all message, usefull for map and for See Cat All tab.

                        /*We will add the messages to the lists and tables. It is interesting to add them to both, since the lists are very useful to search for
                         * messages and work with them, while the tables are interesting because then when loading the tabs it 
                         * loads faster than if all the information had to be passed from the list every time*/
                        listaCAT21v23.Add(newcat21v23);
                        newCatAll.Add(newcatall);
                        AddRowTable21v23(newcat21v23);
                        AddRowTableAllCat21v23(newcat21v23);

                        /*We will see if the airport of which the file is already in the list of used airports or not, 
                         * and if it is not, we will add it. This list is useful to know which buttons to center on the map put*/
                        bool exists = AirportCodesList.Contains(newcat21v23.airportCode);
                        if (exists == false) { AirportCodesList.Add(newcat21v23.airportCode); }
                    }
                    else if (listchecked[2] == true) //If listcheked[2]== true because list checked indicates which cat of messages load. ListCheked[2] is a bool indicating if CAT21v2.1 must be loaded or not
                    {
                        CAT21vs21 newcat21 = new CAT21vs21(arraystring, lib); //Create the CAT21 v2.1 message from the string[] values

                        newcat21.num = numero;//Set number to message
                        newcat21.cat21v21num = CAT21v21num; //set cat10 number to message

                        CAT21v21num++; //increase counter so numbers are unics
                        numero++;

                        if (first == true)
                        {
                            /*Set the time of this file first message.  
                             * This is useful because a file will generally never last more than 24 hours, but it may take two days.
                             * In this way, if we have a message with a time less than the time of the first message, it will surely
                             * be the next day (file that goes from 23:00 to 01:00, the messages from 00:00 will be the next day,
                             * and We will know because they will have an h before the first message */
                            first_time_file = newcat21.Time_of_day_sec;
                            first = false;
                        }
                        CATALL newcatall = new CATALL(newcat21, first_time, first_time_file); //Create a CAT all message, usefull for map and for See Cat All tab.

                        /*We will add the messages to the lists and tables. It is interesting to add them to both, since the lists are very useful to search for
                         * messages and work with them, while the tables are interesting because then when loading the tabs it 
                         * loads faster than if all the information had to be passed from the list every time*/
                        listaCAT21v21.Add(newcat21);
                        newCatAll.Add(newcatall);
                        //   AddRowTable21v21(newcat21);
                        // AddRowTableAllCat21v21(newcat21);

                        /*We will see if the airport of which the file is already in the list of used airports or not, 
                        * and if it is not, we will add it. This list is useful to know which buttons to center on the map put*/
                        bool exists = AirportCodesList.Contains(newcat21.airportCode);
                        if (exists == false) { AirportCodesList.Add(newcat21.airportCode); }

                    }
                }
                if (CAT == 62 && listchecked[3] == true) //If listcheked[0]== true because list checked indicates which cat of messages load. ListCheked[0] is a bool indicating if CAT10 must be loaded or not
                {
                    CAT62 newcat62 = new CAT62(arraystring, lib); //Create the CAT10 message from the string[] values
                    newcat62.num = numero; //Set number to message
                    newcat62.cat62num = this.CAT62num; //set cat10 number to message

                    numero++; //increase counter so numbers are unics
                    CAT62num++;

                    if (first == true)
                    {
                        /*Set the time of this file first message.  
                         * This is useful because a file will generally never last more than 24 hours, but it may take two days.
                         * In this way, if we have a message with a time less than the time of the first message, it will surely
                         * be the next day (file that goes from 23:00 to 01:00, the messages from 00:00 will be the next day,
                         * and We will know because they will have an h before the first message */
                        first_time_file = newcat62.Time_of_day_sec;
                        first = false;
                    }

                    CATALL newcatall = new CATALL(newcat62, first_time, first_time_file); //Create a CAT all message, usefull for map and for See Cat All tab.

                    /*We will add the messages to the lists and tables. It is interesting to add them to both, since the lists are very useful to search for
                     * messages and work with them, while the tables are interesting because then when loading the tabs it 
                     * loads faster than if all the information had to be passed from the list every time*/
                    //lock (listaCAT62)
                    //{
                    listaCAT62.Add(newcat62);
                    //}
                    //lock (listaCATAll)
                    //{
                    newCatAll.Add(newcatall);
                    //}
                    // lock(tablaCAT62)
                    //{ 
                    AddRowTable62(newcat62);
                    //}
                    // lock (tablaAll)
                    //{
                    AddRowTableAllCat62(newcat62);

                    //}

                    /*We will see if the airport of which the file is already in the list of used airports or not, 
                     * and if it is not, we will add it. This list is useful to know which buttons to center on the map put*/
                    bool exists = AirportCodesList.Contains(newcat62.airportCode);
                    if (exists == false) { AirportCodesList.Add(newcat62.airportCode); }
                }

            }

               

                newCatAll = ComputeTimeOfDay(newCatAll); //Applies time corrections so messages get correct time

                /*First we create all the trajectries. Each trajecytorie will be a list with all the messages from that same vehicle.
                 * This is necessary, since to calculate the direction we will use the next or previous point to the current one, 
                 * and it is easier if we first order all the messages in lists and then we look for the point in the corresponding list, 
                 * than not if we look for the point among all the messages from the entire cat all messages list
                 * It is also more practical to have the messages arranged like this for the calculation of the detection ratio*/
                ComputeTrajectories(newCatAll);

                ComputeDirecction(newCatAll); //Compute direction of each message so can be rotated in map and show in which direction they go

                ComputeDetectionRatio(newCatAll); //Computes radar detection rates and applies to message so can be draw in accordance to the radar detecion rate and they time

                foreach (CATALL message in newCatAll) { listaCATAll.Add(message); } //Add all messages from nec cat all to global cat all
                newCatAll = null; //Clear new cat all list to free up space

                ClearTrajectories();

                process = "Computing messages time...";
                listaCATAll = ComputeTimeOfDay(listaCATAll); //Apply time correction to all cat all messages list so all files match time

                numficheros++;
                names.Add(path);
                return 1;
            //}
            //catch
            //{
            //    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            //    return 0;
            //}
        }


        public DataTable GetTablaCAT10()
        {
            return tablaCAT10;
        }
        public DataTable GetTablaCAT21v21()
        {
            return tablaCAT21v21;
        }

        public DataTable GetTablaCAT21v23()
        {
            return tablaCAT21v23;
        }

        public DataTable GetTablaCAT62()
        {
            return tablaCAT62;
        }


        public DataTable GetTablaAll()
        {
            return tablaAll;
        }

        /// <summary>
        /// Start Datatable Cat 10. Add all necessary columns
        /// </summary>
        public void StartTable10()
        {
            tablaCAT10.Columns.Add("Number",typeof(int)); 
            tablaCAT10.Columns.Add("CAT number"); 
            tablaCAT10.Columns.Add("Category"); 
            tablaCAT10.Columns.Add("SAC"); 
            tablaCAT10.Columns.Add("SIC"); 
            tablaCAT10.Columns.Add("Target\nIdentification"); 
            tablaCAT10.Columns.Add("Track\nNumber"); 
            tablaCAT10.Columns.Add("Target\nReport\nDescriptor"); 
            tablaCAT10.Columns.Add("Message Type"); 
            tablaCAT10.Columns.Add("Flight Level"); 
            tablaCAT10.Columns.Add("Time of\nDay"); 
            tablaCAT10.Columns.Add("Track Status"); 
            tablaCAT10.Columns.Add("Position in WGS-84 Co-ordinates"); 
            tablaCAT10.Columns.Add("Position in\nCartesian\nCo-ordinates"); 
            tablaCAT10.Columns.Add("Position in\nPolar\nCo-ordinates"); 
            tablaCAT10.Columns.Add("Track Velocity in Polar Coordinates"); 
            tablaCAT10.Columns.Add("Track Velocity in\nCartesian\nCoordinates"); 
            tablaCAT10.Columns.Add("Target Size\nand\nOrientation"); 
            tablaCAT10.Columns.Add("Target\nAddress"); 
            tablaCAT10.Columns.Add("System\nStatus");
            tablaCAT10.Columns.Add("Vehicle Fleet\nIdentification");
            tablaCAT10.Columns.Add("Pre-programmed\nMessage");
            tablaCAT10.Columns.Add("Measured\nHeight");
            tablaCAT10.Columns.Add("Mode-3A\nCode");
            tablaCAT10.Columns.Add("Mode S MB\nData");
            tablaCAT10.Columns.Add("Standard\nDeviation\nof Position");
            tablaCAT10.Columns.Add("Presence");
            tablaCAT10.Columns.Add("Amplitude\nof Primary\nPlot");
            tablaCAT10.Columns.Add("Calculated\nAcceleration");
        }

        /// <summary>
        /// Start Datatable Cat 21 v. 2.1. Add all necessary columns
        /// </summary>
        private void StartTable21vs21()
        { 
            tablaCAT21v21.Columns.Add("Number", typeof(int));  
            tablaCAT21v21.Columns.Add("CAT number");  
            tablaCAT21v21.Columns.Add("Category");  
            tablaCAT21v21.Columns.Add("SAC"); 
            tablaCAT21v21.Columns.Add("SIC"); 
            tablaCAT21v21.Columns.Add("Target\nIdentification"); 
            tablaCAT21v21.Columns.Add("Track\nNumber"); 
            tablaCAT21v21.Columns.Add("Target\nReport\nDescriptor"); 
            tablaCAT21v21.Columns.Add("Service\nIdentification"); 
            tablaCAT21v21.Columns.Add("Time of\nReport\nTransmission"); 
            tablaCAT21v21.Columns.Add("Position in WGS-84 co-ordinates"); 
            tablaCAT21v21.Columns.Add("Position in WGS-84 co-ordinates high res"); 
            tablaCAT21v21.Columns.Add("Air\nSpeed"); 
            tablaCAT21v21.Columns.Add("True Air\nSpeed"); 
            tablaCAT21v21.Columns.Add("Target Address"); 
            tablaCAT21v21.Columns.Add("Time of\nApplicability\nfor Position"); 
            tablaCAT21v21.Columns.Add("Time of\nMessage\nReception\nof Position"); 
            tablaCAT21v21.Columns.Add("Time of\nApplicability\nfor Velocity"); 
            tablaCAT21v21.Columns.Add("Time of\nMessage\nReception\nof Velocity"); 
            tablaCAT21v21.Columns.Add("Geometric\nHeight");  
            tablaCAT21v21.Columns.Add("Quality\nIndicators");  
            tablaCAT21v21.Columns.Add("MOPS Version");
            tablaCAT21v21.Columns.Add("Mode-3A\nCode");
            tablaCAT21v21.Columns.Add("Roll\nAngle");
            tablaCAT21v21.Columns.Add("Flight\nLevel");
            tablaCAT21v21.Columns.Add("Magnetic\nHeading");
            tablaCAT21v21.Columns.Add("Target\nStatus");
            tablaCAT21v21.Columns.Add("Barometric\nVertical Rate");
            tablaCAT21v21.Columns.Add("Geometric\nVertical Rate");
            tablaCAT21v21.Columns.Add("Airborne Ground Vector");
            tablaCAT21v21.Columns.Add("Track\nAngle\nRate");    
            tablaCAT21v21.Columns.Add("Emitter Category");
            tablaCAT21v21.Columns.Add("Met\nInformation");
            tablaCAT21v21.Columns.Add("Selected Altitude");
            tablaCAT21v21.Columns.Add("Final\nState\nSelected\nAltitude");
            tablaCAT21v21.Columns.Add("Trajectory\nIntent");
            tablaCAT21v21.Columns.Add("Service\nManagement");
            tablaCAT21v21.Columns.Add("Aircraft\nOperational\nStatus");
            tablaCAT21v21.Columns.Add("Surface\nCapabilities\nand\nCharacteristics");
            tablaCAT21v21.Columns.Add("Message\nAmplitude");
            tablaCAT21v21.Columns.Add("Mode S MB Data");
            tablaCAT21v21.Columns.Add("ACAS\nResolution\nAdvisory\nReport");
            tablaCAT21v21.Columns.Add("Receiver\nID");
            tablaCAT21v21.Columns.Add("Data Ages");
        }


        /// <summary>
        /// Start Datatable Cat 21 v.0.23. Add all necessary columns
        /// </summary>
        private void StartTable21v23()
        {
            tablaCAT21v23.Columns.Add("Number", typeof(int)); 
            tablaCAT21v23.Columns.Add("CAT number"); 
            tablaCAT21v23.Columns.Add("Category"); 
            tablaCAT21v23.Columns.Add("SAC"); 
            tablaCAT21v23.Columns.Add("SIC"); 
            tablaCAT21v23.Columns.Add("Target\nIdentification"); 
            tablaCAT21v23.Columns.Add("Target\nReport\nDescriptor"); 
            tablaCAT21v23.Columns.Add("Time of\nDay"); 
            tablaCAT21v23.Columns.Add("Time of\nDay\nAccuracity"); 
            tablaCAT21v23.Columns.Add("Position in WGS-84 co-ordinates"); 
            tablaCAT21v23.Columns.Add("Figure of\nMerit"); 
            tablaCAT21v23.Columns.Add("Link\nTechnology");
            tablaCAT21v23.Columns.Add("Air\nSpeed"); 
            tablaCAT21v23.Columns.Add("True Air\nSpeed"); 
            tablaCAT21v23.Columns.Add("Velocity\nAccuracy");
            tablaCAT21v23.Columns.Add("Target\nAddress"); 
            tablaCAT21v23.Columns.Add("Geometric\nAltitude");
            tablaCAT21v23.Columns.Add("Ground Vector");
            tablaCAT21v23.Columns.Add("Rate of\nTurn");
            tablaCAT21v23.Columns.Add("Roll\nAngle");
            tablaCAT21v23.Columns.Add("Flight\nLevel");
            tablaCAT21v23.Columns.Add("Magnetic\nHeading");
            tablaCAT21v23.Columns.Add("Target\nStatus");
            tablaCAT21v23.Columns.Add("Barometric\nVertical\nRate");
            tablaCAT21v23.Columns.Add("Geometric\nVertical\nRate");
            tablaCAT21v23.Columns.Add("Emitter\nCategory");
            tablaCAT21v23.Columns.Add("Met\nInformation");
            tablaCAT21v23.Columns.Add("Intermediate\nState\nSelected\nAltitude");
            tablaCAT21v23.Columns.Add("Final State\nSelected\nAltitude");
            tablaCAT21v23.Columns.Add("Trajectory\nIntent");
            tablaCAT21v23.Columns.Add("Mode-3A\nCode");
            tablaCAT21v23.Columns.Add("Signal\nAmplitude");

        }


        private void StartTable62()
        {

        }

        /// <summary>
        /// Start Datatable Cat All. Add all necessary columns
        /// </summary>
        private void StartTableAll()
        {
            tablaAll.Columns.Add("Number", typeof(int));
            tablaAll.Columns.Add("CAT number");
            tablaAll.Columns.Add("Category");
            tablaAll.Columns.Add("SAC");
            tablaAll.Columns.Add("SIC");
            tablaAll.Columns.Add("Target\nIdentification");
            tablaAll.Columns.Add("Track\nNumber");
            tablaAll.Columns.Add("Target\nAddress");
            tablaAll.Columns.Add("Target\nReport\nDescriptor");
            tablaAll.Columns.Add("Time of\nDay");
            tablaAll.Columns.Add("Position in WGS-84 Co-ordinates");
            tablaAll.Columns.Add("Flight\nLevel");
            tablaAll.Columns.Add("Mode-3A\nCode");
        }

        /// <summary>
        /// Adds a row to datatable cat 10 from Class Cat10 message
        /// </summary>
        /// <param name="Message">Instance of Class Cat10 containing one messge</param>
        public void AddRowTable10(CAT10 Message)
        { 
            var row = tablaCAT10.NewRow();
            row["Number"] = Message.num;
            row["CAT number"] = Message.cat10num;
            if (Message.CAT != null) { row["Category"] = Message.CAT; }
            else { row["Category"] = "No Data"; }
            if (Message.SAC != null) { row["SAC"] = Message.SAC; }
            else { row["SAC"] = "No Data"; }
            if (Message.SIC != null) { row["SIC"] = Message.SIC; }
            else { row["SIC"] = "No Data"; }
            if (Message.TAR != null)
            {
                if (Message.TAR.Replace(" ","")!="" ) { row["Target\nIdentification"] = Message.TAR; }
                else { row["Target\nIdentification"] = "No Data"; }
            }
            else { row["Target\nIdentification"] = "No Data"; }
            if (Message.TYP != null) { row["Target\nReport\nDescriptor"] = "Click to expand"; }
            else { row["Target\nReport\nDescriptor"] = "No Data"; }
            if (Message.MESSAGE_TYPE != null) { row["Message Type"] = Message.MESSAGE_TYPE; }
            else { row["Message Type"] = "No Data"; }
            if (Message.Flight_Level != null) { row["Flight Level"] = Message.Flight_Level; }
            else { row["Flight Level"] = "No Data"; }
            if (Message.Track_Number != null) { row["Track\nNumber"] = Message.Track_Number; }
            else { row["Track\nNumber"] = "No Data"; }
            if (Message.Time_Of_Day != null) { row["Time of\nDay"] = Message.Time_Of_Day; }
            else { row["Time of\nDay"] = "No Data"; }
            if (Message.CNF != null) { row["Track Status"] = "Click to expand"; }
            else { row["Track Status"] = "No Data"; }
            if (Message.Latitude_in_WGS_84 != null && Message.Longitude_in_WGS_84 != null) { row["Position in WGS-84 Co-ordinates"] =  Message.Latitude_in_WGS_84 + ", " + Message.Longitude_in_WGS_84; }
            else { row["Position in WGS-84 Co-ordinates"] = "No Data"; }
            if (Message.Position_Cartesian_Coordinates != null) { row["Position in\nCartesian\nCo-ordinates"] = Message.Position_Cartesian_Coordinates; }
            else { row["Position in\nCartesian\nCo-ordinates"] = "No Data"; }
            if (Message.Position_In_Polar != null) { row["Position in\nPolar\nCo-ordinates"] = Message.Position_In_Polar; }
            else { row["Position in\nPolar\nCo-ordinates"] = "No Data"; }
            if (Message.Track_Velocity_Polar_Coordinates != null) { row["Track Velocity in Polar Coordinates"] = Message.Track_Velocity_Polar_Coordinates; }
            else { row["Track Velocity in Polar Coordinates"] = "No Data"; }
            if (Message.Track_Velocity_in_Cartesian_Coordinates != null) { row["Track Velocity in\nCartesian\nCoordinates"] = Message.Track_Velocity_in_Cartesian_Coordinates; }
            else { row["Track Velocity in\nCartesian\nCoordinates"] = "No Data"; }
            if (Message.Target_size_and_orientation != null) { row["Target Size\nand\nOrientation"] = Message.Target_size_and_orientation; }
            else { row["Target Size\nand\nOrientation"] = "No Data"; }
            if (Message.Target_Address != null) { row["Target\nAddress"] = Message.Target_Address; }
            else { row["Target\nAddress"] = "No Data"; }
            if (Message.NOGO != null) { row["System\nStatus"] = "Click to expand"; }
            else { row["System\nStatus"] = "No Data"; }
            if (Message.VFI != null) { row["Vehicle Fleet\nIdentification"] = Message.VFI; }
            else { row["Vehicle Fleet\nIdentification"] = "No Data"; }
            if (Message.Pre_programmed_message != null) { row["Pre-programmed\nMessage"] = Message.Pre_programmed_message; }
            else { row["Pre-programmed\nMessage"] = "No Data"; }
            if (Message.Measured_Height != null) { row["Measured\nHeight"] = Message.Measured_Height; }
            else { row["Measured\nHeight"] = "No Data"; }
            if (Message.Mode_3A != null) { row["Mode-3A\nCode"] = Message.Mode_3A; }
            else { row["Mode-3A\nCode"] = "No Data"; }
            if (Message.MB_Data != null) { row["Mode S MB\nData"] = "Click to expand"; }
            else { row["Mode S MB\nData"] = "No Data"; }
            if (Message.Deviation_X != null) { row["Standard\nDeviation\nof Position"] = "Click to expand"; }
            else { row["Standard\nDeviation\nof Position"] = "No Data"; }
            if (Message.REP_Presence != 0) { row["Presence"] = "Click to expand"; }
            else { row["Presence"] = "No Data"; }
            if (Message.PAM != null) { row["Amplitude\nof Primary\nPlot"] = Message.PAM; }
            else { row["Amplitude\nof Primary\nPlot"] = "No Data"; }
            if (Message.Calculated_Acceleration != null) { row["Calculated\nAcceleration"] = Message.Calculated_Acceleration; }
            else { row["Calculated\nAcceleration"] = "No Data"; }
            tablaCAT10.Rows.Add(row);
        }

        /// <summary>
        /// Adds a row to datatable Cat21 v.2.1 from Class Cat21v21 message
        /// </summary>
        /// <param name="Message">Instance of Class Cat21v21 containing one messge</param>
        private void AddRowTable21v21(CAT21vs21 Message)
        {
            var row = tablaCAT21v21.NewRow();
            row["Number"] = Message.num;
            row["CAT number"] = Message.cat21v21num;
            if (Message.CAT != null) { row["Category"] = Message.CAT; }
            else { row["Category"] = "No Data"; }
            if (Message.SAC != null) { row["SAC"] = Message.SAC; }
            else { row["SAC"] = "No Data"; }
            if (Message.SIC != null) { row["SIC"] = Message.SIC; }
            else { row["SIC"] = "No Data"; }
            if (Message.Target_Identification != null) { row["Target\nIdentification"] = Message.Target_Identification; }
            else { row["Target\nIdentification"] = "No Data"; }
            if (Message.ATP != null) { row["Target\nReport\nDescriptor"] = "Click to expand"; }
            else { row["Target\nReport\nDescriptor"] = "No Data"; }
            if (Message.Track_Number != null) { row["Track\nNumber"] = Message.Track_Number; }
            else { row["Track\nNumber"] = "No Data"; }
            if (Message.Service_Identification != null) { row["Service\nIdentification"] = Message.Service_Identification; }
            else { row["Service\nIdentification"] = "No Data"; }
            if (Message.Time_of_Applicability_Position != null) { row["Time of\nApplicability\nfor Position"] = Message.Time_of_Applicability_Position; }
            else { row["Time of\nApplicability\nfor Position"] = "No Data"; }
            if (Message.LatitudeWGS_84 != null && Message.LongitudeWGS_84 != null) { row["Position in WGS-84 co-ordinates"] =  Message.LatitudeWGS_84 + ", " + Message.LongitudeWGS_84; }
            else { row["Position in WGS-84 co-ordinates"] = "No Data"; }
            if (Message.High_Resolution_LatitudeWGS_84 != null && Message.High_Resolution_LongitudeWGS_84 != null) { row["Position in WGS-84 co-ordinates high res"] =  Message.High_Resolution_LatitudeWGS_84 + ", " + Message.High_Resolution_LongitudeWGS_84; }
            else { row["Position in WGS-84 co-ordinates high res"] = "No Data"; }
            if (Message.Time_of_Applicability_Velocity != null) { row["Time of\nApplicability\nfor Velocity"] = Message.Time_of_Applicability_Velocity; }
            else { row["Time of\nApplicability\nfor Velocity"] = "No Data"; }
            if (Message.Air_Speed != null) { row["Air\nSpeed"] = Message.Air_Speed; }
            else { row["Air\nSpeed"] = "No Data"; }
            if (Message.True_Air_Speed != null) { row["True Air\nSpeed"] = Message.True_Air_Speed; }
            else { row["True Air\nSpeed"] = "No Data"; }
            if (Message.Target_address != null) { row["Target Address"] = Message.Target_address; }
            else { row["Target Address"] = "No Data"; }
            if (Message.Time_of_Message_Reception_Position != null) { row["Time of\nMessage\nReception\nof Position"] = Message.Time_of_Message_Reception_Position; }
            else { row["Time of\nMessage\nReception\nof Position"] = "No Data"; }
            if (Message.Time_of_Message_Reception_Velocity != null) { row["Time of\nMessage\nReception\nof Velocity"] = Message.Time_of_Message_Reception_Velocity; }
            else { row["Time of\nMessage\nReception\nof Velocity"] = "No Data"; }
            if (Message.Geometric_Height != null) { row["Geometric\nHeight"] = Message.Geometric_Height; }
            else { row["Geometric\nHeight"] = "No Data"; }
            if (Message.NUCr_NACv != null) { row["Quality\nIndicators"] = "Click to expand"; }
            else { row["Quality\nIndicators"] = "No Data"; }
            if (Message.MOPS != null) { row["MOPS Version"] = Message.MOPS; }
            else { row["MOPS Version"] = "No Data"; }
            if (Message.ModeA3 != null) { row["Mode-3A\nCode"] = Message.ModeA3; }
            else { row["Mode-3A\nCode"] = "No Data"; }
            if (Message.Roll_Angle != null) { row["Roll\nAngle"] = Message.Roll_Angle; }
            else { row["Roll\nAngle"] = "No Data"; }
            if (Message.Flight_Level != null) { row["Flight\nLevel"] = Message.Flight_Level; }
            else { row["Flight\nLevel"] = "No Data"; }
            if (Message.Magnetic_Heading != null) { row["Magnetic\nHeading"] = Message.Magnetic_Heading; }
            else { row["Magnetic\nHeading"] = "No Data"; }
            if (Message.ICF != null) { row["Target\nStatus"] = "Click to expand"; }
            else { row["Target\nStatus"] = "No Data"; }
            if (Message.Barometric_Vertical_Rate != null) { row["Barometric\nVertical Rate"] = Message.Barometric_Vertical_Rate; }
            else { row["Barometric\nVertical Rate"] = "No Data"; }
            if (Message.Geometric_Vertical_Rate != null) { row["Geometric\nVertical Rate"] = Message.Geometric_Vertical_Rate; }
            else { row["Geometric\nVertical Rate"] = "No Data"; }
            if (Message.Ground_vector != null) { row["Airborne Ground Vector"] = Message.Ground_vector; }
            else { row["Airborne Ground Vector"] = "No Data"; }
            if (Message.Track_Angle_Rate != null) { row["Track\nAngle\nRate"] = Message.Track_Angle_Rate; }
            else { row["Track\nAngle\nRate"] = "No Data"; }
            if (Message.Time_of_Asterix_Report_Transmission != null) { row["Time of\nReport\nTransmission"] = Message.Time_of_Asterix_Report_Transmission; }
            else { row["Time of\nReport\nTransmission"] = "No Data"; }
            if (Message.ECAT != null) { row["Emitter Category"] = Message.ECAT; }
            else { row["Emitter Category"] = "No Data"; }
            if (Message.MET_present != 0) { row["Met\nInformation"] = "Click to expand"; }
            else { row["Met\nInformation"] = "No Data"; }
            if (Message.Selected_Altitude != null) { row["Selected Altitude"] = Message.Selected_Altitude; }
            else { row["Selected Altitude"] = "No Data"; }
            if (Message.MV != null) { row["Final\nState\nSelected\nAltitude"] = "Click to expand"; }
            else { row["Final\nState\nSelected\nAltitude"] = "No Data"; }
            if (Message.Trajectory_present != 0) { row["Trajectory\nIntent"] = "Click to expand"; }
            else { row["Trajectory\nIntent"] = "No Data"; }
            if (Message.RP != null) { row["Service\nManagement"] = Message.RP; }
            else { row["Service\nManagement"] = "No Data"; }
            if (Message.RA != null) { row["Aircraft\nOperational\nStatus"] = "Click to expand"; }
            else { row["Aircraft\nOperational\nStatus"] = "No Data"; }
            if (Message.POA != null)
            {
                if (Message.POA.Length > 25) { row["Surface\nCapabilities\nand\nCharacteristics"] = "Click to expand"; }
                else { row["Surface\nCapabilities\nand\nCharacteristics"] = "No Data"; }
            }
            else { row["Surface\nCapabilities\nand\nCharacteristics"] = "No Data"; }
            if (Message.Message_Amplitude != null) { row["Message\nAmplitude"] = Message.Message_Amplitude; }
            else { row["Message\nAmplitude"] = "No Data"; }
            if (Message.MB_Data != null) { row["Mode S MB Data"] = "Click to expand"; }
            else { row["Mode S MB Data"] = "No Data"; }
            if (Message.TYP != null) { row["ACAS\nResolution\nAdvisory\nReport"] = "Click to expand"; }
            else { row["ACAS\nResolution\nAdvisory\nReport"] = "No Data"; }
            if (Message.Receiver_ID != null) { row["Receiver\nID"] = Message.Receiver_ID; }
            else { row["Receiver\nID"] = "No Data"; }
            if (Message.Data_Ages_present != 0) { row["Data Ages"] = "Click to expand"; }
            else { row["Data Ages"] = "No Data"; }
            tablaCAT21v21.Rows.Add(row);
        }

        /// <summary>
        /// Adds a row to datatable cat 21 v0.23 from Class Cat21v 0.23 message
        /// </summary>
        /// <param name="Message">Instance of Class Cat21v0.23 containing one messge</param>
        private void AddRowTable21v23(CAT21vs23 Message)
        {
            var row = tablaCAT21v23.NewRow();
            row["Number"] = Message.num;
            row["CAT number"] = Message.cat21v23num;
            if (Message.CAT != null) { row["Category"] = Message.CAT; }
            else { row["Category"] = "No Data"; }
            if (Message.SAC != null) { row["SAC"] = Message.SAC; }
            else { row["SAC"]  = "No Data"; }
            if (Message.SIC != null) { row["SIC"] = Message.SIC; }
            else { row["SIC"] = "No Data"; }
            if (Message.Target_Identification != null) { row["Target\nIdentification"] = Message.Target_Identification; }
            else { row["Target\nIdentification"] = "No Data"; }
            if (Message.ATP != null) { row["Target\nReport\nDescriptor"] = "Click to expand"; } //
            else { row["Target\nReport\nDescriptor"] = "No Data"; }
            if (Message.Time_of_Day != null) { row["Time of\nDay"] = Message.Time_of_Day; }
            else { row["Time of\nDay"] = "No Data"; }
            if (Message.Time_of_Day_Accuracy != null) { row["Time of\nDay\nAccuracity"] = Message.Time_of_Day_Accuracy; }
            else { row["Time of\nDay\nAccuracity"] = "No Data"; }
            if (Message.LatitudeWGS_84 != null && Message.LongitudeWGS_84 != null) { row["Position in WGS-84 co-ordinates"] = "Latitude: " + Message.LatitudeWGS_84 + ", Longitude: " + Message.LongitudeWGS_84; }
            else { row["Position in WGS-84 co-ordinates"] = "No Data"; }
            if (Message.AC != null) { row["Figure of\nMerit"] = "Click to expand"; }//
            else { row["Figure of\nMerit"] = "No Data"; }
            if (Message.DTI != null) { row["Link\nTechnology"] = "Click to expand"; } //
            else { row["Link\nTechnology"] = "No Data"; }
            if (Message.Air_Speed != null) { row["Air\nSpeed"] = Message.Air_Speed; }
            else { row["Air\nSpeed"] = "No Data"; }
            if (Message.True_Air_Speed != null) { row["True Air\nSpeed"] = Message.True_Air_Speed; }
            else { row["True Air\nSpeed"] = "No Data"; }
            if (Message.Velocity_Accuracy != null) { row["Velocity\nAccuracy"] = Message.Velocity_Accuracy; }
            else { row["Velocity\nAccuracy"] = "No Data"; }
            if (Message.Target_address != null) { row["Target\nAddress"]  = Message.Target_address; }
            else { row["Target\nAddress"] = "No Data"; }
            if (Message.Geometric_Altitude != null) { row["Geometric\nAltitude"] = Message.Geometric_Altitude; }
            else { row["Geometric\nAltitude"] = "No Data"; }
            if (Message.Ground_Vector != null) { row["Ground Vector"] = Message.Ground_Vector; }
            else { row["Ground Vector"] = "No Data"; }
            if (Message.Rate_of_turn != null) { row["Rate of\nTurn"] = Message.Rate_of_turn; }
            else { row["Rate of\nTurn"] = "No Data"; }
            if (Message.Roll_Angle != null) { row["Roll\nAngle"] = Message.Roll_Angle; }
            else { row["Roll\nAngle"] = "No Data"; }
            if (Message.Flight_Level != null) { row["Flight\nLevel"] = Message.Flight_Level; }
            else { row["Flight\nLevel"] = "No Data"; }
            if (Message.Magnetic_Heading != null) { row["Magnetic\nHeading"] = Message.Magnetic_Heading; }
            else { row["Magnetic\nHeading"] = "No Data"; }
            if (Message.ICF != null) { row["Target\nStatus"] = "Click to expand"; } //
            else { row["Target\nStatus"] = "No Data"; }
            if (Message.Barometric_Vertical_Rate != null) { row["Barometric\nVertical\nRate"] = Message.Barometric_Vertical_Rate; }
            else { row["Barometric\nVertical\nRate"] = "No Data"; }
            if (Message.Geometric_Vertical_Rate != null) { row["Geometric\nVertical\nRate"] = Message.Geometric_Vertical_Rate; }
            else { row["Geometric\nVertical\nRate"] = "No Data"; }
            if (Message.ECAT != null) { row["Emitter\nCategory"] = Message.ECAT; }
            else { row["Emitter\nCategory"] = "No Data"; }
            if (Message.MET_present != 0) { row["Met\nInformation"] = "Click to expand"; } //
            else { row["Met\nInformation"] = "No Data"; }
            if (Message.Selected_Altitude != null) { row["Intermediate\nState\nSelected\nAltitude"] = Message.Selected_Altitude; }
            else { row["Intermediate\nState\nSelected\nAltitude"] = "No Data"; }
            if (Message.MV != null) { row["Final State\nSelected\nAltitude"] = "Click to expand"; } //
            else { row["Final State\nSelected\nAltitude"] = "No Data"; }
            if (Message.Trajectory_present != 0) { row["Trajectory\nIntent"] = "Click to expand"; } //
            else { row["Trajectory\nIntent"] = "No Data"; }
            if (Message.ModeA3 != null) { row["Mode-3A\nCode"] = Message.ModeA3; }
            else { row["Mode-3A\nCode"] = "No Data"; }
            if (Message.Signal_Amplitude!=null) { row["Signal\nAmplitude"] = Message.Signal_Amplitude; }
            else { row["Signal\nAmplitude"] = "No Data";  }
            tablaCAT21v23.Rows.Add(row);
        }

        private void AddRowTable62(CAT62 Message)
        {
        }


        /// <summary>
        /// Adds a row to datatable cat all from Class Cat10 message
        /// </summary>
        /// <param name="Message">Instance of Class Cat10 containing one messge</param>
        private void AddRowTableAllCat10(CAT10 Message)
        {
            var row = tablaAll.NewRow();
            row["Number"] = Message.num;
            row["CAT number"] = Message.cat10num;
            if (Message.CAT != null) { row["Category"] = Message.CAT; }
            else { row["Category"] = "No Data"; }
            if (Message.SAC != null) { row["SAC"] = Message.SAC; }
            else { row["SAC"] = "No Data"; }
            if (Message.SIC != null) { row["SIC"] = Message.SIC; }
            else { row["SIC"] = "No Data"; }
            if (Message.TAR != null) { row["Target\nIdentification"] = Message.TAR; }
            else { row["Target\nIdentification"] = "No Data"; }
            if (Message.Target_Address != null) { row["Target\nAddress"] = Message.Target_Address; }
            else { row["Target\nAddress"] = "No Data"; }
            if (Message.Time_Of_Day != null) { row["Time of\nDay"] = Message.Time_Of_Day; }
            else { row["Time of\nDay"] = "No Data"; }
            if (Message.Track_Number != null) { row["Track\nNumber"] = Message.Track_Number; }
            else { row["Track\nNumber"] = "No Data"; }
            if (Message.Latitude_in_WGS_84 != null && Message.Longitude_in_WGS_84 != null) { row["Position in WGS-84 co-ordinates"] = Message.Latitude_in_WGS_84 + ", " + Message.Longitude_in_WGS_84; }
            else { row["Position in WGS-84 co-ordinates"] = "No Data"; }
            if (Message.Flight_Level != null) { row["Flight\nLevel"] = Message.Flight_Level; }
            else { row["Flight\nLevel"] = "No Data"; }
            if (Message.Mode_3A != null) { row["Mode-3A\nCode"] = Message.Mode_3A; }
            else { row["Mode-3A\nCode"] = "No Data"; }
            if (Message.TYP != null) { row["Target\nReport\nDescriptor"] = "Click to expand"; }
            else { row["Target\nReport\nDescriptor"] = "No Data"; }
            tablaAll.Rows.Add(row);

        }

        /// <summary>
        /// Adds a row to datatable cat all from Class Cat21v21 message
        /// </summary>
        /// <param name="Message">Instance of Class Cat21v21 containing one messge</param>
        private void AddRowTableAllCat21v21(CAT21vs21 Message)
        {
            var row = tablaAll.NewRow();
            row["Number"] = Message.num;
            row["CAT number"] = Message.cat21v21num;
            if (Message.CAT != null) { row["Category"] = Message.CAT; }
            else { row["Category"] = "No Data"; }
            if (Message.SAC != null) { row["SAC"] = Message.SAC; }
            else { row["SAC"] = "No Data"; }
            if (Message.SIC != null) { row["SIC"] = Message.SIC; }
            else { row["SIC"] = "No Data"; }
            if (Message.Target_Identification != null) { row["Target\nIdentification"] = Message.Target_Identification; }
            else { row["Target\nIdentification"] = "No Data"; }
            if (Message.Target_address != null) { row["Target\nAddress"] = Message.Target_address; }
            else { row["Target\nAddress"] = "No Data"; }
            if (Message.Time_of_Asterix_Report_Transmission != null) { row["Time of\nDay"] = Message.Time_of_Asterix_Report_Transmission; }
            else { row["Time of\nDay"] = "No Data"; }
            if (Message.Track_Number != null) { row["Track\nNumber"] = Message.Track_Number; }
            else { row["Track\nNumber"] = "No Data"; }
            if (Message.LatitudeWGS_84 != null && Message.LongitudeWGS_84 != null) { row["Position in WGS-84 co-ordinates"] = Message.LatitudeWGS_84 + ", " + Message.LongitudeWGS_84; }
            else { row["Position in WGS-84 co-ordinates"] = "No Data"; }
            if (Message.Flight_Level != null) { row["Flight\nLevel"] = Message.Flight_Level; }
            else { row["Flight\nLevel"] = "No Data"; }
            if (Message.ModeA3 != null) { row["Mode-3A\nCode"] = Message.ModeA3; }
            else { row["Mode-3A\nCode"] = "No Data"; }
            if (Message.ATP != null) { row["Target\nReport\nDescriptor"] = "Click to expand"; }
            else { row["Target\nReport\nDescriptor"] = "No Data"; }
            tablaAll.Rows.Add(row);
        }

 

        /// <summary>
        /// Adds a row to datatable cat all from Class Cat21v23 message
        /// </summary>
        /// <param name="Message">Instance of Class Cat21v23 containing one messge</param>
        private void AddRowTableAllCat21v23(CAT21vs23 Message)
        {
            var row = tablaAll.NewRow();
            row["Number"] = Message.num;
            row["CAT number"] = Message.cat21v23num;
            if (Message.CAT != null) { row["Category"] = Message.CAT; }
            else { row["Category"] = "No Data"; }
            if (Message.SAC != null) { row["SAC"] = Message.SAC; }
            else { row["SAC"] = "No Data"; }
            if (Message.SIC != null) { row["SIC"] = Message.SIC; }
            else { row["SIC"] = "No Data"; }
            if (Message.Target_Identification != null) { row["Target\nIdentification"] = Message.Target_Identification; }
            else { row["Target\nIdentification"] = "No Data"; }
            if (Message.Target_address != null) { row["Target\nAddress"] = Message.Target_address; }
            else { row["Target\nAddress"] = "No Data"; }
            if (Message.Time_of_Day != null) { row["Time of\nDay"] = Message.Time_of_Day; }
            else { row["Time of\nDay"] = "No Data"; }
            if (Message.LatitudeWGS_84 != null && Message.LongitudeWGS_84 != null) { row["Position in WGS-84 co-ordinates"] = Message.LatitudeWGS_84 + ", " + Message.LongitudeWGS_84; }
            else { row["Position in WGS-84 co-ordinates"] = "No Data"; }
            if (Message.Flight_Level != null) { row["Flight\nLevel"] = Message.Flight_Level; }
            else { row["Flight\nLevel"] = "No Data"; }
            if (Message.ATP != null) { row["Target\nReport\nDescriptor"] = "Click to expand"; } //
            else { row["Target\nReport\nDescriptor"] = "No Data"; }
            row["Track\nNumber"] = "No Data";
            if (Message.ModeA3 != null) { row["Mode-3A\nCode"] = Message.ModeA3; }
            else { row["Mode-3A\nCode"] = "No Data"; }
            tablaAll.Rows.Add(row);
        }

        private void AddRowTableAllCat62(CAT62 Message)
        {
            var row = tablaAll.NewRow();
            row["Number"] = Message.num;
            row["CAT number"] = Message.cat62num;
            if (Message.CAT != null) { row["Category"] = Message.CAT; }
            else { row["Category"] = "No Data"; }
            if (Message.SAC != null) { row["SAC"] = Message.SAC; }
            else { row["SAC"] = "No Data"; }
            if (Message.SIC != null) { row["SIC"] = Message.SIC; }
            else { row["SIC"] = "No Data"; }
            if (Message.Target_Identification != null) { row["Target\nIdentification"] = Message.Target_Identification; }
            else { row["Target\nIdentification"] = "No Data"; }
            if (Message.Derived_Data_Address != null) { row["Target\nAddress"] = Message.Time_of_Track_Information; }
            else { row["Target\nAddress"] = "No Data"; }
            if (Message.Time_of_Track_Information != null) { row["Time of\nDay"] = Message.Time_of_Track_Information; }
            else { row["Time of\nDay"] = "No Data"; }
            if (Message.LatitudeWGS_84 != null && Message.LongitudeWGS_84 != null) { row["Position in WGS-84 co-ordinates"] = Message.LatitudeWGS_84 + ", " + Message.LongitudeWGS_84; }
            else { row["Position in WGS-84 co-ordinates"] = "No Data"; }
            if (Message.Measured_Flight_Level != null) { row["Flight\nLevel"] = Message.Measured_Flight_Level; }
            else { row["Flight\nLevel"] = "No Data"; }
            if (Message.Derived_Data_REP != null) { row["Target\nReport\nDescriptor"] = "Click to expand"; } //
            else { row["Target\nReport\nDescriptor"] = "No Data"; }
            row["Track\nNumber"] = "No Data";
            if (Message.ModeA3 != null) { row["Mode-3A\nCode"] = Message.ModeA3; }
            else { row["Mode-3A\nCode"] = "No Data"; }
            tablaAll.Rows.Add(row);
        }

        /// <summary>
        /// Computes the direction of all messages of a CATALL List.
        /// </summary>
        /// <param name="List">CAT ALL List contaning all the messages</param>
        private void ComputeDirecction(List<CATALL> List)
        {
            process = "Computing trajectories...";

            process = "Applying trajectories...";

            int i = 0;
            
            /*Once trajectories are created, we will walk trhought the CATALL list and search the direction of each message*/
            foreach (CATALL message in List)
            {
                i++;
                process = "Applying trajectory for message " + i + " of " + Convert.ToString(List.Count) + " messages...";

                if (message.Latitude_in_WGS_84 != -200 && message.Longitude_in_WGS_84 != -200)
                {
                    if (message.DetectionMode == "SMR")
                    {
                        FindDirection(SMRTrajectories, message);
                    }
                    if (message.DetectionMode == "MLAT")
                    {
                        FindDirection(MLATTrajectories, message);
                    }
                    if (message.DetectionMode == "ADSB")
                    {
                        FindDirection(ADSBTrajectories, message);
                    }
                    if(message.DetectionMode=="CAT 62")
                    {
                        FindDirection(CAT62Trajectories, message);
                    }
                }
            }
        }




        /// <summary>
        /// Order all messages from a CATALL list int trajectories. Each trajectory is a list contaning all the messages from a vehicle 
        /// 
        /// Trajectories are not returned. The changes in trajectories are applied in the trajectories list of this class. Every time function is called trajectories are reseted
        /// </summary>
        /// <param name="List">List of which we want to compute the trajectories</param>
        private void ComputeTrajectories(List<CATALL> List)
        {
            SMRTrajectories = new List<Trajectories>(); 
            MLATTrajectories = new List<Trajectories>();
            ADSBTrajectories = new List<Trajectories>();
            CAT62Trajectories = new List<Trajectories>();
            int i = 0;

            foreach (CATALL message in List)
            {
                process = "Computing trajectory for message " + i +" of " + Convert.ToString(List.Count)+" messages...";
                i++;
               
                if (message.Latitude_in_WGS_84 != -200 && message.Longitude_in_WGS_84 != -200)
                {
                    if (message.DetectionMode == "SMR" )
                    {
                        if (message.Target_Identification != null)
                        {
                            if (SMRTrajectories.Exists(x => x.Target_Identification == message.Target_Identification)) { SMRTrajectories.Find(x => x.Target_Identification == message.Target_Identification).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                SMRTrajectories.Add(traj);
                         
                            }
                        }
                        else if (message.Target_Address != null)
                        {
                            if (SMRTrajectories.Exists(x => x.Target_Address == message.Target_Address)) { SMRTrajectories.Find(x => x.Target_Address == message.Target_Address).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                SMRTrajectories.Add(traj);       

                            }
                        }
                        else if (message.Track_number != null)
                        {
                            if (SMRTrajectories.Exists(x => x.Track_number == message.Track_number)) { SMRTrajectories.Find(x => x.Track_number == message.Track_number).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                SMRTrajectories.Add(traj);
                            }
                        }
                    }
                    else if (message.DetectionMode == "MLAT")
                    {
                        if (message.Target_Identification != null)
                        {
                            if (MLATTrajectories.Exists(x => x.Target_Identification == message.Target_Identification)) { MLATTrajectories.Find(x => x.Target_Identification == message.Target_Identification).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                MLATTrajectories.Add(traj);

                            }
                        }
                        else if (message.Target_Address != null)
                        {
                            if (MLATTrajectories.Exists(x => x.Target_Address == message.Target_Address)) { MLATTrajectories.Find(x => x.Target_Address == message.Target_Address).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                MLATTrajectories.Add(traj);

                            }
                        }
                        else if (message.Track_number != null)
                        {
                            if (MLATTrajectories.Exists(x => x.Track_number == message.Track_number)) { MLATTrajectories.Find(x => x.Track_number == message.Track_number).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                MLATTrajectories.Add(traj);
                            }
                        }
                    }
                    else if (message.DetectionMode == "ADSB")
                    {
                        if (message.Target_Identification != null)
                        {
                            if (ADSBTrajectories.Exists(x => x.Target_Identification == message.Target_Identification)) { ADSBTrajectories.Find(x => x.Target_Identification == message.Target_Identification).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                ADSBTrajectories.Add(traj);
                            }
                        }
                        else if (message.Target_Address != null)
                        {
                            if (ADSBTrajectories.Exists(x => x.Target_Address == message.Target_Address)) { ADSBTrajectories.Find(x => x.Target_Address == message.Target_Address).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                ADSBTrajectories.Add(traj);
                            }
                        }
                        else if (message.Track_number != null)
                        {
                            if (ADSBTrajectories.Exists(x => x.Track_number == message.Track_number)) { ADSBTrajectories.Find(x => x.Track_number == message.Track_number).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                ADSBTrajectories.Add(traj);
                            }
                        }
                    }
                    else if (message.DetectionMode == "CAT 62")
                    {
                        if (message.Target_Identification != null)
                        {
                            if (CAT62Trajectories.Exists(x => x.Target_Identification == message.Target_Identification)) { CAT62Trajectories.Find(x => x.Target_Identification == message.Target_Identification).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                CAT62Trajectories.Add(traj);
                            }
                        }
                        else if (message.Target_Address != null)
                        {
                            if (CAT62Trajectories.Exists(x => x.Target_Address == message.Target_Address)) { CAT62Trajectories.Find(x => x.Target_Address == message.Target_Address).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                CAT62Trajectories.Add(traj);
                            }
                        }
                        else if (message.Track_number != null)
                        {
                            if (CAT62Trajectories.Exists(x => x.Track_number == message.Track_number)) { CAT62Trajectories.Find(x => x.Track_number == message.Track_number).AddTimePoint(message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.Time_Of_day); }
                            else
                            {
                                Trajectories traj = new Trajectories(message.Target_Identification, message.Time_Of_day, message.Latitude_in_WGS_84, message.Longitude_in_WGS_84, message.type, message.Target_Address, message.DetectionMode, message.CAT, message.SAC, message.SIC, message.Track_number);
                                CAT62Trajectories.Add(traj);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Clears all the trajectories
        /// </summary>
        private void ClearTrajectories()
        {
            SMRTrajectories = new List<Trajectories>();
            MLATTrajectories = new List<Trajectories>();
            ADSBTrajectories = new List<Trajectories>();
            CAT62Trajectories = new List<Trajectories>();
        }

        /// <summary>
        /// Given a List of trajectories and a CATALL message computes the direction of this message
        /// </summary>
        /// <param name="listtraj">List of trajectories between which is the one of the message of which we look for the direction</param>
        /// <param name="message">message of which we are looking for direcion</param>
        private void FindDirection(List<Trajectories> listtraj, CATALL message)
        {
            if (message.Target_Identification != null && message.Target_Identification.Count() > 1 && listtraj.Exists(x => x.Target_Identification == message.Target_Identification))
            {
                Trajectories t = listtraj.Find(x => x.Target_Identification == message.Target_Identification);
                int index = t.ListTimePoints.FindIndex(x => x.time == message.Time_Of_day);

                ComputedirectionFromindex(t, index, message);
            }

            else if (message.Target_Address != null && listtraj.Exists(x => x.Target_Address == message.Target_Address))
            {
                Trajectories t = listtraj.Find(x => x.Target_Address == message.Target_Address);
                int index = t.ListTimePoints.FindIndex(x => x.time == message.Time_Of_day);
                ComputedirectionFromindex(t, index, message);
            }

            else if (message.Track_number != null && listtraj.Exists(x => x.Track_number == message.Track_number))
            {
                Trajectories t = listtraj.Find(x => x.Track_number == message.Track_number);
                int index = t.ListTimePoints.FindIndex(x => x.time == message.Time_Of_day);
                ComputedirectionFromindex(t, index, message);
            }
        }

        /// <summary>
        /// Finds the direction of a message from the trajectory of this message, the index and the message
        /// 
        /// The function does not return anything, direction is set in the parameter of the CAT ALL class instance
        /// </summary>
        /// <param name="t">trajectory of the message</param>
        /// <param name="index">index of which position the message ocupies in the trajectory</param>
        /// <param name="message">Message of whioch we are looking for direction</param>
        private void ComputedirectionFromindex(Trajectories t, int index, CATALL message)
        {
            if (t.CountTimepoint() > 2)
            {
                if ((index + 2) < t.CountTimepoint())
                {

                    PointWithTime p = t.ListTimePoints[index + 1];
                    if (t.ListTimePoints[index + 1].time == message.Time_Of_day)
                    {
                        bool dif = false;
                        int i = 2;
                        while (dif == false)
                        {
                            p = t.ListTimePoints[index + i];
                            if (p.time != message.Time_Of_day) { dif = true; }
                            if ((index + i + 1) >= t.CountTimepoint()) { dif = true; }
                            i++;
                        }
                    }
                    double X = p.point.Lng - message.Longitude_in_WGS_84;
                    double Y = p.point.Lat - message.Latitude_in_WGS_84;
                    int direction = 100;
                    double dir = 0;
                    dir = (Math.Atan2(Y, X) * (180 / Math.PI));
                    try
                    {
                        direction = Convert.ToInt32(dir);
                    }
                    catch { direction = 0; }
                    if (message.type == "car")
                    {
                        direction = -(direction - 180);
                    }
                    else if (message.type == "plane")
                    {
                        direction = -(direction - 45);
                    }
                    message.direction = direction;
                }
                else if ((index + 1) < t.CountTimepoint())
                {
                    double X;
                    double Y;
                    int direction;
                    double dir;
                    PointWithTime p = t.ListTimePoints[index + 1];
                    if (t.ListTimePoints[index + 1].time != message.Time_Of_day || index == 0)
                    {
                        p = t.ListTimePoints[index + 1];
                        X = p.point.Lng - message.Longitude_in_WGS_84;
                        Y = p.point.Lat - message.Latitude_in_WGS_84;

                        dir = (Math.Atan2(Y, X) * (180 / Math.PI));
                        try
                        {
                            direction = Convert.ToInt32(dir);
                        }
                        catch { direction = 0; }
                    }

                    else
                    {
                        p = t.ListTimePoints[index - 1];
                        X = message.Longitude_in_WGS_84 - p.point.Lng;
                        Y = message.Latitude_in_WGS_84 - p.point.Lat;
                        dir = (Math.Atan2(Y, X) * (180 / Math.PI));
                        try
                        {
                            direction = Convert.ToInt32(dir);
                        }
                        catch { direction = 0; }

                    }

                    if (message.type == "car")
                    {
                        direction = -(direction - 180);
                    }
                    else if (message.type == "plane")
                    {
                        direction = -(direction - 45);
                    }
                    message.direction = direction;

                }
                else
                {
                    try
                    {
                        PointWithTime p = t.ListTimePoints[index - 1];
                        double X = message.Longitude_in_WGS_84 - p.point.Lng;
                        double Y = message.Latitude_in_WGS_84 - p.point.Lat;
                        int direction = 100;
                        double dir = (Math.Atan2(Y, X) * (180 / Math.PI));
                        try
                        {
                            direction = Convert.ToInt32(dir);
                        }
                        catch { direction = 0; }
                        if (message.type == "car")
                        {
                            direction = -(direction - 180);
                        }
                        else if (message.type == "plane")
                        {
                            direction = -(direction - 45);
                        }
                        message.direction = direction;
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Compute detection ratio of every message from a list. The detection ratio is computed taking into
        /// account all the messages of each radar in the same file. Radar can be configured to detect every X seconds,
        /// it is not a fixed value. But if it will be the same value for the same radar in the same file. In this way, 
        /// by calculating the refresh rate of each vehicle on the same radar in the same file, making the average, and rounding it, 
        /// we can obtain the refresh rate value of that radar. Once we have it calculated, we insert it in each message,
        /// in this way we can have it accessible from the map, which is where we are interested in knowing that value
        /// </summary>
        /// <param name="List">List to which we want to apply the detection ratio</param>
        private void ComputeDetectionRatio(List<CATALL> List)
        {
            process = "Computing radars detection ratio...";
            double ADSBratio = 0;
            int ADSBRatio;
            int ADSBCount = 0;

            if (ADSBTrajectories.Count() > 0)
            {
                foreach (Trajectories t in ADSBTrajectories)
                {
                    if (t.CountTimepoint() > 3)
                    {
                        double r = (t.ListTimePoints[t.ListTimePoints.Count() - 1].time - t.ListTimePoints[0].time) / (t.ListTimePoints.Count() - 1);
                        if (r < 5)
                        {
                            ADSBCount++;
                            ADSBratio += r;
                        }
                    }
                }
                ADSBRatio = Convert.ToInt32(ADSBratio / ADSBCount);
                if (ADSBRatio < 1) { ADSBRatio = 1; }
            }
            else
            {
                ADSBRatio = 1;
            }
            process = "Applying radars detection ratio...";
            foreach (CATALL message in List)
            {
                if (message.CAT == "21 v. 2.1")
                {
                    message.refreshratio = ADSBRatio;
                }
                else
                {
                    message.refreshratio = 1;
                }
            }
        }

        /// <summary>
        /// Computes the time of each message taking into acount the days
        /// </summary>
        /// <param name="list">List to which we want to compute time</param>
        /// <returns></returns>
        private List<CATALL> ComputeTimeOfDay(List<CATALL> list)
        {
            List<CATALL> List = new List<CATALL>();
            List = list.OrderBy(CATAll => CATAll.List_Time_Of_Day).ToList();
            int firsttime = List[0].List_Time_Of_Day;
            int lasttime = List[List.Count - 1].List_Time_Of_Day;
            double da = (lasttime - firsttime) / 86400;
            int days = Convert.ToInt32(Math.Truncate(da)) + 1;
            if (List[0].List_Time_Of_Day < 0)
            {
                double fir = (firsttime / 86400);
                int firstday = Convert.ToInt32(Math.Truncate(fir)) - 1;
                foreach (CATALL mess in List)
                {
                    mess.Time_Of_day = mess.List_Time_Of_Day + (-firstday * 86400);
                }
            }
            else
            {
                foreach (CATALL mess in List)
                {
                    mess.Time_Of_day = mess.List_Time_Of_Day;
                }
            }
            return List;
        }

    }
}
 
 
 
 
 