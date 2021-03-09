using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// PROBA BRANCA
/// </summary>

namespace PGTAWPF
{
    /// <summary>
    /// Class for decoding CAT 21 version 2.1 messages
    /// </summary>
    /// <param name="lib">Decoding Library is a library with useful functions for all categories</param>
    /// <param name="mensaje">String [] containing all the octets of the message to decode </param>
    /// <param name="FSPEC1">FSPEC string, removing bits indicating continuity / end of FSPEC</param>
    /// <param name="CAT">String indicating the cat</param>
    /// <param name="num">Unique number for each message. They are assigned starting at 0 and growing, as messages are decoded. It will be used to identify specific messages</param>
    /// <param name="cat21v21num">Unique number for each message, the same as the parameter "num", but this is unique within this category, and not unique in all categories.</param>
    /// <param name="airportCode">Code that identifies to which airport this message belongs</param>
    /// 
    ///Within the class there are many more parameters. 
    ///The parameters that are at the beginning of a function are typical parameters of the message parameter that is being decoded. 
    ///For more information on any of those parameters, refer to the Eurocontrol documentation on Asterix.

    public class CAT62
    {
        readonly LibreriaDecodificacion lib;
        readonly string FSPEC1;
        readonly string[] mensaje;
        public string CAT = "62";
        public int num;
        public int cat62num;
        public int airportCode;

        #region Constructors

        public CAT62() { }

        public CAT62(string[] mensajehexa, LibreriaDecodificacion lib)
        {
            //try
            //{
                this.lib = lib;
                this.mensaje = mensajehexa;
                FSPEC1 = lib.FSPEC(mensaje); //Get FSPEC string 
                int longFSPEC = this.FSPEC1.Length / 7; //get number of octets used by FSPEC
                int pos = 3 + longFSPEC; //Start octet will be 3 plus number of octets used by FSPEC
                char[] FSPEC = FSPEC1.ToCharArray(0, FSPEC1.Length); //Convert FSPEC to char array to work in a more practical way
                this.mensaje = lib.Passarmensajeenteroabinario(mensaje);//Pass the message from hexadecimal to binary to work in a more practical way

                /* From now on each function looks to see if the decoding parameter exists in the 
                 message (checking if the FSPEC in its position == 1) and if it exists calls the function to decode the parameter */

                if (FSPEC[0] == '1') { pos = this.Compute_Data_Source_Identification(mensaje, pos); }
                if (FSPEC[2] == '1') { pos = this.Compute_Service_Identification(mensaje, pos); }
                if (FSPEC[3] == '1') { pos = this.Compute_Time_of_Track_Information(mensaje, pos); }
                if (FSPEC[4] == '1') { pos = this.Compute_PositionWGS_84(mensaje, pos); }
                if (FSPEC[5] == '1') { pos = this.Compute_Position_in_Cartesian_Coordinates(mensaje, pos); }
                if (FSPEC[6] == '1') { pos = this.Compute_Track_Velocity_in_Cartesian_Coordinates(mensaje, pos); }
                if (FSPEC.Count() > 8)
                {
                    if (FSPEC[7] == '1') { pos = this.Compute_Calculated_Acceleration(mensaje, pos); }
                    if (FSPEC[8] == '1') { pos = this.Compute_Mode_A3(mensaje, pos); }
                    if (FSPEC[9] == '1') { pos = this.Compute_Target_Identification(mensaje, pos); }
                    if (FSPEC[10] == '1') { pos = this.Compute_Aircraft_Derived_Data(mensaje, pos); }
                    if (FSPEC[11] == '1') { pos = this.Compute_Track_Number(mensaje, pos); }
                    if (FSPEC[12] == '1') { pos = this.Compute_Target_Status(mensaje, pos); }
                    if (FSPEC[13] == '1') { pos = this.Compute_System_Track_Update_Ages(mensaje, pos); }
            }
            if (FSPEC.Count() > 16)
            {
                if (FSPEC[14] == '1') { pos = this.Compute_Mode_of_Movement(mensaje, pos); }
                if (FSPEC[15] == '1') { pos = this.Compute_System_Track_Data_Ages(mensaje, pos); }
                if (FSPEC[16] == '1') { pos = this.Compute_Flight_level(mensaje, pos); }
                if (FSPEC[17] == '1') { pos = this.Compute_Calculated_Track_Geometric_Altitude(mensaje, pos); }
                if (FSPEC[18] == '1') { pos = this.Compute_Calculated_Track_Barometric_Altitude(mensaje, pos); }
                if (FSPEC[19] == '1') { pos = this.Compute_Calculated_Rate_of_Climb(mensaje, pos); }
                if (FSPEC[20] == '1') { pos = this.Compute_Flight_Plan_Related_Data(mensaje, pos); }
            }
            if (FSPEC.Count() > 22)
            {
                if (FSPEC[21] == '1') { pos = this.Compute_Target_Size_and_Orientation(mensaje, pos); }
                if (FSPEC[22] == '1') { pos = this.Compute_Vehicle_Fleet_Identificatior(mensaje, pos); }
                if (FSPEC[23] == '1') { pos = this.Compute_Mode_5_Data_Reports(mensaje, pos); }
                if (FSPEC[24] == '1') { pos = this.Compute_Mode_2_code(mensaje, pos); }
                if (FSPEC[25] == '1') { pos = this.Compute_Composed_Track_Number(mensaje, pos); }
                if (FSPEC[26] == '1') { pos = this.Compute_Estimated_accuracies(mensaje, pos); }
                if (FSPEC[27] == '1') { pos = this.Compute_System_Measured_Information(mensaje, pos); }
            }
            //}
            //catch
            //{
            //    mensaje = mensajehexa;
            //}
            this.lib = null;
        }

        #region Data Item I021/010, Data Source Identification

        /// <summary>
        ///  Data Item I021/010, Data Source Identification
        /// 
        /// Definition : Identification of the ADS-B station providing information.
        /// Format : Two-octet fixed length Data Item.
        /// </summary>

        public string SAC;
        public string SIC;
        private int Compute_Data_Source_Identification(string[] message, int pos)
        {
            SAC = Convert.ToString(Convert.ToInt32(message[pos], 2));
            SIC = Convert.ToString(Convert.ToInt32(message[pos + 1], 2));
            this.airportCode = lib.GetAirporteCode(Convert.ToInt32(SIC)); //Computes airport code from SIC 
            pos += 2;
            return pos;
        }

        #endregion


        #endregion

        #region Data Item I021/015, Service Identification
        /// <summary>
        /// Data Item I021/015, Service Identification
        /// 
        /// Definition : Identification of the service provided to one or more users.
        /// Format : One-Octet fixed length data item.
        /// </summary>
        public string Service_Identification;
        private int Compute_Service_Identification(string[] message, int pos)
        {
            Service_Identification = Convert.ToString(Convert.ToInt32(message[pos], 2));
            pos++;
            return pos;
        }

        #endregion


        #region Data Item I021/161, Track Number
        /// <summary>
        /// Data Item I021/161, Track Number
        /// 
        /// Definition: An integer value representing a unique reference to a track record within a particular track file.
        /// Format: Two-octet fixed length Data Item.
        /// </summary>
        public string Track_Number;
        private int Compute_Track_Number(string[] message, int pos)
        {
            Track_Number = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2));
            pos += 2;
            return pos;
        }

        #endregion


        #region Data Item I021/070, Mode 3/A Code in Octal Representation
        /// <summary>
        /// Data Item I021/070, Mode 3/A Code in Octal Representation
        /// 
        /// Definition: Mode-3/A code converted into octal representation.
        /// Format: Two-octet fixed length Data Item.
        /// </summary>
        public string ModeA3;

        public string ModeA3_Has_Changed;

        public string FullA3Info;
        private int Compute_Mode_A3(string[] message, int pos)
        {
            ModeA3 = Convert.ToString(lib.ConvertDecimalToOctal(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4, 12), 2))).PadLeft(4, '0');
            if (message[pos].Substring(3, 1) == "0")
            {
                ModeA3_Has_Changed = "No Change";
            }
            else
            {
                ModeA3_Has_Changed = "Mode 3/A has changed";

            }
            FullA3Info = ModeA3_Has_Changed + ", value:" + ModeA3;
            pos += 2;
            return pos;
        }

        #endregion



        #region Time of Track Information

        public string Time_of_Track_Information;
        public int Time_of_day_sec;
        public double Time_of_day_milisec;
        private int Compute_Time_of_Track_Information(string[] message, int pos)
        {

            int str = Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2);
            double segundos = (Convert.ToDouble(str) / 128);
            Time_of_day_milisec =segundos;
            Time_of_day_sec = Convert.ToInt32(Math.Truncate(segundos));
            TimeSpan tiempo = TimeSpan.FromSeconds(segundos);
            Time_of_Track_Information = tiempo.ToString(@"hh\:mm\:ss\:fff");
            pos += 3;
            return pos;
        }

        #endregion


        #region Data Item I062/080, Track Status
        /// <summary>
        /// Data Item I021/200, Target Status
        /// 
        /// Definition : Status of the target
        /// Format : One-octet fixed length Data Item
        /// </summary>
        public string MON;
        public string SPI;
        public string MRH;
        public string SRC;
        public string CNF;
        public string SIM;
        public string TSE;
        public string TSB;
        public string FPC;
        public string AFF;
        public string STP;
        public string KOS;
        public string AMA;
        public string MD4;
        public string ME;
        public string MI;
        public string MD5;
        public string CST;
        public string PSR;
        public string SSR;
        public string MDS;
        public string ADS;
        public string SUC;
        public string AAC;
        public string SDS;
        public string EMS;
        public string PFT; 
        public string FPLT;
        public string DUPT;
        public string DUPF;
        public string DUPM;
        public string SFC;
        public string IDD;
        public string IEC;


        private int Compute_Target_Status(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0")
            {
                MON = "Multisensor";
            }
            else
            {
                MON = "Monosensor track";
            }
            if (message[pos].Substring(1, 1) == "0")
            {
                SPI = "SPI default value";
            }
            else
            {
                SPI = "SPI present in the last report received from a sensor capable of decoding this data";
            }
            if (message[pos].Substring(2, 1) == "0")
            {
                MRH = "Barometric altitude (Mode C) more reliable ";
            }
            else
            {
                MRH = "Geometric altitude more reliable";
            }
            string src = message[pos].Substring(3, 3);
            if (src == "000")
            {
                SRC = "No source";
            }
            else if (src == "001")
            {
                SRC = "GNSS";
            }
            else if (src == "010")
            {
                SRC = "3D radar";
            }
            else if (src == "011")
            {
                SRC = "Triangulation";
            }
            else if (src == "100")
            {
                SRC = "Height from coverage";
            }
            else if (src == "101")
            {
                SRC = "Speed look-up table";
            }
            else if (src == "110")
            {
                SRC = "Default height";
            }
            else if (src == "111")
            {
                SRC = "Multilateration";
            }
            if (message[pos].Substring(6, 1) == "0")
            {
                CNF = "Confirmed track";
            }
            else
            {
                CNF = "Tentative track";
            }
            if (message[pos].Substring(7, 1) == "1")
            {
                pos++;
                if (message[pos].Substring(0, 1) == "0")
                {
                    SIM = "Actual Track";
                }
                else
                {
                    SIM = "Simulated Track";
                }
                if (message[pos].Substring(1, 1) == "0")
                {
                    TSE = "TSE Default value";
                }
                else
                {
                    TSE = "Last message transmitted to the user for the track k";
                }
                if (message[pos].Substring(2, 1) == "0")
                {
                    TSB = "TSB Default value";
                }
                else
                {
                    TSB = "First message transmitted to the user  for the track";
                }
                if (message[pos].Substring(3, 1) == "0")
                {
                    FPC = "Not flight-plan correlated";
                }
                else
                {
                    FPC = "Flight plan correlated";
                }
                if (message[pos].Substring(4, 1) == "0")
                {
                    AFF = "AFF Default value";
                }
                else
                {
                    AFF = "ADS-B data inconsistent with other surveillance information ";
                }
                if (message[pos].Substring(5, 1) == "0")
                {
                    STP = "STP Default value";
                }
                else
                {
                    STP = "Slave Track Promotion";
                }
                if (message[pos].Substring(6, 1) == "0")
                {
                    KOS = "Complementary service used";
                }
                else
                {
                    KOS = "Background service used";
                }
                if (message[pos].Substring(7, 1) == "1")
                {
                    pos++;
                    if (message[pos].Substring(0, 1) == "0")
                    {
                        AMA = "Track not resulting from amalgamation process";
                    }
                    else
                    {
                        AMA = "Track resulting from amalgamation process";
                    }
                    string md4 = message[pos].Substring(1, 2);
                    if (md4 == "00")
                    {
                        MD4 = "No Mode 4 interrogation";
                    }
                    else if (md4 == "01")
                    {
                        MD4 = "Friendly target";
                    }
                    else if (md4 == "10")
                    {
                        MD4 = "Unknown target";
                    }
                    else if (md4 == "11")
                    {
                        MD4 = "No reply";
                    }
                    if (message[pos].Substring(3, 1) == "0")
                    {
                        ME = "ME Default value";
                    }
                    else
                    {
                        ME = "Military Emergency present in the last report received from a sensor capable of decoding this data ";
                    }
                    if (message[pos].Substring(4, 1) == "0")
                    {
                        MI = "MI Default value";
                    }
                    else
                    {
                        MI = "Military Identification present in the last report received from a sensor capable of decoding this data";
                    }

                    string md5 = message[pos].Substring(5, 2);
                    if (md5 == "00")
                    {
                        MD5 = "No Mode 5 interrogation";
                    }
                    else if (md5 == "01")
                    {
                        MD5 = "Friendly target";
                    }
                    else if (md5 == "10")
                    {
                        MD5 = "Unknown target";
                    }
                    else if (md5 == "11")
                    {
                        MD5 = "No reply";
                    }
                    if (message[pos].Substring(7, 1) == "1")
                    {
                        pos++;
                        if (message[pos].Substring(0, 1) == "0")
                        {
                            CST = "CST Default value ";
                        }
                        else
                        {
                            CST = "Age of the last received track update is higher than system dependent threshold(coasting)";
                        }
                        if (message[pos].Substring(1, 1) == "0")
                        {
                            PSR = "PSR Default value";
                        }
                        else
                        {
                            PSR = "Age of the last received PSR track update is higher than system dependent threshold";
                        }
                        if (message[pos].Substring(2, 1) == "0")
                        {
                            SSR = " SSR Default value";
                        }
                        else
                        {
                            SSR = "First message transmitted to the user  for the track";
                        }
                        if (message[pos].Substring(3, 1) == "0")
                        {
                            MDS = "MDS Default value";
                        }
                        else
                        {
                            MDS = "Age of the last received Mode S track update is higher than system dependent threshold";
                        }
                        if (message[pos].Substring(4, 1) == "0")
                        {
                            ADS = "ADS Default value";
                        }
                        else
                        {
                            ADS = "Age of the last received ADS-B track update is higher than system dependent threshold ";
                        }
                        if (message[pos].Substring(5, 1) == "0")
                        {
                            SUC = "SUC Default value";
                        }
                        else
                        {
                            SUC = "Special Used Code (Mode A codes to be defined in the system to mark a track with special interest)";
                        }
                        if (message[pos].Substring(6, 1) == "0")
                        {
                            AAC = "Complementary service used";
                        }
                        else
                        {
                            AAC = "Assigned Mode A Code Conflict (same discrete Mode A Code assigned to another track)";
                        }
                        if (message[pos].Substring(7, 1) == "1")
                        {
                            pos++;
                            string ssd = message[pos].Substring(0, 2);
                            if(ssd=="00")
                            {
                                SDS = "Combined";
                            }
                            else if (ssd == "01")
                            {
                                SDS = "Cooperative only";
                            }
                            else if (ssd == "10")
                            {
                                SDS = "Non-Cooperative only";
                            }
                            else if (ssd == "11")
                            {
                                SDS = "Not defined";
                            }
                            int ems= Convert.ToInt32(message[pos].Substring(2,3),2);
                            if (ems == 0)
                            {
                                EMS = "No emergency";
                            }
                            else if (ems == 1)
                            {
                                EMS = "General emergency";
                            }
                            else if (ems == 2)
                            {
                                EMS = "Lifeguard / medical";
                            }
                            else if (ems == 3)
                            {
                                EMS = "Minimum fuel";
                            }
                            else if (ems == 4)
                            {
                                EMS = "No communications";
                            }
                            else if (ems == 5)
                            {
                                EMS = "Unlawful interference";
                            }
                            else if (ems == 6)
                            {
                                EMS = "“Downed” Aircraft";
                            }
                            else if (ems == 7)
                            {
                                EMS = "Undefined";
                            }
                            if(message[pos].Substring(5,1)=="0")
                            {
                                PFT = "No indication";
                            }
                            else
                            {
                                PFT = "Potential False Track Indication";
                            }
                            if (message[pos].Substring(6, 1) == "0")
                            {
                                FPLT = "Default value";
                            }
                            else
                            {
                                FPLT = "Track created / updated with FPL data";
                            }
                            if (message[pos].Substring(7, 1) == "1")
                            {
                                pos++;
                                if (message[pos].Substring(0, 1) == "0")
                                {
                                    DUPT = "DUPT Default value";
                                }
                                else
                                {
                                    DUPT= "Duplicate Mode 3/A Code";
                                }
                                if (message[pos].Substring(1, 1) == "0")
                                {
                                    DUPF= "DUPF Default value";
                                }
                                else
                                {
                                    DUPF = "Duplicate Flight Plan";
                                }
                                if (message[pos].Substring(2, 1) == "0")
                                {
                                    DUPM = "DUPM Default value";
                                }
                                else
                                {
                                    DUPM = "Duplicate Flight Plan due to manual correlation";
                                }
                                if (message[pos].Substring(3, 1) == "0")
                                {
                                    SFC = "SFC Default value";
                                }
                                else
                                {
                                    SFC = "Surface target";
                                }
                                if (message[pos].Substring(4, 1) == "0")
                                {
                                    IDD = "IDD No indication";
                                }
                                else
                                {
                                    IDD = "Duplicate Flight-ID";
                                }
                                if (message[pos].Substring(5, 1) == "0")
                                {
                                    IEC = "IEC Default value";
                                }
                                else
                                {
                                    IEC = "Inconsistent Emergency Code";
                                }
                              
                            }
                        }
                    }
                }
            }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I010/042, Position in Cartesian Co-ordinates

        /// <summary>
        /// Data Item I010/042, Position in Cartesian Co-ordinates
        /// 
        /// Definition: Position of a target in Cartesian co-ordinates, in two’s complement form.
        /// Format: Four-octet fixed length Data Item.
        /// </summary>
        /// <param name="Position_Cartesian_Coordinates"> Position in string format for tables </param>
        /// <param name="X_Component_map">X componet to use in map</param>
        /// <param name="Y_Component_map">Y component to use in map</param>
        public double X_Component_map = -99999;
        public double Y_Component_map = -99999;
        public string Position_Cartesian_Coordinates;
        private int Compute_Position_in_Cartesian_Coordinates(string[] message, int pos)
        {
            X_Component_map = (lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * 0.5);
            Y_Component_map = (lib.ComputeA2Complement(string.Concat(message[pos + 3], message[pos + 4], message[pos + 5])) * 0.5);
            string X_Component = Convert.ToString(X_Component_map);
            string Y_Component = Convert.ToString(Y_Component_map);
            Position_Cartesian_Coordinates = "X: " + X_Component + ", Y: " + Y_Component;
            //    Point p = new Point(X_Component_map, Y_Component_map);
            //    PointLatLng position = lib.ComputeWGS_84_from_Cartesian(p, this.SIC); //Compute WGS84 position from cartesian position
            //    Set_WGS84_Coordinates(position); //Apply computed WGS84 position to this message
            pos += 6;
            return pos;
        }
        #endregion

        #region Data Item I021/130, Position in WGS-84 Co-ordinates

        /// <summary>
        /// Data Item I021/130, Position in WGS-84 Co-ordinates
        /// 
        /// Definition : Position in WGS-84 Co-ordinates.
        /// Format : Six-octet fixed length Data Item.
        /// </summary>
        /// <param name="Latitude_in_WGS_84">Latitude in Degrees minutes seconds that will show in the tables</param>
        /// <param name="Longitudee_in_WGS_84">Longitude in Degrees minutes seconds that will show in the tables</param>
        /// <param name="Latitude_in_WGS_84_map">Latitude in decimals used to draw markers on map</param>
        /// <param name="Latitude_in_WGS_84_map">Longiutde in decimals used to draw markers on map</param>

        public string LatitudeWGS_84;
        public string LongitudeWGS_84;
        public double LatitudeWGS_84_map = -200;
        public double LongitudeWGS_84_map = -200;
        private int Compute_PositionWGS_84(string[] message, int pos)
        {

            double Latitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3])) * (180 / (Math.Pow(2, 25)));
            pos += 4;
            double Longitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3])) * (180 / (Math.Pow(2, 25)));
            LatitudeWGS_84_map = Convert.ToDouble(Latitude);
            LongitudeWGS_84_map = Convert.ToDouble(Longitude);
            int Latdegres = Convert.ToInt32(Math.Truncate(Latitude));
            int Latmin = Convert.ToInt32(Math.Truncate((Latitude - Latdegres) * 60));
            double Latsec = Math.Round(((Latitude - (Latdegres + (Convert.ToDouble(Latmin) / 60))) * 3600), 2);
            int Londegres = Convert.ToInt32(Math.Truncate(Longitude));
            int Lonmin = Convert.ToInt32(Math.Truncate((Longitude - Londegres) * 60));
            double Lonsec = Math.Round(((Longitude - (Londegres + (Convert.ToDouble(Lonmin) / 60))) * 3600), 2);
            LatitudeWGS_84 = Convert.ToString(Latdegres) + "º " + Convert.ToString(Latmin) + "' " + Convert.ToString(Latsec) + "''";
            LongitudeWGS_84 = Convert.ToString(Londegres) + "º" + Convert.ToString(Lonmin) + "' " + Convert.ToString(Lonsec) + "''";
            pos += 4;
            return pos;
        }

        #endregion

        #region Mode 5 Data reports & extended mode 

        public bool Mode5_present = false;

        public string SUM;
        public string PMN;
        public string POS;
        public string GA;
        public string EM1;
        public string TOS;
        public string XP;


        public string M5;
        public string ID;
        public string DA;
        public string M1;
        public string M2;
        public string M3;
        public string MC;
        public string X;

        public int PIN;
        public int NAT;
        public int MIS;

        public string Mode5_pos_WGS86_Latitude;
        public string Mode5_pos_WGS86_Longitude;

        public string Mode5_GNSS_RES;
        public string Mode5_GNSS_GA;

        public string EM1_value;

        public double TOS_value = 0;

        public string X1;
        public string X2;
        public string X3;
        public string XC;
        public string X5;


        private int Compute_Mode_5_Data_Reports(string[] message, int pos)
        {
            SUM = message[pos].Substring(0, 1);
            PMN = message[pos].Substring(1, 1);
            POS = message[pos].Substring(2, 1);
            GA= message[pos].Substring(3, 1);
            EM1 = message[pos].Substring(4, 1);
            TOS = message[pos].Substring(5, 1);
            XP = message[pos].Substring(6, 1);
            pos++;
            if (SUM=="1")
            {
                Mode5_present = true;
                if(message[pos].Substring(0,1)=="0")
                {
                    M5 = "No Mode 5 interrogation";
                }
                else
                {
                    M5 = "Mode 5 interrogation";
                }
                if (message[pos].Substring(1, 1) == "0")
                {
                    ID = "No authenticated Mode 5 ID reply ";
                }
                else
                {
                    ID = "Authenticated Mode 5 ID reply";
                }
                if (message[pos].Substring(2, 1) == "0")
                {
                    DA = "No authenticated Mode 5 Data reply or Report ";
                }
                else
                {
                    DA = "Authenticated Mode 5 Data reply or Report(i.e any valid Mode 5 reply type other than ID) ";
                }
                if (message[pos].Substring(3, 1) == "0")
                {
                    M1 = "Mode 1 code not present or not from Mode 5 reply ";
                }
                else
                {
                    M1 = "Mode 1 code from Mode 5 reply. ";
                }
                if (message[pos].Substring(4, 1) == "0")
                {
                    M2 = "Mode 2 code not present or not from Mode 5 reply";
                }
                else
                {
                    M2 = "Mode 2 code from Mode 5 reply.";
                }
                if (message[pos].Substring(5, 1) == "0")
                {
                    M3 = "Mode 3 code not present or not from Mode 5 reply";
                }
                else
                {
                    M3 = "Mode 3 code from Mode 5 reply.";
                }
                if (message[pos].Substring(6, 1) == "0")
                {
                    MC = "Mode C altitude not present or not from Mode 5 reply";
                }
                else
                {
                    MC = "Mode C altitude from Mode 5 reply";
                }
                if (message[pos].Substring(7, 1) == "0")
                {
                    X = " X-pulse set to zero or no authenticated Data reply or Report received.";
                }
                else
                {
                    X = "X-pulse set to one.";
                }
                pos++;
            }
            if(PMN == "1")
            {
                Mode5_present = true;

                PIN = Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(2,14), 2);
                NAT = Convert.ToInt32(message[pos+2].Substring(4,5), 2);
                MIS = Convert.ToInt32(message[pos+3].Substring(3,6), 2);
                pos += 4;
            }
            if(POS=="1")
            {
                Mode5_present = true;

                double Latitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23)));
                pos += 3;
                double Longitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23)));
                pos += 3;
                int Latdegres = Convert.ToInt32(Math.Truncate(Latitude));
                int Latmin = Convert.ToInt32(Math.Truncate((Latitude - Latdegres) * 60));
                double Latsec = Math.Round(((Latitude - (Latdegres + (Convert.ToDouble(Latmin) / 60))) * 3600), 2);
                int Londegres = Convert.ToInt32(Math.Truncate(Longitude));
                int Lonmin = Convert.ToInt32(Math.Truncate((Longitude - Londegres) * 60));
                double Lonsec = Math.Round(((Longitude - (Londegres + (Convert.ToDouble(Lonmin) / 60))) * 3600), 2);
                Mode5_pos_WGS86_Latitude= Convert.ToString(Latdegres) + "º " + Convert.ToString(Latmin) + "' " + Convert.ToString(Latsec) + "''";
                Mode5_pos_WGS86_Longitude = Convert.ToString(Londegres) + "º" + Convert.ToString(Lonmin) + "' " + Convert.ToString(Lonsec) + "''";
               
            }
            if(GA=="1")
            {
                Mode5_present = true;

                if (message[pos].Substring(0, 1) == "1")
                {
                    Mode5_GNSS_RES = "GA reported in 100 ft increments";
                }
                else
                {
                    Mode5_GNSS_RES = "GA reported in 25 ft increments";
                }
               Mode5_GNSS_GA = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(2,14))*25);
                pos += 2;
            }
            if(EM1=="1")
            {
                Mode5_present = true;

                EM1_value = Convert.ToString(lib.ConvertDecimalToOctal(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4, 12), 2))).PadLeft(4, '0');
                pos += 2;
            }
            if(TOS=="1")
            {
                Mode5_present = true;

                TOS_value = lib.ComputeA2Complement(message[pos]) /128;
                pos++;
            }
            if (XP=="1")
            {
                Mode5_present = true;

                if (message[pos].Substring(3,1)=="0")
                {
                    X5 = "X-pulse set to zero or no authenticated Data reply or Report received.";
                }
                else
                {
                    X5 = "X-pulse set to one (present)";
                }
                if (message[pos].Substring(4, 1) == "0")
                {
                    XC = "X-pulse set to zero or no Mode C reply";
                }
                else
                {
                    XC="X - pulse set to one(present)";
                }
                if (message[pos].Substring(5, 1) == "0")
                {
                    X3 = "X-pulse set to zero or no Mode 3 / A reply";
                }
                else
                {
                    X3 = "X-pulse set to one (present)";
                }
                if (message[pos].Substring(6, 1) == "0")
                {
                    X2 = "X-pulse set to zero or no Mode 2 reply";
                }
                else
                {
                    X2 = "X-pulse set to one (present)";
                }
                if (message[pos].Substring(7, 1) == "0")
                {
                    X1 = "X-pulse set to zero or no Mode 1 reply ";
                }
                else
                {
                    X1 = "X-pulse set to one (present)";
                }
                pos++;
            }

            return pos;
        }

        #endregion

        #region Track mode 2 code

        public string Mode_2_code;

        private int Compute_Mode_2_code(string[] message, int pos)
        {
            EM1_value = Convert.ToString(lib.ConvertDecimalToOctal(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4, 12), 2))).PadLeft(4, '0');
            pos += 2;
            return pos;
        }
        #endregion


        #region Data Item I062/130, Calculated Track Geometric Altitude

        /// <summary>
        /// Data Item I021/140, Geometric Height
        /// 
        /// Definition : Minimum height from a plane tangent to the earth’s ellipsoid,
        /// defined by WGS-84, in two’s complement form.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string Calculated_Track_Geometric_Altitude;
        private int Compute_Calculated_Track_Geometric_Altitude(string[] message, int pos)
        {
            Calculated_Track_Geometric_Altitude = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 6.25) + " ft";
            pos += 2;
            return pos;
        }
        #endregion

        #region Data Item I062/135, Calculated Track Barometric Altitude 

        /// <summary>
        /// Data Item I021/140, Geometric Height
        /// 
        /// Definition : Minimum height from a plane tangent to the earth’s ellipsoid,
        /// defined by WGS-84, in two’s complement form.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        /// 
        public string Calculated_Track_Barometric_Altitude;
        public string QNH;
        private int Compute_Calculated_Track_Barometric_Altitude(string[] message, int pos)
        {
            if(message[pos].Substring(0,1)=="0")
            {
                QNH = "No QNH correction applied ";
            }
            else
            {
                QNH = "QNH correction applied ";
            }
            Calculated_Track_Geometric_Altitude = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(1,15)) * 0.25) + " FL";
            pos += 2;
            return pos;
        }
        #endregion


        #region Data Item I062/136, Measured Flight Level 


        /// <summary>
        /// Data Item I021/145, Flight Level
        /// 
        /// Definition: Flight Level from barometric measurements, not QNH corrected, in two’s complement form.
        /// Format: Two-Octet fixed length data item.
        /// </summary>
        public string Measured_Flight_Level;
        private int Compute_Flight_level(string[] message, int pos)
        {
            Measured_Flight_Level = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * (0.25)) + " FL";
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I062/185, Calculated Track Velocity (Cartesian) 

        /// <summary>
        /// Data Item I010/202, Calculated Track Velocity in Cartesian Co-ordinates
        /// 
        /// Definition: Calculated track velocity expressed in Cartesian co-ordinates, in two’s complement representation.
        /// Format: Four-octet fixed length Data Item.
        /// </summary>
        public string Vx;
        public string Vy;
        public string Track_Velocity_in_Cartesian_Coordinates;
        private int Compute_Track_Velocity_in_Cartesian_Coordinates(string[] message, int pos)
        {
            double vx = (lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 0.25);
            Vx = "Vx: " + Convert.ToString(vx) + " m/s, ";
            double vy = (lib.ComputeA2Complement(string.Concat(message[pos + 2], message[pos + 3])) * 0.25);
            Vy = "Vy: " + Convert.ToString(vy) + " m/s";
            Track_Velocity_in_Cartesian_Coordinates = Vx + Vy;
            pos += 4;
            return pos;
        }

        #endregion


        #region Data Item I062/200, Mode of Movement 

        public string TRANS;
        public string LONG;
        public string VERT;
        public string ADF;

        private int Compute_Mode_of_Movement(string[] message, int pos)
        {
            string trans = message[pos].Substring(0, 2);
            if(trans=="00")
            {
                TRANS = "Constant Course";
            }
            else if( trans=="01")
            {
                TRANS = "Right Turn";
            }
            else if (trans == "10")
            {
                TRANS = "Left Turn";
            }
            else if (trans == "11")
            {
                TRANS = "Undetermined";
            }
            string _long = message[pos].Substring(2, 2);
            if (_long == "00")
            {
                LONG = "Constant Groundspeed";
            }
            else if (_long == "01")
            {
                LONG = "Increasing Groundspeed";
            }
            else if (_long == "10")
            {
                LONG = "Decreasing Groundspeed";
            }
            else if (_long == "11")
            {
                LONG = "Undetermined";
            }
            string vert = message[pos].Substring(4, 2);
            if (vert == "00")
            {
                VERT = "Level";
            }
            else if (vert == "01")
            {
                VERT = "Climb";
            }
            else if (vert == "10")
            {
                VERT = "Descent";
            }
            else if (vert == "11")
            {
                VERT = "Undetermined";
            }
            if(message[pos].Substring(6,1)=="0")
            {
                ADF = "No altitude discrepancy";
            }
            else
            {
                ADF = "Altitude discrepancy";
            }
            pos++;
            return pos;
        }

        #endregion


        #region Data Item I062/210, Calculated Acceleration (Cartesian)

        /// <summary>
        /// Definition : Calculated Acceleration of the target, in two’s complement form.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string Ax;
        public string Ay;
        public string Calculated_Acceleration;
        private int Compute_Calculated_Acceleration(string[] message, int pos)
        {
            double ax = lib.ComputeA2Complement(message[pos]) * 0.25;
            double ay = lib.ComputeA2Complement(message[pos + 1]) * 0.25;
            if (Convert.ToInt32(ax) >= 31 || Convert.ToInt32(ax) <= -31) { Ax = "Ax exceed the max value or is the max value (+-31 m/s^2)"; }
            else { Ax = "Ax: " + Convert.ToString(ax) + "m/s^2"; }
            if (Convert.ToInt32(ay) >= 31 || Convert.ToInt32(ax) <= -31)
            { 
                Ay = "Ay exceed the max value or is the max value (+-31 m/s^2)"; 
            }
            else 
            { 
                Ay = "Ay: " + Convert.ToString(ay) + "m/s^2"; 
            }
            Calculated_Acceleration = Ax + " " + Ay;
            pos += 2;
            return pos;
        }

        #endregion


        #region Data Item I062/220, Calculated Rate Of Climb/Descent 

        public string Calculated_Rate_of_Climb;

        private int Compute_Calculated_Rate_of_Climb(string[] message, int pos)
        {
            Calculated_Rate_of_Climb = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 6.25)+"ft/min";
            pos += 2;
            return pos;
        }


        #endregion


        #region Data Item I010/245, Target Identification

        /// <summary>
        /// Data Item I010/245, Target Identification
        /// 
        /// Definition: Target (aircraft or vehicle) identification in 8 characters.
        /// Format: Seven-octet fixed length Data Item.
        /// </summary>
        public string STI;
        public string Target_Identification;
        public string TAR;
        private int Compute_Target_Identification(string[] message, int pos)
        {
            char[] OctetoChar = message[pos].ToCharArray(0, 8);
            int sti = Convert.ToInt32(string.Concat(OctetoChar[0], OctetoChar[1]), 2);
            if (sti == 0) { STI = "Callsign or registration downlinked from transponder"; }
            else if (sti == 1) { STI = "Callsign not downlinked from transponder"; }
            else if (sti == 2) { STI = "Registration not downlinked from transponder"; }
            else { STI = "Invalid"; }
            StringBuilder Identification = new StringBuilder();
            string octets = string.Concat(message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5], message[pos + 6]);
            for (int i = 0; i < 8; i++) { Identification.Append(lib.Compute_Char(octets.Substring(i * 6, 6))); }
            TAR = Identification.ToString().Trim();
            Target_Identification = TAR;
            pos += 7;
            return pos;
        }

        #endregion

        #region Data Item I062/270, Target Size & Orientation 


        /// <summary>
        /// Data Item I010/270, Target Size & Orientation
        /// 
        /// Definition: Target size defined as length and width of the detected target, and orientation.
        /// Format: Variable length Data Item comprising a first part of one octet, followed by one-octet extents as necessary.
        /// </summary>

        public string Target_size_and_orientation;
        public string LENGHT;
        public string ORIENTATION;
        public string WIDTH;
        private int Compute_Target_Size_and_Orientation(string[] message, int pos)
        {
            LENGHT = "Lenght:  " + Convert.ToString(Convert.ToInt32(message[pos].Substring(0, 7), 2)) + "m";
            Target_size_and_orientation = LENGHT;
            pos++;
            if (message[pos].Substring(7, 1) == "1")
            {
                ORIENTATION = "Orientation: " + Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos].Substring(0, 7), 2)) * (360 / 128)) + "°";
                Target_size_and_orientation = Target_size_and_orientation + ", " + ORIENTATION;
                pos++;
                if (message[pos].Substring(7, 1) == "1")
                {
                    WIDTH = "Widht: " + Convert.ToString(Convert.ToInt32(message[pos].Substring(0, 7), 2)) + "m";
                    Target_size_and_orientation = Target_size_and_orientation + ", " + WIDTH;
                    pos++;
                }
            }
            return pos;
        }

        #endregion


        #region Data Item I062/290, System Track Update Ages

        public bool Update_ages_present = false;

        public string Update_Ages_TRK;
        public string Update_Ages_PSR;
        public string Update_Ages_SSR;
        public string Update_Ages_MDS;
        public string Update_Ages_ADS;
        public string Update_Ages_ES;
        public string Update_Ages_VDL;

        public string Update_Ages_UAT="0";
        public string Update_Ages_LOP = "0";
        public string Update_Ages_MLT = "0";

        public string Update_Ages_TRK_value;
        public string Update_Ages_PSR_value;
        public string Update_Ages_SSR_value;
        public string Update_Ages_MDS_value;
        public string Update_Ages_ADS_value;
        public string Update_Ages_ES_value;
        public string Update_Ages_VDL_value;

        public string Update_Ages_UAT_value;
        public string Update_Ages_LOP_value;
        public string Update_Ages_MLT_value;


        private int Compute_System_Track_Update_Ages(string[] message, int pos)
        {
            Update_Ages_TRK = message[pos].Substring(0, 1);
            Update_Ages_PSR = message[pos].Substring(1, 1);
            Update_Ages_SSR = message[pos].Substring(2, 1);
            Update_Ages_MDS = message[pos].Substring(3, 1);
            Update_Ages_ADS = message[pos].Substring(4, 1);
            Update_Ages_ES = message[pos].Substring(5, 1);
            Update_Ages_VDL = message[pos].Substring(6, 1);
            pos++;
            if (message[pos].Substring(7, 1) == "1")
            {

                Update_Ages_UAT = message[pos].Substring(0, 1);
                Update_Ages_LOP = message[pos].Substring(1, 1);
                Update_Ages_MLT = message[pos].Substring(2, 1);
                pos++;
            }
            if(Update_Ages_TRK=="1")
            {
                Update_ages_present =true;
                Update_Ages_TRK_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if(Update_Ages_PSR == "1")
            {
                Update_ages_present = true;

                Update_Ages_PSR_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Update_Ages_SSR == "1")
            {
                Update_ages_present = true;

                Update_Ages_SSR_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Update_Ages_MDS == "1")
            {
                Update_ages_present = true;

                Update_Ages_MDS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Update_Ages_ADS == "1")
            {
                Update_ages_present = true;

                Update_Ages_ADS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(string.Concat(message[pos], message[pos+1]), 2)) * 0.25) + "s";
                pos+=2;
            }
            if (Update_Ages_ES == "1")
            {
                Update_ages_present = true;

                Update_Ages_ES_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Update_Ages_VDL == "1")
            {
                Update_ages_present = true;

                Update_Ages_VDL_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Update_Ages_UAT == "1")
            {
                Update_ages_present = true;

                Update_Ages_UAT_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Update_Ages_LOP == "1")
            {
                Update_ages_present = true;

                Update_Ages_LOP_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Update_Ages_MLT == "1")
            {
                Update_ages_present = true;

                Update_Ages_MLT_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            return pos;
        }


        #endregion


        #region Data Item I062/295, Track Data Ages 

        public bool Data_ages_present = false;

        public string Track_Ages_MFL;
        public string Track_Ages_MD1;
        public string Track_Ages_MD2;
        public string Track_Ages_MDA;
        public string Track_Ages_MD4;
        public string Track_Ages_MD5;
        public string Track_Ages_MHG;

        public string Track_Ages_IAS="0";
        public string Track_Ages_TAS = "0";
        public string Track_Ages_SAL = "0";
        public string Track_Ages_FSS = "0";
        public string Track_Ages_TID = "0";
        public string Track_Ages_COM = "0";
        public string Track_Ages_SAB = "0";

        public string Track_Ages_ACS = "0";
        public string Track_Ages_BVR = "0";
        public string Track_Ages_GVR = "0";
        public string Track_Ages_RAN = "0";
        public string Track_Ages_TAR = "0";
        public string Track_Ages_TAN = "0";
        public string Track_Ages_GSP = "0";

        public string Track_Ages_VUN = "0";
        public string Track_Ages_MET = "0";
        public string Track_Ages_EMC = "0";
        public string Track_Ages_POS = "0";
        public string Track_Ages_GAL = "0";
        public string Track_Ages_PUN = "0";
        public string Track_Ages_MB = "0";

        public string Track_Ages_IAR = "0";
        public string Track_Ages_MAC = "0";
        public string Track_Ages_BPS = "0";


        public string Track_Ages_MFL_value;
        public string Track_Ages_MD1_value;
        public string Track_Ages_MD2_value;
        public string Track_Ages_MDA_value;
        public string Track_Ages_MD4_value;
        public string Track_Ages_MD5_value;
        public string Track_Ages_MHG_value;

        public string Track_Ages_IAS_value;
        public string Track_Ages_TAS_value;
        public string Track_Ages_SAL_value;
        public string Track_Ages_FSS_value;
        public string Track_Ages_TID_value;
        public string Track_Ages_COM_value;
        public string Track_Ages_SAB_value;

        public string Track_Ages_ACS_value;
        public string Track_Ages_BVR_value;
        public string Track_Ages_GVR_value;
        public string Track_Ages_RAN_value;
        public string Track_Ages_TAR_value;
        public string Track_Ages_TAN_value;
        public string Track_Ages_GSP_value;
        
        public string Track_Ages_VUN_value;
        public string Track_Ages_MET_value;
        public string Track_Ages_EMC_value;
        public string Track_Ages_POS_value;
        public string Track_Ages_GAL_value;
        public string Track_Ages_PUN_value;
        public string Track_Ages_MB_value;
        
        public string Track_Ages_BPS_value;
        public string Track_Ages_IAR_value;
        public string Track_Ages_MAC_value;

        private int Compute_System_Track_Data_Ages(string[] message, int pos)
        {
            Track_Ages_MFL = message[pos].Substring(0, 1);
            Track_Ages_MD1 = message[pos].Substring(1, 1);
            Track_Ages_MD2 = message[pos].Substring(2, 1);
            Track_Ages_MDA = message[pos].Substring(3, 1);
            Track_Ages_MD4 = message[pos].Substring(4, 1);
            Track_Ages_MD5 = message[pos].Substring(5, 1);
            Track_Ages_MHG = message[pos].Substring(6, 1);
            if (message[pos].Substring(7, 1) == "1")
            {
                pos++;

                Track_Ages_IAS = message[pos].Substring(0, 1);
                Track_Ages_TAS = message[pos].Substring(1, 1);
                Track_Ages_SAL = message[pos].Substring(2, 1);
                Track_Ages_FSS = message[pos].Substring(3, 1);
                Track_Ages_TID = message[pos].Substring(4, 1);
                Track_Ages_COM = message[pos].Substring(5, 1);
                Track_Ages_SAB = message[pos].Substring(6, 1);
                if (message[pos].Substring(7, 1) == "1")
                {
                    pos++;

                    Track_Ages_ACS = message[pos].Substring(0, 1);
                    Track_Ages_BVR = message[pos].Substring(1, 1);
                    Track_Ages_GVR = message[pos].Substring(2, 1);
                    Track_Ages_RAN = message[pos].Substring(3, 1);
                    Track_Ages_TAR = message[pos].Substring(4, 1);
                    Track_Ages_TAN = message[pos].Substring(5, 1);
                    Track_Ages_GSP = message[pos].Substring(6, 1);
                    if (message[pos].Substring(7, 1) == "1")
                    {
                        pos++;

                        Track_Ages_VUN = message[pos].Substring(0, 1);
                        Track_Ages_MET = message[pos].Substring(1, 1);
                        Track_Ages_EMC = message[pos].Substring(2, 1);
                        Track_Ages_POS = message[pos].Substring(3, 1);
                        Track_Ages_GAL = message[pos].Substring(4, 1);
                        Track_Ages_PUN = message[pos].Substring(5, 1);
                        Track_Ages_MB = message[pos].Substring(6, 1);
                        if (message[pos].Substring(7, 1) == "1")
                        {
                            pos++;

                            Track_Ages_IAR = message[pos].Substring(0, 1);
                            Track_Ages_MAC = message[pos].Substring(1, 1);
                            Track_Ages_BPS = message[pos].Substring(2, 1);                         
                        }
                    }
                }
            }
            pos++;

            if (Track_Ages_MFL == "1")
            {
                Data_ages_present = true;
                Track_Ages_MFL_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MD1 == "1")
            {
                Data_ages_present = true;

                Track_Ages_MD1_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MD2 == "1")
            {
                Data_ages_present = true;

                Track_Ages_MD2_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MDA == "1")
            {
                Data_ages_present = true;

                Track_Ages_MDA_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MD4 == "1")
            {
                Data_ages_present = true;

                Track_Ages_MD4_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MD5 == "1")
            {
                Data_ages_present = true;

                Track_Ages_MD5_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MHG == "1")
            {
                Data_ages_present = true;

                Track_Ages_MHG_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_IAS == "1")
            {
                Data_ages_present = true;

                Track_Ages_IAS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_TAS == "1")
            {
                Data_ages_present = true;

                Track_Ages_TAS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_SAL == "1")
            {
                Data_ages_present = true;

                Track_Ages_SAL_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_FSS == "1")
            {
                Data_ages_present = true;

                Track_Ages_FSS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_TID == "1")
            {
                Data_ages_present = true;

                Track_Ages_TID_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_COM == "1")
            {
                Data_ages_present = true;

                Track_Ages_COM_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_SAB == "1")
            {
                Data_ages_present = true;

                Track_Ages_SAB_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_ACS == "1")
            {
                Data_ages_present = true;

                Track_Ages_ACS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_BVR == "1")
            {
                Data_ages_present = true;

                Track_Ages_BVR_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_GVR == "1")
            {
                Data_ages_present = true;

                Track_Ages_GVR_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_RAN == "1")
            {
                Data_ages_present = true;

                Track_Ages_RAN_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_TAR == "1")
            {
                Data_ages_present = true;

                Track_Ages_TAR_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_TAN == "1")
            {
                Data_ages_present = true;

                Track_Ages_TAN_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_GSP == "1")
            {
                Data_ages_present = true;

                Track_Ages_GSP_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_VUN== "1")
            {
                Data_ages_present = true;

                Track_Ages_VUN_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MET == "1")
            {
                Data_ages_present = true;

                Track_Ages_MET_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_EMC == "1")
            {
                Data_ages_present = true;

                Track_Ages_EMC_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_POS == "1")
            {
                Data_ages_present = true;

                Track_Ages_POS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_GAL == "1")
            {
                Data_ages_present = true;

                Track_Ages_GAL_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_PUN == "1")
            {
                Data_ages_present = true;

                Track_Ages_PUN_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MB == "1")
            {
                Data_ages_present = true;

                Track_Ages_MB_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_IAR == "1")
            {
                Data_ages_present = true;

                Track_Ages_IAR_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_MAC == "1")
            {
                Data_ages_present = true;

                Track_Ages_MAC_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }
            if (Track_Ages_BPS == "1")
            {
                Data_ages_present = true;

                Track_Ages_BPS_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.25) + "s";
                pos++;
            }        
            return pos;
        }

        #endregion

        #region Data Item I062/300, Vehicle Fleet Identification 
        /// <summary>
        /// Data Item I010/300, Vehicle Fleet Identification
        /// 
        /// Definition: Vehicle fleet identification number.
        /// Format: One octet fixed length Data Item.
        /// </summary>
        public string VFI;
        private int Compute_Vehicle_Fleet_Identificatior(string[] message, int pos)
        {
            int vfi = Convert.ToInt32(message[pos], 2);
            if (vfi == 0) { VFI = "Unknown"; }
            else if (vfi == 1) { VFI = "ATC equipment maintenance"; }
            else if (vfi == 2) { VFI = "Airport maintenance"; }
            else if (vfi == 3) { VFI = "Fire"; }
            else if (vfi == 4) { VFI = "Bird scarer"; }
            else if (vfi == 5) { VFI = "Snow plough"; }
            else if (vfi == 6) { VFI = "Runway sweeper"; }
            else if (vfi == 7) { VFI = "Emergency"; }
            else if (vfi == 8) { VFI = "Police"; }
            else if (vfi == 9) { VFI = "Bus"; }
            else if (vfi == 10) { VFI = "Tug (push/tow)"; }
            else if (vfi == 11) { VFI = "Grass cutter"; }
            else if (vfi == 12) { VFI = "Fuel"; }
            else if (vfi == 13) { VFI = "Baggage"; }
            else if (vfi == 14) { VFI = "Catering"; }
            else if (vfi == 15) { VFI = "Aircraft maintenance"; }
            else if (vfi == 16) { VFI = "Flyco (follow me)"; }
            pos = pos++;
            return pos;
        }

        #endregion

        #region Data Item I062/340, Measured Information 

        public bool Measured_Information_present = false;

        public string Measured_Information_SID;
        public string Measured_Information_POS;
        public string Measured_Information_HEI;
        public string Measured_Information_MDC;
        public string Measured_Information_MDA;
        public string Measured_Information_TYP;

        public string Measured_Information_SAC;
        public string Measured_Information_SIC;

        public string Measured_Information_Polar_Position;

        public string Measured_Information_Height;

        public string Measured_Information_V;
        public string Measured_Information_G;
        public string Measured_Information_Mode_C_Code;

        public string Measured_Information_V_Mode_3A;
        public string Measured_Information_G_Mode_3A;
        public string Measured_Information_L_Mode_3A;
        public string Measured_Information_Mode_3A;

        public string Measured_Information_TYP_Value;
        public string Measured_Information_SIM_Value;
        public string Measured_Information_RAB_Value;
        public string Measured_Information_TST_Value;


        private int Compute_System_Measured_Information(string[] message, int pos)
        {
            Measured_Information_SID = message[pos].Substring(0, 1);
            Measured_Information_POS = message[pos].Substring(1, 1);
            Measured_Information_HEI = message[pos].Substring(2, 1);
            Measured_Information_MDC = message[pos].Substring(3, 1);
            Measured_Information_MDA = message[pos].Substring(4, 1);
            Measured_Information_TYP = message[pos].Substring(5, 1);
            pos++;

            if (Measured_Information_SID == "1")
            {
                Measured_Information_present = true;
                Measured_Information_SAC = Convert.ToString(Convert.ToInt32(message[pos], 2));
                Measured_Information_SIC = Convert.ToString(Convert.ToInt32(message[pos + 1], 2));
                this.airportCode = lib.GetAirporteCode(Convert.ToInt32(SIC)); //Computes airport code from SIC 
                pos += 2;
            }

            if(Measured_Information_POS=="1")
            {
                Measured_Information_present = true;

                double Range = Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2); //I suppose in meters
                string RHO;
                string THETA;
                if (Range >= 65536) 
                { 
                    RHO = "RHO exceds the max range or is the max range ";
                } //RHO = " + Convert.ToString(Range) + "m"; MessageBox.Show("RHO exceed the max range or is the max range, RHO = {0}m", Convert.ToString(Range)); }
                else
                { 
                    RHO = "ρ:" + Convert.ToString(Range) + "m";
                }//MessageBox.Show("RHO is the max range, RHO = {0}m", Convert.ToString(Range)); }
                THETA = ", θ:" + String.Format("{0:0.00}", Convert.ToDouble(Convert.ToInt32(string.Concat(message[pos + 2], message[pos + 3]), 2)) * (360 / (Math.Pow(2, 16)))) + "°"; //I suppose in degrees
                Measured_Information_Polar_Position = RHO + THETA;
                pos += 4;

            }

            if(Measured_Information_HEI=="1")
            {
                Measured_Information_present = true;

                Measured_Information_Height = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 25) + " ft";
                pos += 2;
            }
            if(Measured_Information_MDC=="1")
            {
                Measured_Information_present = true;

                if (message[pos].Substring(0,1)=="0")
                {
                    Measured_Information_V = "Code validated";

                }
                else
                {
                    Measured_Information_V = "Code not validated";
                }
                if (message[pos].Substring(1, 1) == "0")
                {
                    Measured_Information_G = "Default";

                }
                else
                {
                    Measured_Information_G = "Garbled code";
                }
                Measured_Information_Mode_C_Code = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(2,14)) * 0.25) + " FL";
                pos += 2;
            }

            if(Measured_Information_MDA=="1")
            {
                Measured_Information_present = true;

                char[] OctetoChar = message[pos].ToCharArray(0, 8);
                if (OctetoChar[0] == '0') 
                {
                    Measured_Information_V_Mode_3A = "V: Code validated"; 
                }
                else
                {
                    Measured_Information_V_Mode_3A = "V: Code not validated";
                }
                if (OctetoChar[1] == '0') 
                {
                    Measured_Information_G_Mode_3A = "G: Default"; 
                }
                else 
                {
                    Measured_Information_G_Mode_3A = "G: Garbled code";
                }
                if (OctetoChar[2] == '0') 
                {
                    Measured_Information_L_Mode_3A = "L: Mode-3/A code derived from the reply of the transponder";
                }
                else 
                {
                    Measured_Information_L_Mode_3A = "L: Mode-3/A code not extracted during the last scan";
                }
                Measured_Information_Mode_3A = Convert.ToString(lib.ConvertDecimalToOctal(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4, 12), 2))).PadLeft(4, '0');
                pos += 2;
            }

            if (Measured_Information_TYP == "1")
            {
                string typ = message[pos].Substring(0, 3);
                if (typ == "000")
                {
                    Measured_Information_TYP_Value = "No detection";
                }
                else if (typ == "001")
                {
                    Measured_Information_TYP_Value = "Single PSR detection";
                }
                else if (typ == "010")
                {
                    Measured_Information_TYP_Value = "Single SSR detection";
                }
                else if (typ == "011")
                {
                    Measured_Information_TYP_Value = "SSR + PSR detection";
                }
                else if (typ == "100")
                {
                    Measured_Information_TYP_Value = "Single ModeS All-Call";
                }
                else if (typ == "101")
                {
                    Measured_Information_TYP_Value = "Single ModeS Roll-Call";
                }
                else if (typ == "110")
                {
                    Measured_Information_TYP_Value = "ModeS All-Call + PSR";
                }
                else if (typ == "111")
                {
                    Measured_Information_TYP_Value = "ModeS Roll-Call +PSR";
                }
                if(message[pos].Substring(3,1)=="0")
                {
                    Measured_Information_SIM_Value = "Actual target report ";
                }
                else
                {
                    Measured_Information_SIM_Value = "Simulated target report ";
                }
                if (message[pos].Substring(4, 1) == "0")
                {
                    Measured_Information_RAB_Value = "Report from target transponder";
                }
                else
                {
                    Measured_Information_RAB_Value = "Report from field monitor (fixed transponder)";
                }
                if (message[pos].Substring(5, 1) == "0")
                {
                    Measured_Information_TST_Value = "Real target report ";
                }
                else
                {
                    Measured_Information_TST_Value = "Test target report";
                }
                pos++;
            }
            return pos;
        }

        #endregion


        #region Data Item I062/380, Aircraft Derived Data 

        public bool Derived_data_present = false;

        public string Derived_Data_ADR;
        public string Derived_Data_ID;
        public string Derived_Data_MHG;
        public string Derived_Data_IAS;
        public string Derived_Data_TAS;
        public string Derived_Data_SAL;
        public string Derived_Data_FSS;

        public string Derived_Data_TIS="0";
        public string Derived_Data_TID = "0";
        public string Derived_Data_COM = "0";
        public string Derived_Data_SAB = "0";
        public string Derived_Data_ACS = "0";
        public string Derived_Data_BVR = "0";
        public string Derived_Data_GVR = "0";

        public string Derived_Data_RAN = "0";
        public string Derived_Data_TAR = "0";
        public string Derived_Data_TAN = "0";
        public string Derived_Data_GSP = "0";
        public string Derived_Data_VUN = "0";
        public string Derived_Data_MET = "0";
        public string Derived_Data_EMC = "0";

        public string Derived_Data_POS = "0";
        public string Derived_Data_GAL = "0";
        public string Derived_Data_PUN = "0";
        public string Derived_Data_MB = "0";
        public string Derived_Data_IAR = "0";
        public string Derived_Data_MAC = "0";
        public string Derived_Data_BPS = "0";

        public string Derived_Data_Address;
        public string Derived_Data_Target_id;
        public string Derived_Data_MHG_value;
        public string Derived_Data_IAS_value;
        public string Derived_Data_TAS_value;
        public string Derived_Data_SAL_SAS_value;
        public string Derived_Data_SAL_Source_value;
        public string Derived_Data_SAL_Altitude_value;

        public string Derived_Data_FSS_MV_value;
        public string Derived_Data_FSS_AH_value;
        public string Derived_Data_FSS_AM_value;
        public string Derived_Data_FSS_Altitude_value;


        public string Derived_Data_TIS_NAV_value;
        public string Derived_Data_TIS_NVB_value;

        public int Derived_Data_REP;
        public string[] Derived_Data_TCA;
        public string[] Derived_Data_NC;
        public int[] Derived_Data_TCP;
        public string[] Derived_Data_Altitude;
        public string[] Derived_Data_Latitude;
        public string[] Derived_Data_Longitude;
        public string[] Derived_Data_Point_Type;
        public string[] Derived_Data_TD;
        public string[] Derived_Data_TRA;
        public string[] Derived_Data_TOA;
        public string[] Derived_Data_TOV;
        public string[] Derived_Data_TTR;

        public string Derived_Data_COM_Value;
        public string Derived_Data_COM_STAT;
        public string Derived_Data_COM_SSC;
        public string Derived_Data_COM_ARC;
        public string Derived_Data_COM_AIC;
        public string Derived_Data_COM_B1A;
        public string Derived_Data_COM_B1B;

        public string Derived_Data_SAB_AC;
        public string Derived_Data_SAB_MN;
        public string Derived_Data_SAB_DC;
        public string Derived_Data_SAB_GBS;
        public string Derived_Data_SAB_STAT;

        public string Derived_Data_ACS_TYP;
        public string Derived_Data_ACS_STYP;
        public string Derived_Data_ACS_ARA;
        public string Derived_Data_ACS_RAC;
        public string Derived_Data_ACS_RAT;
        public string Derived_Data_ACS_MTE;
        public string Derived_Data_ACS_TTI;
        public string Derived_Data_ACS_TID;

        public string Derived_Data_BVR_value;
        public string Derived_Data_GVR_value;
        public string Derived_Data_RAN_value;

        public string Derived_Data_TAR_TI;
        public string Derived_Data_TAR_value;
        public string Derived_Data_TAN_value;

        public string Derived_Data_GSP_value;
        public string Derived_Data_VUN_value;
        public string Derived_Data_MET_WS;
        public string Derived_Data_MET_WD;
        public string Derived_Data_MET_TMP;
        public string Derived_Data_MET_TRB;
        public string Derived_Data_MET_WS_value;
        public string Derived_Data_MET_WD_value;
        public string Derived_Data_MET_TMP_value;
        public string Derived_Data_MET_TRB_value;
        public string Derived_Data_EMC_ECAT;

        public string Derived_Data_POS_Latitude;
        public string Derived_Data_POS_Longitude;

        public string Derived_Data_GAL_value;
        public string Derived_Data_PUN_value;


        public string[] Derived_Data_MB_Data;
        public string[] Derived_Data_MB_BDS1;
        public string[] Derived_Data_MB_BDS2;
        public int Derived_Data_MB_modeS_rep;

        public string Derived_Data_IAR_value;
        public string Derived_Data_MAC_value;
        public string Derived_Data_BPS_value;

        private int Compute_Aircraft_Derived_Data(string[] message, int pos)
        {
            Derived_Data_ADR = message[pos].Substring(0, 1);
            Derived_Data_ID = message[pos].Substring(1, 1);
            Derived_Data_MHG = message[pos].Substring(2, 1);
            Derived_Data_IAS = message[pos].Substring(3, 1);
            Derived_Data_TAS = message[pos].Substring(4, 1);
            Derived_Data_SAL = message[pos].Substring(5, 1);
            Derived_Data_FSS = message[pos].Substring(6, 1);
            if (message[pos].Substring(7, 1) == "1")
            {
                pos++;
                Derived_Data_TIS = message[pos].Substring(0, 1);
                Derived_Data_TID = message[pos].Substring(1, 1);
                Derived_Data_COM = message[pos].Substring(2, 1);
                Derived_Data_SAB = message[pos].Substring(3, 1);
                Derived_Data_ACS = message[pos].Substring(4, 1);
                Derived_Data_BVR = message[pos].Substring(5, 1);
                Derived_Data_GVR = message[pos].Substring(6, 1);
                if (message[pos].Substring(7, 1) == "1")
                {
                    pos++;

                    Derived_Data_RAN = message[pos].Substring(0, 1);
                    Derived_Data_TAR = message[pos].Substring(1, 1);
                    Derived_Data_TAN = message[pos].Substring(2, 1);
                    Derived_Data_GSP = message[pos].Substring(3, 1);
                    Derived_Data_VUN = message[pos].Substring(4, 1);
                    Derived_Data_MET = message[pos].Substring(5, 1);
                    Derived_Data_EMC = message[pos].Substring(6, 1);
                    if (message[pos].Substring(7, 1) == "1")
                    {
                        pos++;

                        Derived_Data_POS = message[pos].Substring(0, 1);
                        Derived_Data_GAL = message[pos].Substring(1, 1);
                        Derived_Data_PUN = message[pos].Substring(2, 1);
                        Derived_Data_MB = message[pos].Substring(3, 1);
                        Derived_Data_IAR = message[pos].Substring(4, 1);
                        Derived_Data_MAC = message[pos].Substring(5, 1);
                        Derived_Data_BPS = message[pos].Substring(6, 1);
                    }
                }
            }
            pos++;

            if (Derived_Data_ADR=="1")
            {
                Derived_data_present = true;
                Derived_Data_Address = string.Concat(lib.BinarytoHexa(message[pos]), lib.BinarytoHexa(message[pos + 1]), lib.BinarytoHexa(message[pos + 2]));
                pos += 3;
            }
            if (Derived_Data_ID == "1")
            {
                Derived_data_present = true;

                StringBuilder Identification = new StringBuilder();
                string octets = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5]);
                for (int i = 0; i < 8; i++) 
                { 
                    Identification.Append(lib.Compute_Char(octets.Substring(i * 6, 6))); 
                }
                Derived_Data_Target_id = Identification.ToString().Trim();
                pos += 6;
            }

            if (Derived_Data_MHG == "1")
            {
                Derived_data_present = true;

                Derived_Data_MHG_value = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos+1]), 2) * (360 / (Math.Pow(2, 16)))) + "º";
                pos += 2;
            }

            if(Derived_Data_IAS=="1")
            {
                Derived_data_present = true;

                double val = Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15), 2);
                if(message[pos].Substring(0,1)=="0")
                {
                    Derived_Data_IAS_value = Convert.ToString(val * Math.Pow(2, -14)) + "NM/s";
                }
                else 
                {
                    Derived_Data_IAS_value = Convert.ToString(val * 0.001) + "Match";
                }
                pos += 2;
            }
            if(Derived_Data_TAS=="1")
            {
                Derived_data_present = true;

                Derived_Data_TAS_value = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2)) + " Knots";
                pos += 2;
            }
            if (Derived_Data_SAL=="1")
            {
                Derived_data_present = true;

                if (message[pos].Substring(0,1)=="0")
                {
                    Derived_Data_SAL_SAS_value = "No source information provided";
                }
                else
                {
                    Derived_Data_SAL_SAS_value = "Source Information provided";
                }
                string val = message[pos].Substring(1, 2);
                if(val=="00")
                {
                    Derived_Data_SAL_Source_value = "Unknown";
                }
                else if (val == "01")
                {
                    Derived_Data_SAL_Source_value = "Aircraft Altitude";
                }
                else if (val == "10")
                {
                    Derived_Data_SAL_Source_value = "FCU/MCP Selected Altitude";
                }
                else if (val == "11")
                {
                    Derived_Data_SAL_Source_value = "FMS Selected Altitude";
                }
                Derived_Data_SAL_Altitude_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(3, 13)) * 25) + " ft";
                pos += 2;
            }

            if (Derived_Data_FSS=="1")
            {
                Derived_data_present = true;

                if (message[pos].Substring(0,1)=="0")
                {
                    Derived_Data_FSS_MV_value = "Not active";
                }
                else 
                {
                    Derived_Data_FSS_MV_value = "Active";
                }
                if(message[pos].Substring(1,1)=="0")
                {
                    Derived_Data_FSS_AH_value = "Not Active";
                }
                else
                {
                    Derived_Data_FSS_AH_value = "Active";
                }
                if(message[pos].Substring(2,1)=="0")
                {
                    Derived_Data_FSS_AM_value = "Not active";
                }
                else
                {
                    Derived_Data_FSS_AM_value="Active";
                }
                Derived_Data_FSS_Altitude_value= Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(3, 13)) * 25) + " ft";
                pos += 2;
            }

            if(Derived_Data_TIS=="1")
            {
                Derived_data_present = true;

                if (message[pos].Substring(0,1)=="0")
                {
                    Derived_Data_TIS_NAV_value = "Trajectory Intent Data is available for this aircraft";
                }
                else
                {
                    Derived_Data_TIS_NVB_value = "Trajectory Intent Data is not available for this aircraft ";
                }
                if(message[pos].Substring(1,1)=="0")
                {
                    Derived_Data_TIS_NVB_value = "Trajectory Intent Data is valid";
                }
                else
                {
                    Derived_Data_TIS_NVB_value = "Trajectory Intent Data is not valid";
                }
                pos++;
            }
            if(Derived_Data_TID=="1")
            {
                Derived_data_present = true;

                Derived_Data_REP = Convert.ToInt32(message[pos], 2);
                Derived_Data_TCA = new string[Derived_Data_REP];
                Derived_Data_NC = new string[Derived_Data_REP];
                Derived_Data_TCP = new int[Derived_Data_REP];
                Derived_Data_Altitude = new string[Derived_Data_REP];
                Derived_Data_Latitude = new string[Derived_Data_REP];
                Derived_Data_Longitude = new string[Derived_Data_REP];
                Derived_Data_Point_Type = new string[Derived_Data_REP];
                Derived_Data_TD = new string[Derived_Data_REP];
                Derived_Data_TRA = new string[Derived_Data_REP];
                Derived_Data_TOA = new string[Derived_Data_REP];
                Derived_Data_TOV = new string[Derived_Data_REP];
                Derived_Data_TTR = new string[Derived_Data_REP];
                pos++;

                for (int i = 0; i < Derived_Data_REP; i++)
                {
                    if (message[pos].Substring(0, 1) == "0") { Derived_Data_TCA[i] = "TCP number available"; }
                    else { Derived_Data_TCA[i] = "TCP number not available"; }
                    if (message[pos].Substring(1, 1) == "0") { Derived_Data_NC[i] = "TCP compliance"; }
                    else { Derived_Data_NC[i] = "TCP non-compliance"; }
                    Derived_Data_TCP[i] = Convert.ToInt32(message[pos].Substring(2, 6));
                    pos++;
                    Derived_Data_Altitude[i] = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 10) + " ft";
                    pos += 2;
                    Derived_Data_Latitude[i] = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23)))) + " deg";
                    pos += 3;
                    Derived_Data_Longitude[i] = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23)))) + " deg";
                    pos += 3;
                    int pt = Convert.ToInt32(message[pos].Substring(0, 4), 2);
                    if (pt == 0) { Derived_Data_Point_Type[i] = "Unknown"; }
                    else if (pt == 1) { Derived_Data_Point_Type[i] = "Fly by waypoint (LT) "; }
                    else if (pt == 2) { Derived_Data_Point_Type[i] = "Fly over waypoint (LT)"; }
                    else if (pt == 3) { Derived_Data_Point_Type[i] = "Hold pattern (LT)"; }
                    else if (pt == 4) { Derived_Data_Point_Type[i] = "Procedure hold (LT)"; }
                    else if (pt == 5) { Derived_Data_Point_Type[i] = "Procedure turn (LT)"; }
                    else if (pt == 6) { Derived_Data_Point_Type[i] = "RF leg (LT)"; }
                    else if (pt == 7) { Derived_Data_Point_Type[i] = "Top of climb (VT)"; }
                    else if (pt == 8) { Derived_Data_Point_Type[i] = "Top of descent (VT)"; }
                    else if (pt == 9) { Derived_Data_Point_Type[i] = "Start of level (VT)"; }
                    else if (pt == 10) { Derived_Data_Point_Type[i] = "Cross-over altitude (VT)"; }
                    else { Derived_Data_Point_Type[i] = "Transition altitude (VT)"; }
                    string td = message[pos].Substring(4, 2);
                    if (td == "00") { Derived_Data_TD[i] = "N/A"; }
                    else if (td == "01") { Derived_Data_TD[i] = "Turn right"; }
                    else if (td == "10") { Derived_Data_TD[i] = "Turn left"; }
                    else { Derived_Data_TD[i] = "No turn"; }
                    if (message[pos].Substring(6, 1) == "0") { Derived_Data_TRA[i] = "TTR not available"; }
                    else { Derived_Data_TRA[i] = "TTR available"; }
                    if (message[pos].Substring(7, 1) == "0") { Derived_Data_TOA[i] = "TOV available"; }
                    else { Derived_Data_TOA[i] = "TOV not available"; }
                    pos++;
                    Derived_Data_TOV[i] = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2)) + " sec";
                    pos += 3;
                    Derived_Data_TTR[i] = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2) * 0.01) + " Nm";
                    pos += 2;
                }
            }

            if(Derived_Data_COM=="1")
            {
                Derived_data_present = true;

                int com = Convert.ToInt32(message[pos].Substring(0, 3), 2);
                if(com==0)
                {
                    Derived_Data_COM_Value = "No communications capability (surveillance only)";
                }
                else if(com==1)
                {
                    Derived_Data_COM_Value = "Comm. A and Comm. B capability";
                }
                else if (com == 2)
                {
                    Derived_Data_COM_Value = "Comm. A, Comm. B and Uplink ELM";
                }
                else if (com == 3)
                {
                    Derived_Data_COM_Value = "Comm. A, Comm. B, Uplink ELM and Downlink ELM";
                }
                else if (com == 4)
                {
                    Derived_Data_COM_Value = "Level 5 Transponder capability";
                }
                int stat=Convert.ToInt32(message[pos].Substring(3, 3), 2);
                if (stat == 0)
                {
                    Derived_Data_COM_STAT = "No alert, no SPI, aircraft airborne";
                }
                else if(stat==1)
                {
                    Derived_Data_COM_STAT = "No alert, no SPI, aircraft on ground";
                }
                else if (stat == 2)
                {
                    Derived_Data_COM_STAT = "Alert, no SPI, aircraft airborne";
                }
                else if (stat == 3)
                {
                    Derived_Data_COM_STAT = "Alert, no SPI, aircraft on ground ";
                }
                else if (stat == 4)
                {
                    Derived_Data_COM_STAT = "Alert, SPI, aircraft airborne or on ground ";
                }
                else if (stat == 5)
                {
                    Derived_Data_COM_STAT = "No alert, SPI, aircraft airborne or on ground";
                }
                pos++;
                if (message[pos].Substring(0,1)=="0")
                {
                    Derived_Data_COM_SSC = "No specific service capability";
                }
                else
                {
                    Derived_Data_COM_SSC = "Specific service capability";
                }
                if (message[pos].Substring(1, 1) == "0")
                {
                    Derived_Data_COM_ARC = "Altitude reporting capability resolution = 100 ft";
                }
                else
                {
                    Derived_Data_COM_ARC = "Altitude reporting capability resolution = 25 ft";
                }
                if (message[pos].Substring(2, 1) == "0")
                {
                    Derived_Data_COM_AIC = "No aircraft identification capability";
                }
                else
                {
                    Derived_Data_COM_AIC = "Aircraft identification capability";
                }

                pos++;
            }

            if(Derived_Data_SAB=="1")
            {
                Derived_data_present = true;

                string ac = message[pos].Substring(0, 2);
                if(ac=="00")
                {
                    Derived_Data_SAB_AC = "Unknown";
                }
                else if (ac == "01")
                {
                    Derived_Data_SAB_AC = "ACAS not operational";
                }
                else if (ac == "10")
                {
                    Derived_Data_SAB_AC = "ACAS operational";
                }
                else if (ac == "11")
                {
                    Derived_Data_SAB_AC = "Invalid";
                }
                string mn = message[pos].Substring(2, 2);
                if (mn== "00")
                {
                    Derived_Data_SAB_MN = "Unknown";
                }
                else if (mn == "01")
                {
                    Derived_Data_SAB_MN = "Multiple navigational aids not operating";
                }
                else if (mn == "10")
                {
                    Derived_Data_SAB_MN = "Multiple navigational aids operating";
                }
                else if (mn == "11")
                {
                    Derived_Data_SAB_MN = "Invalid";
                }
                string dc = message[pos].Substring(4, 2);
                if (dc == "00")
                {
                    Derived_Data_SAB_DC = "Unknown";
                }
                else if (dc == "01")
                {
                    Derived_Data_SAB_DC = "Differential correction";
                }
                else if (dc == "10")
                {
                    Derived_Data_SAB_DC = "No differential correction";
                }
                else if (dc == "11")
                {
                    Derived_Data_SAB_MN = "Invalid";
                }
                if(message[pos].Substring(6,1)=="0")
                {
                    Derived_Data_SAB_GBS = "Transponder Ground Bit not set or unknown";
                }
                else
                {
                    Derived_Data_SAB_GBS = "Transponder Ground Bit set";
                }
                pos++;
                int stat = Convert.ToInt32(message[pos].Substring(5,3), 2);
                if(stat==0)
                {
                    Derived_Data_SAB_STAT = "No emergency";
                }
                else if(stat==1)
                {
                    Derived_Data_SAB_STAT = "General emergency";
                }
                else if (stat == 2)
                {
                    Derived_Data_SAB_STAT = "Lifeguard / medical";
                }
                else if (stat == 3)
                {
                    Derived_Data_SAB_STAT = "Minimum fuel";
                }
                else if (stat == 4)
                {
                    Derived_Data_SAB_STAT = "No communications";
                }
                else if (stat == 5)
                {
                    Derived_Data_SAB_STAT = "Unlawful interference";
                }
                else if (stat == 6)
                {
                    Derived_Data_SAB_STAT = "“Downed” Aircraft";
                }
                else if (stat == 7)
                {
                    Derived_Data_SAB_STAT = "Unknown";
                }
                pos++;
            }

            if (Derived_Data_ACS=="1")
            {
                Derived_data_present = true;

                string messg = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5], message[pos + 6]);
                Derived_Data_ACS_TYP = messg.Substring(0, 5);
                Derived_Data_ACS_STYP = messg.Substring(5, 3);
                Derived_Data_ACS_ARA = messg.Substring(8, 14);
                Derived_Data_ACS_RAC = messg.Substring(22, 4);
                Derived_Data_ACS_RAT = messg.Substring(26, 1);
                Derived_Data_ACS_MTE = messg.Substring(27, 1);
                Derived_Data_ACS_TTI = messg.Substring(28, 2);
                Derived_Data_ACS_TID = messg.Substring(30, 26);
                pos += 7;
            }

            if(Derived_Data_BVR=="1")
            {
                Derived_data_present = true;

                Derived_Data_BVR_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 6.25) + " feet/minute";
                pos += 2;
            }

            if(Derived_Data_GVR=="1")
            {
                Derived_data_present = true;

                Derived_Data_GVR_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 6.25) + " feet/minute";
                pos += 2;
            }
            if(Derived_Data_RAN=="1")
            {
                Derived_data_present = true;

                Derived_Data_RAN_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos])) * 0.01) + "º";
                pos += 2;
            }

            if(Derived_Data_TAR=="1")
            {
                Derived_data_present = true;

                string ti = message[pos].Substring(0, 2);
                if(ti=="00")
                {
                    Derived_Data_TAR_TI = "Not available";
                }
                else if (ti=="01")
                {
                    Derived_Data_TAR_TI = "Left";
                }
                else if (ti == "10")
                {
                    Derived_Data_TAR_TI = "Right";
                }
                else if (ti == "11")
                {
                    Derived_Data_TAR_TI = "Straight";
                }
                Derived_Data_TAR_value= Convert.ToString(lib.ComputeA2Complement(message[pos].Substring(0,7)) * 0.025) + "º/s";
                pos += 2;
            }
            if(Derived_Data_TAN=="1")
            {
                Derived_data_present = true;

                Derived_Data_TAN_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos],message[pos+1])) * (360/Math.Pow(2,16))) + "º";
                pos += 2;
            }
            if(Derived_Data_GSP=="1")
            {
                Derived_data_present = true;

                Derived_Data_GSP_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * (Math.Pow(2, -14))) + "NM/s";
                pos += 2;
            }
            if(Derived_Data_VUN=="1")
            {
                Derived_data_present = true;

                Derived_Data_VUN_value = "Unknown";
                pos++;
            }
            if(Derived_Data_MET=="1")
            {
                Derived_data_present = true;

                if (message[pos].Substring(0,1)=="0")
                {
                    Derived_Data_MET_WS = "Not valid Wind Speed";
                }
                else
                {
                    Derived_Data_MET_WS = "Valid Wind Speed";
                }
                if (message[pos].Substring(1, 1) == "0")
                {
                    Derived_Data_MET_WD = "Not valid Wind Direction";
                }
                else
                {
                    Derived_Data_MET_WD = "Valid Wind Direction ";
                }
                if (message[pos].Substring(2, 1) == "0")
                {
                    Derived_Data_MET_TMP = "Not valid Temperature ";
                }
                else
                {
                    Derived_Data_MET_TMP= "Valid Temperature ";
                }
                if (message[pos].Substring(3, 1) == "0")
                {
                    Derived_Data_MET_TRB= "Not valid Turbulence ";
                }
                else
                {
                    Derived_Data_MET_TRB= "Valid Turbulence ";
                }
                pos++;
                Derived_Data_MET_WS_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]))) + "Knt";
                pos += 2;
                Derived_Data_MET_WD_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]))) + "º";
                pos += 2;
                Derived_Data_MET_TMP_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 0.25) + "ºC";
                pos += 2;
                Derived_Data_MET_TRB_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])));
                pos ++;
            }
            if(Derived_Data_EMC=="1")
            {
                Derived_data_present = true;

                int ecat = Convert.ToInt32(message[pos], 2);
                if(ecat==1)
                {
                    Derived_Data_EMC_ECAT = "light aircraft ≤ 7000 kg";
                }
                else if (ecat == 2)
                {
                    Derived_Data_EMC_ECAT = "reserved";
                }

                else if (ecat == 3)
                {
                    Derived_Data_EMC_ECAT = "7000 kg < medium aircraft < 136000 kg";
                }

                else if (ecat == 4)
                {
                    Derived_Data_EMC_ECAT = "reserved";
                }

                else if (ecat == 5)
                {
                    Derived_Data_EMC_ECAT = "136000 kg ≤ heavy aircraft";
                }

                else if (ecat == 6)
                {
                    Derived_Data_EMC_ECAT = "highly manoeuvrable (5g acceleration capability) and high speed(> 400 knots cruise) ";
                }

                else if (ecat == 7 || ecat == 8 || ecat == 9)
                {
                    Derived_Data_EMC_ECAT = "reserved";
                }

                else if (ecat == 10)
                {
                    Derived_Data_EMC_ECAT = "rotocraft";
                }

                else if (ecat == 11)
                {
                    Derived_Data_EMC_ECAT = "glider / sailplane";
                }

                else if (ecat == 12)
                {
                    Derived_Data_EMC_ECAT = "lighter-than-air";
                }
                else if (ecat == 13)
                {
                    Derived_Data_EMC_ECAT = "unmanned aerial vehicle";
                }
                else if (ecat == 14)
                {
                    Derived_Data_EMC_ECAT = "space / transatmospheric vehicle";
                }

                else if (ecat == 15)
                {
                    Derived_Data_EMC_ECAT = "ultralight / handglider / paraglider";
                }

                else if (ecat == 16)
                {
                    Derived_Data_EMC_ECAT = "parachutist / skydiver";
                }

                else if (ecat == 17|| ecat == 18|| ecat == 19)
                {
                    Derived_Data_EMC_ECAT = "reserved ";
                }

                else if (ecat == 20)
                {
                    Derived_Data_EMC_ECAT = "surface emergency vehicle";
                }

                else if (ecat == 21)
                {
                    Derived_Data_EMC_ECAT = "surface service vehicle";
                }

                else if (ecat == 22)
                {
                    Derived_Data_EMC_ECAT = "fixed ground or tethered obstruction";
                }
                else 
                {
                    Derived_Data_EMC_ECAT = "reserved ";
                }
                pos++;
            }

            if (Derived_Data_POS=="1")
            {
                Derived_data_present = true;

                double Latitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23)));
                pos += 3;
                double Longitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23)));
                int Latdegres = Convert.ToInt32(Math.Truncate(Latitude));
                int Latmin = Convert.ToInt32(Math.Truncate((Latitude - Latdegres) * 60));
                double Latsec = Math.Round(((Latitude - (Latdegres + (Convert.ToDouble(Latmin) / 60))) * 3600), 2);
                int Londegres = Convert.ToInt32(Math.Truncate(Longitude));
                int Lonmin = Convert.ToInt32(Math.Truncate((Longitude - Londegres) * 60));
                double Lonsec = Math.Round(((Longitude - (Londegres + (Convert.ToDouble(Lonmin) / 60))) * 3600), 2);
                Derived_Data_POS_Latitude = Convert.ToString(Latdegres) + "º " + Convert.ToString(Latmin) + "' " + Convert.ToString(Latsec) + "''";
                Derived_Data_POS_Longitude = Convert.ToString(Londegres) + "º" + Convert.ToString(Lonmin) + "' " + Convert.ToString(Lonsec) + "''";
                pos += 3;
            }

            if(Derived_Data_GAL=="1")
            {
                Derived_data_present = true;

                Derived_Data_GAL_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 6.25)+"ft";
                pos += 2;
            }
            if (Derived_Data_PUN=="1")
            {
                Derived_data_present = true;

                Derived_Data_PUN_value = Convert.ToString(Convert.ToInt32(message[pos].Substring(4, 4), 2));
                pos++;
            }

            if(Derived_Data_MB=="1")
            {
                Derived_data_present = true;

                int modeS_rep = Convert.ToInt32(message[pos], 2);
                if (modeS_rep < 0) { Derived_Data_MB_Data = new string[modeS_rep]; Derived_Data_MB_BDS1 = new string[modeS_rep]; Derived_Data_MB_BDS2 = new string[modeS_rep]; }
                pos++;
                for (int i = 0; i < modeS_rep; i++)
                {
                    Derived_Data_MB_Data[i] = String.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5], message[pos + 6]);
                    Derived_Data_MB_BDS1[1] = message[pos + 7].Substring(0, 4);
                    Derived_Data_MB_BDS2[1] = message[pos + 7].Substring(4, 4);
                    pos += 8;
                }
            }

            if(Derived_Data_IAR=="1")
            {
                Derived_data_present = true;

                Derived_Data_IAR_value = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2)) + " Knots";
                pos += 2;
            }

            if(Derived_Data_MAC=="1")
            {
                Derived_data_present = true;

                Derived_Data_MAC_value = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2)*0.008) + " Mach";
                pos += 2;
            }

            if(Derived_Data_BPS=="1")
            {
                Derived_data_present = true;

                Derived_Data_BPS_value = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4,12), 2) * 0.1) + " mb";
                pos += 2;
            }

            return pos;
        }


        #endregion

        #region Data Item I062/390, Flight Plan Related Data 

        public bool Flight_plan_related_present=false;

        public string Flight_Plan_Data_TAG;
        public string Flight_Plan_Data_CSN;
        public string Flight_Plan_Data_IFI;
        public string Flight_Plan_Data_FCT;
        public string Flight_Plan_Data_TAC;
        public string Flight_Plan_Data_WTC;
        public string Flight_Plan_Data_DEP;

        public string Flight_Plan_Data_DST="0";
        public string Flight_Plan_Data_RDS = "0";
        public string Flight_Plan_Data_CFL = "0";
        public string Flight_Plan_Data_CTL = "0";
        public string Flight_Plan_Data_TOD = "0";
        public string Flight_Plan_Data_AST = "0";
        public string Flight_Plan_Data_STS = "0";

        public string Flight_Plan_Data_STD = "0";
        public string Flight_Plan_Data_STA = "0";
        public string Flight_Plan_Data_PEM = "0";
        public string Flight_Plan_Data_PEC = "0";


        public string Flight_Plan_Data_TAG_SIC;
        public string Flight_Plan_Data_TAG_SAC;
        public string Flight_Plan_Data_CSN_value;

        public string Flight_Plan_Data_IFI_TYP;
        public string Flight_Plan_Data_IFI_NBR;

        public string Flight_Plan_Data_FCT_GAT;
        public string Flight_Plan_Data_FCT_FR1;
        public string Flight_Plan_Data_FCT_RVSM;
        public string Flight_Plan_Data_FCT_HPR;

        public string Flight_Plan_Data_TAC_value;
        public string Flight_Plan_Data_WTC_value;

        public string Flight_Plan_Data_DEP_value;

        public string Flight_Plan_Data_DST_value;
        public string Flight_Plan_Data_RDS_value;
        public string Flight_Plan_Data_CFL_value;
        public string Flight_Plan_Data_CTL_Centre;
        public string Flight_Plan_Data_CTL_Position;


        public int Flight_Plan_Data_TOD_REP;
        public string[] Flight_Plan_Data_TOD_TYP;
        public string[] Flight_Plan_Data_TOD_DAY;
        public string[] Flight_Plan_Data_TOD_HOR;
        public string[] Flight_Plan_Data_TOD_MIN;
        public string[] Flight_Plan_Data_TOD_AVS;
        public string[] Flight_Plan_Data_TOD_SEC;

        public string Flight_Plan_Data_AST_value;
        public string Flight_Plan_Data_STS_EMP;
        public string Flight_Plan_Data_STS_AVL;

        public string Flight_Plan_Data_STD_value;
        public string Flight_Plan_Data_STA_value;

        public string Flight_Plan_Data_PEM_validity;
        public string Flight_Plan_Data_PEM_reply;

        public string Flight_Plan_Data_PEC_value;


        private int Compute_Flight_Plan_Related_Data(string[] message, int pos)
        {
            Flight_Plan_Data_TAG = message[pos].Substring(0, 1);
            Flight_Plan_Data_CSN = message[pos].Substring(1, 1);
            Flight_Plan_Data_IFI = message[pos].Substring(2, 1);
            Flight_Plan_Data_FCT = message[pos].Substring(3, 1);
            Flight_Plan_Data_TAC = message[pos].Substring(4, 1);
            Flight_Plan_Data_WTC = message[pos].Substring(5, 1);
            Flight_Plan_Data_DEP = message[pos].Substring(6, 1);
            if (message[pos].Substring(7, 1) == "1")
            {
                pos++;
                Flight_Plan_Data_DST = message[pos].Substring(0, 1);
                Flight_Plan_Data_RDS = message[pos].Substring(1, 1);
                Flight_Plan_Data_CFL = message[pos].Substring(2, 1);
                Flight_Plan_Data_CTL = message[pos].Substring(3, 1);
                Flight_Plan_Data_TOD = message[pos].Substring(4, 1);
                Flight_Plan_Data_AST = message[pos].Substring(5, 1);
                Flight_Plan_Data_STS = message[pos].Substring(6, 1);

                if (message[pos].Substring(7, 1) == "1")
                {
                    pos++;
                    Flight_Plan_Data_STD = message[pos].Substring(0, 1);
                    Flight_Plan_Data_STA = message[pos].Substring(1, 1);
                    Flight_Plan_Data_PEM = message[pos].Substring(2, 1);
                    Flight_Plan_Data_PEC = message[pos].Substring(3, 1);
                }
            }
            pos++;
            if (Flight_Plan_Data_TAG == "1")
            {
                Flight_plan_related_present = true;
                Flight_Plan_Data_TAG_SAC = Convert.ToString(Convert.ToInt32(message[pos], 2));
                Flight_Plan_Data_TAG_SIC = Convert.ToString(Convert.ToInt32(message[pos + 1], 2));
                pos += 2;
            }
            if (Flight_Plan_Data_CSN == "1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                //string octets = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5], message[pos + 6]);
                for (int i = 0; i < 7; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos+i])); 
                }
                Flight_Plan_Data_CSN_value = Identification.ToString().Trim();
                pos += 7;
            }

            if(Flight_Plan_Data_IFI=="1")
            {
                Flight_plan_related_present = true;

                string typ = message[pos].Substring(0,2);
                if(typ=="00")
                {
                    Flight_Plan_Data_IFI_TYP = "Plan Number";
                }
                else if(typ=="01")
                {
                    Flight_Plan_Data_IFI_TYP = "Unit 1 internal flight number";
                }
                else if (typ == "10")
                {
                    Flight_Plan_Data_IFI_TYP = "Unit 2 internal flight number";
                }
                else if (typ == "11")
                {
                    Flight_Plan_Data_IFI_TYP = "Unit 3 internal flight number";
                }
                Flight_Plan_Data_IFI_NBR = Convert.ToString(Convert.ToInt32(String.Concat(message[pos],message[pos+1], message[pos + 2], message[pos + 3]).Substring(5,27), 2));
                pos += 4;
            }
            if(Flight_Plan_Data_FCT=="1")
            {
                Flight_plan_related_present = true;

                string val = message[pos].Substring(0, 2);
                if(val=="00")
                {
                    Flight_Plan_Data_FCT_GAT = "Unknown";
                }
                else if (val=="01")
                {
                    Flight_Plan_Data_FCT_GAT = "General Air Traffic";

                }
                else if (val == "10")
                {
                    Flight_Plan_Data_FCT_GAT = "Operational Air Traffic";

                }
                else if (val == "11")
                {
                    Flight_Plan_Data_FCT_GAT = "Not applicable";

                }
                val = message[pos].Substring(2, 2);
                if (val == "00")
                {
                    Flight_Plan_Data_FCT_FR1 = "Instrument Flight Rules";
                }
                else if (val == "01")
                {
                    Flight_Plan_Data_FCT_FR1 = "Visual Flight rules";

                }
                else if (val == "10")
                {
                    Flight_Plan_Data_FCT_FR1 = "Not applicable";

                }
                else if (val == "11")
                {
                    Flight_Plan_Data_FCT_FR1 = "Controlled Visual Flight Rules";

                }
                val = message[pos].Substring(4, 2);
                if (val == "00")
                {
                    Flight_Plan_Data_FCT_RVSM = "Unknown";
                }
                else if (val == "01")
                {
                    Flight_Plan_Data_FCT_RVSM = "Approved";

                }
                else if (val == "10")
                {
                    Flight_Plan_Data_FCT_RVSM = "Exempt";

                }
                else if (val == "11")
                {
                    Flight_Plan_Data_FCT_RVSM = "Not Approved";

                }
                if (message[pos].Substring(6,1)=="0")
                {
                    Flight_Plan_Data_FCT_HPR = "Normal Priority Flight";
                }
                else
                {
                    Flight_Plan_Data_FCT_HPR = "High Priority Flight";

                }
                pos++;
            }

            if(Flight_Plan_Data_TAC_value=="1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                //string octets = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5], message[pos + 6]);
                for (int i = 0; i < 4; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_TAC_value = Identification.ToString().Trim();
                pos += 4;           
            }

            if(Flight_Plan_Data_WTC=="1")
            {
                Flight_plan_related_present = true;

                Flight_Plan_Data_WTC_value = lib.OctetoBinarioASCII(message[pos]);
                pos++;
            }

            if(Flight_Plan_Data_DEP=="1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_DEP_value = Identification.ToString().Trim();
                pos += 4;
            }
            if (Flight_Plan_Data_DST == "1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_DST_value = Identification.ToString().Trim();
                pos += 4;
            }

            if(Flight_Plan_Data_RDS=="1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_RDS_value = Identification.ToString().Trim();
                pos += 3;
            }

            if(Flight_Plan_Data_CFL=="1")
            {
                Flight_plan_related_present = true;

                Flight_Plan_Data_CFL_value = Convert.ToString(Convert.ToInt32(string.Concat(message[pos],message[pos]),2))+"FL";
                pos += 2;
            }
            if(Flight_Plan_Data_CTL=="1")
            {
                Flight_plan_related_present = true;

                Flight_Plan_Data_CTL_Centre = message[pos];
                Flight_Plan_Data_CTL_Position = message[pos + 1];
                pos += 2;
            }
            if(Flight_Plan_Data_TOD=="1")
            {
                Flight_plan_related_present = true;


                Flight_Plan_Data_TOD_REP = Convert.ToInt32(message[pos], 2);
                Flight_Plan_Data_TOD_TYP = new string[Flight_Plan_Data_TOD_REP];
                Flight_Plan_Data_TOD_DAY = new string[Flight_Plan_Data_TOD_REP];
                Flight_Plan_Data_TOD_HOR = new string[Flight_Plan_Data_TOD_REP];
                Flight_Plan_Data_TOD_MIN = new string[Flight_Plan_Data_TOD_REP];
                Flight_Plan_Data_TOD_AVS = new string[Flight_Plan_Data_TOD_REP];
                Flight_Plan_Data_TOD_SEC = new string[Flight_Plan_Data_TOD_REP];

                pos++;

                for (int i = 0; i < Flight_Plan_Data_TOD_REP; i++)
                {
                    int typ = Convert.ToInt32(message[pos].Substring(0, 5));
                    if(typ==0)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Scheduled off-block time";
                    }
                    else if (typ == 1)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Estimated off-block time";
                    }
                    else if (typ == 2)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Estimated take-off time";
                    }
                    else if (typ == 3)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Actual off-block time";
                    }
                    else if (typ == 4)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Predicted time at runway hold";
                    }
                    else if (typ == 5)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Actual time at runway hold";
                    }
                    else if (typ == 6)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Actual line-up time";
                    }
                    else if (typ == 7)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Actual take-off time";
                    }
                    else if (typ == 8)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Estimated time of arrival";
                    }
                    else if (typ == 9)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Predicted landing time";
                    }
                    else if (typ == 10)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Actual landing time";
                    }
                    else if (typ == 11)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Actual time off runway";
                    }
                    else if (typ == 12)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Predicted time to gate";
                    }
                    else if (typ == 13)
                    {
                        Flight_Plan_Data_TOD_TYP[i] = "Actual on-block time";
                    }

                    int day = Convert.ToInt32(message[pos].Substring(5, 2));
                    if(day==0)
                    {
                        Flight_Plan_Data_TOD_DAY[i] = "Today";
                    }
                    else if (day == 1)
                    {
                        Flight_Plan_Data_TOD_DAY[i] = "Yesterday";
                    }
                    else if (day == 2)
                    {
                        Flight_Plan_Data_TOD_DAY[i] = "Tomorrow";
                    }
                    else if (day == 3)
                    {
                        Flight_Plan_Data_TOD_DAY[i] = "Invalid";
                    }
                    pos++;

                    Flight_Plan_Data_TOD_HOR[i] = Convert.ToString(Convert.ToInt32(message[pos].Substring(3, 5), 2));
                    pos++;
                    Flight_Plan_Data_TOD_MIN[i] = Convert.ToString(Convert.ToInt32(message[pos].Substring(2, 6), 2));
                    pos++;
                    if(message[pos].Substring(0,1)=="0")
                    {
                        Flight_Plan_Data_TOD_AVS[i] = "Seconds available";
                    }
                    else
                    {
                        Flight_Plan_Data_TOD_AVS[i] = "Seconds not available";
                    }

                    Flight_Plan_Data_TOD_SEC[i] = Convert.ToString(Convert.ToInt32(message[pos].Substring(2, 6), 2));
                    pos++;
                }
            }
        
            if(Flight_Plan_Data_AST=="1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_AST_value = Identification.ToString().PadLeft(6,' ');
                pos += 6;
            }

            if (Flight_Plan_Data_STS=="1")
            {
                Flight_plan_related_present = true;

                string emp = message[pos].Substring(0, 2);
                if(emp=="00")
                {
                    Flight_Plan_Data_STS_EMP = "Empty";
                }
                else if (emp == "01")
                {
                    Flight_Plan_Data_STS_EMP = "Occupied";
                }
                else if (emp == "10")
                {
                    Flight_Plan_Data_STS_EMP = "Unknown";
                }
                else if (emp == "11")
                {
                    Flight_Plan_Data_STS_EMP = "Invalid";
                }

                string avl = message[pos].Substring(0, 2);
                if(avl=="00")
                {
                    Flight_Plan_Data_STS_AVL = "Available";
                }
                else if (avl == "01")
                {
                    Flight_Plan_Data_STS_AVL = "Not available";
                }
                else if (avl == "02")
                {
                    Flight_Plan_Data_STS_AVL = "Unknown";
                }
                else if (avl == "03")
                {
                    Flight_Plan_Data_STS_AVL = "Invalid";
                }
                pos++;
            }

            if(Flight_Plan_Data_STD=="1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                for (int i = 0; i < 7; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_STD_value = Identification.ToString().PadLeft(7, ' ');
                pos += 7;
            }
            if (Flight_Plan_Data_STA == "1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                for (int i = 0; i < 7; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_STA_value = Identification.ToString().PadLeft(7, ' ');
                pos += 7;
            }
            if (Flight_Plan_Data_PEM=="1")
            {
                Flight_plan_related_present = true;

                if (message[pos].Substring(3,1)=="0")
                {
                    Flight_Plan_Data_PEM_validity = "No valid Mode 3/A available";
                }
                else
                {
                    Flight_Plan_Data_PEM_validity = "Valid Mode 3/A available";
                }
                Flight_Plan_Data_PEM_reply = Convert.ToString(lib.ConvertDecimalToOctal(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4, 12), 2))).PadLeft(4, '0');
                pos += 2;
            }

            if(Flight_Plan_Data_PEC=="1")
            {
                Flight_plan_related_present = true;

                StringBuilder Identification = new StringBuilder();
                for (int i = 0; i < 7; i++)
                {
                    Identification.Append(lib.OctetoBinarioASCII(message[pos + i]));
                }
                Flight_Plan_Data_PEC_value = Identification.ToString().PadLeft(7, ' ');
                pos += 7;
            }

            return pos;
        }


        #endregion


        #region Data Item I062/500, Estimated Accuracies

        public bool Estimated_accuracies_present = false;

        public string Estimated_accuracies_APC;
        public string Estimated_accuracies_COV;
        public string Estimated_accuracies_APW;
        public string Estimated_accuracies_AGA;
        public string Estimated_accuracies_ABA;
        public string Estimated_accuracies_ATV;
        public string Estimated_accuracies_AA;

        public string Estimated_accuracies_ARC="0";


        public string Estimated_accuracies_APC_X;
        public string Estimated_accuracies_APC_Y;

        public string Estimated_accuracies_COV_value;

        public string Estimated_accuracies_APW_Latitude;
        public string Estimated_accuracies_APW_Longitude;
        public string Estimated_accuracies_AGA_value;
        public string Estimated_accuracies_ABA_value;

        public string Estimated_accuracies_ATV_X;
        public string Estimated_accuracies_ATV_Y;

        public string Estimated_accuracies_AA_X;
        public string Estimated_accuracies_AA_Y;

        public string Estimated_accuracies_ARC_value;

        private int Compute_Estimated_accuracies(string[] message, int pos)
        {
            Estimated_accuracies_APC = message[pos].Substring(0, 1);
            Estimated_accuracies_COV = message[pos].Substring(1, 1);
            Estimated_accuracies_APW = message[pos].Substring(2, 1);
            Estimated_accuracies_AGA = message[pos].Substring(3, 1);
            Estimated_accuracies_ABA = message[pos].Substring(4, 1);
            Estimated_accuracies_ATV = message[pos].Substring(5, 1);
            Estimated_accuracies_AA = message[pos].Substring(6, 1);
            if (message[pos].Substring(7, 1) == "1")
            {
                pos++;
                Estimated_accuracies_ARC = message[pos].Substring(0, 1);

            }
            pos++;
            if(Estimated_accuracies_APC=="1")
            {
                Estimated_accuracies_present = true;
                Estimated_accuracies_APC_X = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos]), 2) * 0.5) + " m";
                pos += 2;
                Estimated_accuracies_APC_Y = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos]), 2) * 0.5) + " m";
                pos += 2;

            }

            if (Estimated_accuracies_COV=="1")
            {
                Estimated_accuracies_present = true;

                Estimated_accuracies_COV_value = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 0.5)+"m";
                pos += 2;
            }
            if(Estimated_accuracies_APW=="1")
            {
                Estimated_accuracies_present = true;

                Estimated_accuracies_APW_Latitude = Convert.ToString(Convert.ToDouble(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2)) * (180 / Math.Pow(2, 25)))+"º";
                pos += 2;
                Estimated_accuracies_APW_Longitude = Convert.ToString(Convert.ToDouble(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2)) * (180 / Math.Pow(2, 25)))+"º";
                pos += 2;
            }

            if(Estimated_accuracies_AGA=="1")
            {
                Estimated_accuracies_present = true;

                Estimated_accuracies_AGA_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * (6.25))+" ft";
                pos ++;
            }
            if( Estimated_accuracies_ABA=="1")
            {
                Estimated_accuracies_present = true;

                Estimated_accuracies_AGA_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * (0.25)) + " FL";
                pos++;
            }
            if(Estimated_accuracies_ATV=="1")
            {
                Estimated_accuracies_present = true;

                Estimated_accuracies_ATV_X = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * (0.25)) + " m/s";
                pos++;
                Estimated_accuracies_ATV_Y = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * (0.25)) + " m/s";
                pos++;
            }
            if (Estimated_accuracies_AA=="1")
            {
                Estimated_accuracies_present = true;

                Estimated_accuracies_AA_X = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * (0.25)) + " m/s^2";
                pos++;
                Estimated_accuracies_AA_Y = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * (0.25)) + " m/s^2";
                pos++;
            }

            if(Estimated_accuracies_ARC=="1")
            {
                Estimated_accuracies_present = true;

                Estimated_accuracies_ARC_value = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * (6.25)) + " ft/min";
                pos++;
            }

            return pos;
        }

        #endregion

        #region Data Item I062/510, Composed Track Number 

        public string system_unit_identification;
        public string system_track_number;

        public List<string> system_unit_identification_composed = new List<string>();
        public List<string> system_track_number_composed= new List<string>();

        private int Compute_Composed_Track_Number(string[] message, int pos)
        {
            system_unit_identification = message[pos];
            system_track_number=Convert.ToString(Convert.ToInt32(string.Concat(message[pos+1],message[pos+2]).Substring(0, 15)));
            pos+=3;
            while(message[pos-1].Substring(7,1)=="1")
            {
                
                system_unit_identification_composed.Add(message[pos]);
                system_track_number_composed.Add(Convert.ToString(Convert.ToInt32(string.Concat(message[pos + 1], message[pos + 2]).Substring(0, 15))));
                pos += 3;
            }
            return pos;
        }


        #endregion
    }
}
