using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PGTAWPF
{


    /// <summary>
    /// Class for decoding CAT 21 version 0.23 messages (or 0.26, which is very similar) 
    /// </summary>
    /// <param name="lib">Decoding Library is a library with useful functions for all categories</param>
    /// <param name="mensaje">String [] containing all the octets of the message to decode </param>
    /// <param name="FSPEC1">FSPEC string, removing bits indicating continuity / end of FSPEC</param>
    /// <param name="CAT">String indicating the cat</param>
    /// <param name="type">indicates if the message is version 0.23 or 0.26 (0 = 0.23, 1 = 0.26)</param> 
    /// <param name="num">Unique number for each message. They are assigned starting at 0 and growing, as messages are decoded. It will be used to identify specific messages</param>
    /// <param name="cat21v23num">Unique number for each message, the same as the parameter "num", but this is unique within this category, and not unique in all categories.</param>
    /// <param name="airportCode">Code that identifies to which airport this message belongs</param>
    /// 
    ///Within the class there are many more parameters. 
    ///The parameters that are at the beginning of a function are typical parameters of the message parameter that is being decoded. 
    ///For more information on any of those parameters, refer to the Eurocontrol documentation on Asterix.
    public class CAT21vs23
    {
        readonly LibreriaDecodificacion lib;
        readonly string FSPEC1;
        readonly public char[] FSPEC;
        readonly string[] mensaje;
        public string CAT = "21 v. 0.23";
        int type=0; //0=0.23, 1=0.26
        public int num;
        public int cat21v23num;
        public int airportCode;

        #region Constructors

        public CAT21vs23(){}

        public CAT21vs23(string[] mensajehexa, LibreriaDecodificacion lib)
        {
            try
            {
                this.lib = lib;
                this.mensaje = mensajehexa;
                FSPEC1 = lib.FSPEC(mensaje); //Get FSPEC string
                int longFSPEC = this.FSPEC1.Length / 7; //Get number of octets used by FSPEC
                int pos = 3 + longFSPEC;  //Start octet will be 3 plus number of used octets by FSPEC
                FSPEC = FSPEC1.ToCharArray(0, FSPEC1.Length);//Convert FSPEC to char array to work in a more practical way
                this.mensaje = lib.Passarmensajeenteroabinario(mensaje);//Pass the message from hexadecimal to binary to work in a more practical way

                /* From now on each function looks to see if the decoding parameter exists in the 
                message (checking if the FSPEC in its position == 1) and if it exists calls the function to decode the parameter */

                if (FSPEC[0] == '1') { pos = this.Compute_Data_Source_Identification(mensaje, pos); }
                if (FSPEC[1] == '1') { pos = this.Compute_Target_Report_Descripter(mensaje, pos); }
                if (FSPEC[2] == '1') { pos = this.Compute_Time_of_Day(mensaje, pos); }
                if (FSPEC[3] == '1') { pos = this.Compute_PositionWGS_84(mensaje, pos); }
                if (FSPEC[4] == '1') { pos = this.Compute_Target_Address(mensaje, pos); }
                if (FSPEC[5] == '1') { pos = this.Compute_Geometric_Altitude(mensaje, pos); }
                if (FSPEC[6] == '1') { pos = this.Compute_Figure_of_Merit(mensaje, pos); }
                if (FSPEC.Count() > 8)
                {
                    if (FSPEC[7] == '1') { pos = this.Compute_Link_Technology_Indicator(mensaje, pos); }
                    if (FSPEC[8] == '1') { pos = this.Compute_Roll_Angle(mensaje, pos); }
                    if (FSPEC[9] == '1') { pos = this.Compute_Flight_level(mensaje, pos); }
                    if (FSPEC[10] == '1') { pos = this.Compute_Air_Speed(mensaje, pos); }
                    if (FSPEC[11] == '1') { pos = this.Compute_True_Air_Speed(mensaje, pos); }
                    if (FSPEC[12] == '1') { pos = this.Compute_Magnetic_Heading(mensaje, pos); }
                    if (FSPEC[13] == '1') { pos = this.Compute_Barometric_Vertical_Rate(mensaje, pos); }
                }
                if (FSPEC.Count() > 16)
                {
                    if (FSPEC[14] == '1') { pos = this.Compute_Geometric_Vertical_Rate(mensaje, pos); }
                    if (FSPEC[15] == '1') { pos = this.Compute_Ground_Vector(mensaje, pos); }
                    if (FSPEC[16] == '1') { pos = this.Compute_Rate_of_turn(mensaje, pos); }
                    if (FSPEC[17] == '1') { pos = this.Compute_Target_Identification(mensaje, pos); }
                    if (FSPEC[18] == '1') { pos = this.Compute_Velocity_accuracy(mensaje, pos); }
                    if (FSPEC[19] == '1') { pos = this.Compute_Time_of_Day_Accuracy(mensaje, pos); }
                    if (FSPEC[20] == '1') { pos = this.Compute_Target_Status(mensaje, pos); }
                }
                if (FSPEC.Count() > 22)
                {
                    if (FSPEC[21] == '1') { pos = this.Compute_Emitter_Category(mensaje, pos); }
                    if (FSPEC[22] == '1') { pos = this.Compute_Met_Information(mensaje, pos); }
                    if (FSPEC[23] == '1') { pos = this.Compute_Intermediate_State_Selected_Altitude(mensaje, pos); }
                    if (FSPEC[24] == '1') { pos = this.Compute_Final_State_Selected_Altitude(mensaje, pos); }
                    if (FSPEC[25] == '1') { pos = this.Compute_Trajectory_Intent(mensaje, pos); }
                    if (FSPEC[26] == '1') { pos = this.Compute_Mode_A3(mensaje, pos); }
                    if (FSPEC[27] == '1') { pos = this.Compute_Message_Amplitude(mensaje, pos); }
                }
            }
            
            catch
            {
            }
            this.lib = null;
        }

        #endregion

        #region Compute Parameters

        #region Data Item I021/010, Data Source Identification

        /// <summary>
        /// Data Item I021/010, Data Source Identification
        /// 
        /// Definition : Identification of the ADS-B station providing information
        /// Format : Two-octet fixed length Data Item
        /// </summary>

        public string SAC;
        public string SIC;
        private int Compute_Data_Source_Identification(string[] message, int pos)
        {
            SAC = Convert.ToString(Convert.ToInt32(message[pos], 2));
            SIC = Convert.ToString(Convert.ToInt32(message[pos + 1], 2));
            this.airportCode = lib.GetAirporteCode(Convert.ToInt32(SIC));
            if (Convert.ToInt32(SIC)==104 || Convert.ToInt32(SIC)==108) //If it has SIC = 104 or 108 it is version 0.26
            {
                CAT = "21 v. 0.26"; type = 1; 
            }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/040, Target Report Descriptor
        /// <summary>
        /// Data Item I021/040, Target Report Descriptor
        /// 
        /// Definition: Type and characteristics of the data as transmitted by a system.
        /// Format: Two-Octet fixed length data item.
        /// </summary>

        public string ATP;
        public string ARC;
        public string DCR;
        public string GBS;
        public string SIM;
        public string TST;
        public string RAB;
        public string SAA;
        public string SPI;
        private int Compute_Target_Report_Descripter(string[] message, int pos)
        {
            char[] OctetoChar = message[pos].ToCharArray(0, 8);
            if (OctetoChar[0] == '0') { DCR = "No differential correction (ADS-B)"; }
            else { DCR = "Differential correction (ADS-B)"; }
            if (OctetoChar[1] == '0') { GBS = "Ground Bit not set"; }
            else { GBS = "Ground Bit set"; }
            if (OctetoChar[2] == '0') { SIM = "Actual target report"; }
            else { SIM = "Simulated target report"; }
            if (OctetoChar[3] == '0') { TST = "Default"; }
            else { TST = "Test Target"; }
            if (OctetoChar[4] == '0') { RAB = "Report from target transponder"; }
            else { TST = "Report from field monitor (fixed transponder)"; }
            if (OctetoChar[5] == '0') { SAA = "Equipment capable to provide Selected Altitude"; }
            else { SAA = "Equipment not capable to provide Selected Altitude"; }
            if (OctetoChar[6] == '0') { SPI = "Absence of SPI"; }
            else { SPI = "Special Position Identification"; }
            int atp = Convert.ToInt32(message[pos+1].Substring(0,3), 2);
            int arc = Convert.ToInt32(message[pos + 1].Substring(3, 2), 2);
            if (atp == 0) { ATP = "Non unique address"; }
            else if (atp == 1) { ATP = "24-Bit ICAO address"; }
            else if (atp == 2) { ATP = "Surface vehicle address"; }
            else if (atp == 3) { ATP = "Anonymous address"; }
            else { ATP = "Reserved for future use"; }
            if (arc == 0) { ARC = "Unknown"; }
            else if (arc == 1) { ARC = "25 ft"; }
            else if (arc == 2) { ARC = "100 ft"; }
            pos += 2;
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
        /// 
        public string LatitudeWGS_84;
        public string LongitudeWGS_84;
        public double LatitudeWGS_84_map=-200;
        public double LongitudeWGS_84_map=-200;
        private int Compute_PositionWGS_84(string[] message, int pos)
        {
            double Latitude;
            double Longitude;
            if (type == 1)  //Version 0.23 and 0.26 calculate this parameter differently. See Asterix documentation
            {
                Latitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3])) * (180 / (Math.Pow(2, 25))); pos += 4;
                Longitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3])) * (180 / (Math.Pow(2, 25))); pos += 4;
            }
            else
            {
                Latitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23))); pos += 3;
                Longitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23))); pos += 3;
            }
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
            return pos;
        }

        #endregion

        #region Data Item I021/080, Target Address
        /// <summary>
        /// Data Item I021/080, Target Address
        /// 
        /// Definition: Target address (emitter identifier) assigned uniquely to each target.
        /// Format: Three-octet fixed length Data Item.
        /// </summary>

        public string Target_address;
        private int Compute_Target_Address(string[] message, int pos) 
        {
            Target_address = string.Concat(lib.BinarytoHexa(message[pos]), lib.BinarytoHexa(message[pos + 1]), lib.BinarytoHexa(message[pos + 2]));
            pos += 3; 
            return pos;
        }

        #endregion

        #region Data Item I021/140, Geometric Altitude
        /// <summary>
        /// Data Item I021/140, Geometric Altitude
        /// 
        /// Definition : Minimum altitude from a plane tangent to the earth’s ellipsoid, defined by WGS-84, in two’s complement form.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string Geometric_Altitude;
        private int Compute_Geometric_Altitude(string[] message, int pos)
        {
            Geometric_Altitude = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 6.25) + " ft";
            pos +=  2; 
            return pos;
        }

        #endregion

        #region Data Item I021/230 Roll Angle
        /// <summary>
        /// Data Item I021/230 Roll Angle
        /// 
        /// Definition : The roll angle, in two’s complement form, of an aircraft executing a turn.
        /// Format : A two byte fixed length data item.
        /// </summary>
        public string Roll_Angle;
        private int Compute_Roll_Angle(string[] message, int pos)
        { 
            Roll_Angle = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos])) * 0.01) + "º";
            return pos++; 
        }

        #endregion

        #region Data Item I021/145, Flight Level
        /// <summary>
        /// Data Item I021/145, Flight Level
        /// 
        /// Definition : Flight Level from barometric measurements, not QNH corrected, in two’s complement form.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string Flight_Level;
        private int Compute_Flight_level(string[] message, int pos)
        { 
            Flight_Level = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * (0.25)) + " FL";
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/150, Air Speed
        /// <summary>
        /// Data Item I021/150, Air Speed
        /// 
        /// Definition : Calculated Air Speed (Element of Air Vector).
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string Air_Speed;
        private int Compute_Air_Speed(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") { Air_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15), 2) * Math.Pow(2, -14)) + " NM/s"; }
            else { Air_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15), 2) * 0.001) + " Mach"; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/151 True Airspeed
        /// <summary>
        /// Data Item I021/151 True Airspeed
        /// 
        /// Definition : True Air Speed
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string True_Air_Speed;
        private int Compute_True_Air_Speed(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") { True_Air_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15), 2)) + " Knots"; }
            else { True_Air_Speed = "Value exceeds defined rage"; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/152, Magnetic Heading
        /// <summary>
        /// Data Item I021/152, Magnetic Heading
        /// 
        /// Definition : Magnetic Heading (Element of Air Vector).
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        /// 
        public string Magnetic_Heading;
        private int Compute_Magnetic_Heading(string[] message, int pos)
        { 
            Magnetic_Heading = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos]), 2) * (360 / (Math.Pow(2, 16)))) + "º";
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/155, Barometric Vertical Rate
        /// <summary>
        /// Data Item I021/155, Barometric Vertical Rate
        /// 
        /// Definition: Barometric Vertical Rate, in two’s complement form.
        /// Format: Two-Octet fixed length data item.
        /// </summary>
        /// 
        public string Barometric_Vertical_Rate;
        private int Compute_Barometric_Vertical_Rate(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") { Barometric_Vertical_Rate = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(1, 15)) * 6.25) + " feet/minute"; }
            else { Barometric_Vertical_Rate = "Value exceeds defined rage"; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/157, Geometric Vertical Rate
        /// <summary>
        /// Data Item I021/157, Geometric Vertical Rate
        /// 
        /// Definition : Geometric Vertical Rate, in two’s complement form, with reference to WGS-84.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        /// 
        public string Geometric_Vertical_Rate;
        private int Compute_Geometric_Vertical_Rate(string[] message, int pos)
        {
            Geometric_Vertical_Rate = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(0, 16)) * 6.25) + " feet/minute"; 
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/170, Target Identification
        /// <summary>
        /// Data Item I021/170, Target Identification
        /// 
        /// Definition: Target (aircraft or vehicle) identification in 8 characters, as reported by the target.
        /// Format: Six-octet fixed length Data Item.
        /// </summary>
        /// 
        public string Target_Identification;
        private int Compute_Target_Identification(string[] message, int pos)
        {
            StringBuilder Identification = new StringBuilder();
            string octets = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5]);
            for (int i = 0; i < 8; i++) { Identification.Append(lib.Compute_Char(octets.Substring(i * 6, 6))); }
            string tar = Identification.ToString();
            if (tar.Length > 1) { Target_Identification = tar; }
            return pos + 6;
        }

        #endregion

        #region Data Item I021/200, Target Status
        /// <summary>
        /// Data Item I021/200, Target Status
        /// 
        /// Definition: Status of the target
        /// Format: One-octet fixed length Data Item
        /// </summary>

        public string ICF;
        public string LNAV;
        public string PS;
        public string SS;
        private int Compute_Target_Status(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") { ICF = "No intent change active"; }
            else { ICF = "Intent change flag raised"; }
            if (message[pos].Substring(1, 1) == "0") { LNAV = "LNAV Mode engaged"; }
            else { LNAV = "LNAV Mode not engaged"; }
            int ps = Convert.ToInt32(message[pos].Substring(3, 3), 2);
            if (ps == 0) { PS = "No emergency / not reported"; }
            else if (ps == 1) { PS = "General emergency"; }
            else if (ps == 2) { PS = "Lifeguard / medical emergency"; }
            else if (ps == 3) { PS = "Minimum fuel"; }
            else if (ps == 4) { PS = "No communications"; }
            else if (ps == 5) { PS = "Unlawful interference"; }
            else { PS = "'Downed' Aircraft "; }
            int ss = Convert.ToInt32(message[pos].Substring(6, 2), 2);
            if (ss == 0) { SS = "No condition reported"; }
            else if (ss == 1) { SS = "Permanent Alert (Emergency condition)"; }
            else if (ss == 2) { SS = "Temporary Alert (change in Mode 3/A Code other than emergency)"; }
            else { SS = "SPI set"; }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/020, Emitter Category
        /// <summary>
        /// Data Item I021/020, Emitter Category
        /// 
        /// Definition: Characteristics of the originating ADS-B unit
        /// Format: One-Octet fixed length data item.
        /// </summary>

        public string ECAT;
        private int Compute_Emitter_Category(string[] message, int pos)
        {
            int ecat = Convert.ToInt32(message[pos], 2);
            if (ecat == 0) { ECAT = "No ADS - B Emitter Category Information"; }
            if (ecat == 1) { ECAT = "Light aircraft"; }
            if (ecat == 2) { ECAT = "Reserved"; }
            if (ecat == 3) { ECAT = "Medium aircraft"; }
            if (ecat == 4) { ECAT = "Reserved"; }
            if (ecat == 5) { ECAT = "Heavy aircraft"; }
            if (ecat == 6) { ECAT = "Highly manoeuvrable(5g acceleration capability) and high speed(> 400 knots cruise)"; }
            if (ecat == 7 || ecat == 8 || ecat == 9) { ECAT = "Reserved"; }
            if (ecat == 10) { ECAT = "Rotocraft"; }
            if (ecat == 11) { ECAT = "Glider / Sailplane"; }
            if (ecat == 12) { ECAT = "Lighter than air"; }
            if (ecat == 13) { ECAT = "Unmanned Aerial Vehicle"; }
            if (ecat == 14) { ECAT = "Space / Transatmospheric Vehicle"; }
            if (ecat == 15) { ECAT = "Ultralight / Handglider / Paraglider"; }
            if (ecat == 16) { ECAT = "Parachutist / Skydiver"; }
            if (ecat == 17 || ecat == 18 || ecat == 19) { ECAT = "Reserved"; }
            if (ecat == 20) { ECAT = "Surface emergency vehicle"; }
            if (ecat == 21) { ECAT = "Surface service vehicle"; }
            if (ecat == 22) { ECAT = "Fixed ground or tethered obstruction"; }
            if (ecat == 23 || ecat == 24) { ECAT = "Reserved"; }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/220, Met Information
        /// <summary>
        /// Data Item I021/220, Met Information
        /// 
        /// Definition : Meteorological information.
        /// Format : Compound data item consisting of a one byte primary sub-field, followed by up to four fixed length data fields.
        /// </summary>

        public int MET_present = 0;
        public string Wind_Speed;
        public string Wind_Direction;
        public string Temperature;
        public string Turbulence;
        private int Compute_Met_Information(string[] message, int pos)
        {
            MET_present = 1;
            int posin = pos;
            int posfin = pos++;
            if (message[posin].Substring(0, 1) == "1") { Wind_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin], message[posfin]), 2)) + " Knots"; posfin += 2; }
            if (message[posin].Substring(1, 1) == "1") { Wind_Direction = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin], message[posfin]), 2)) + " degrees"; posfin += 2; }
            if (message[posin].Substring(2, 1) == "1") { Temperature = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin], message[posfin]), 2) * 0.25) + " ºC"; posfin +=  2; }
            if (message[posin].Substring(3, 1) == "1") { Turbulence = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin], message[posfin]), 2)) + " Turbulence"; posfin += 2; }
            return posfin;
        }

        #endregion

        #region Data Item I021/146, Intermediate State Selected Altitude
        /// <summary>
        /// Data Item I021/146, Intermediate State Selected Altitude
        /// 
        /// Definition: The short-term vertical intent as described by either the FMS selected altitude, the Altitude Control Panel Selected Altitude, or the current aircraft altitude according to the aircraft's mode of flight.
        /// Format: Two-Octet fixed length data item.
        /// </summary>

        public string SAS;
        public string Source;
        public string Sel_Altitude;
        public string Selected_Altitude;
        private int Compute_Intermediate_State_Selected_Altitude(string[] message, int pos)
        {

            string sou = message[pos].Substring(1, 2);
            if (sou == "00") { Source = "Unknown"; }
            else if (sou == "01") { Source = "Aircraft Altitude (Holding Altitude)"; }
            else if (sou == "10") { Source = "MCP/FCU Selected Altitude"; }
            else { Source = "FMS Selected Altitude"; }
            Sel_Altitude = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(3, 13)) * 25) + " ft";
            if (message[pos].Substring(0, 1) == "0") { Selected_Altitude = "No source information provided. Altitude: " + Sel_Altitude; }
            else { Selected_Altitude = "Source: " + Source + " Altitude: " + Sel_Altitude; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/148, Final State Selected Altitude
        /// <summary>
        /// Data Item I021/148, Final State Selected Altitude
        /// 
        /// Definition : The vertical intent value that corresponds with the ATC cleared altitude, as derived from the Altitude Control Panel.
        /// Format : Two-Octet fixed length data item.
        /// </summary>

        public string MV;
        public string AH;
        public string AM;
        public string Final_State_Altitude;
        private int Compute_Final_State_Selected_Altitude(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") { MV = "Not active or unknown"; }
            else { MV = "Active"; }
            if (message[pos].Substring(1, 1) == "0") { AH = "Not active or unknown"; }
            else { AH = "Active"; }
            if (message[pos].Substring(2, 1) == "0") { AM = "Not active or unknown"; }
            else { AM = "Active"; }
            Final_State_Altitude = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(3, 13)) * 25) + " ft";
            pos +=  2;
            return pos;
        }

        #endregion

        #region Data Item I021/030, Time of Day
        /// <summary>
        /// Data Item I021/030, Time of Day
        /// 
        /// Definition : Time of applicability (measurement) of the reported position, in the form of elapsed time since last midnight, expressed as UTC.
        /// Format : Three-Octet fixed length data item.
        /// </summary>
        /// <param name="Time_of_day_sec">Time in int format for the map</param>
        /// <param name="Time_of_Day">Time in string format for tables</param>

        public string Time_of_Day;
        public int Time_of_day_sec;
        private int Compute_Time_of_Day(string[] message, int pos)
        {
            int str = Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2);
            double segundos = (Convert.ToDouble(str) / 128);
            Time_of_day_sec = Convert.ToInt32(Math.Truncate(segundos));
            TimeSpan tiempo = TimeSpan.FromSeconds(segundos);
            Time_of_Day = tiempo.ToString(@"hh\:mm\:ss\:fff");
            pos += 3;
            return pos;
        }

        #endregion

        #region Data Item I021/032, Time of Day Accuracy
        /// <summary>
        /// Data Item I021/032, Time of Day Accuracy
        /// 
        /// Definition : The maximum difference between the actual time of applicability of the reported position and the time reported in the Time of Day item (I021/030).
        /// Format : One-Octet fixed length data item.
        /// </summary>

        public string Time_of_Day_Accuracy;
        private int Compute_Time_of_Day_Accuracy(string[] message, int pos)
        {
            int time = Convert.ToInt32(message[pos],2);
            Time_of_Day_Accuracy = Convert.ToString(((Convert.ToDouble(time)) * (1 / 256))) + " sec";
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/090, Figure of Merit

        /// <summary>
        /// Data Item I021/090, Figure of Merit
        /// 
        /// Definition : ADS figure of merit (FOM) provided by the aircraft avionics
        /// Format : Two-octet fixed length Data Item
        /// </summary>
        /// 
        public string AC;
        public string MN;
        public string DC;
        public string PA;
        private int Compute_Figure_of_Merit(string[] message, int pos)
        {
            string ac = message[pos].Substring(0, 2);
            string mn = message[pos].Substring(2, 2);
            string dc = message[pos].Substring(4, 2);
            if (ac == "00") { AC = "Unknown"; }
            if (ac == "01") { AC = "ACAS not operational"; }
            if (ac == "10") { AC = "ACAS operational"; }
            if (ac == "11") { AC = "Invalid"; }
            if (mn == "00") { MN = "Unknown"; }
            if (mn == "01") { MN = "Multiple navigational aids not operating"; }
            if (mn == "10") { MN = "Multiple navigational aids operating"; }
            if (mn == "11") { MN = "Invalid"; }
            if (dc == "00") { DC = "Unknown"; }
            if (dc == "01") { DC = "Differential correction"; }
            if (dc == "10") { DC = "No differential correction"; }
            if (dc == "11") { DC = "Invalid"; }
            PA = Convert.ToString(Convert.ToInt32(message[pos + 1].Substring(4, 4), 2));
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/210, Link Technology Indicator
        /// <summary>
        /// Data Item I021/210, Link Technology Indicator
        /// 
        /// Definition :Indication of which ADS link technology has been used to send the target report.
        /// Format :One-octet fixed length Data Item
        /// </summary>
        public string DTI;
        public string MDS;
        public string UAT;
        public string VDL;
        public string OTR;
        private int Compute_Link_Technology_Indicator(string[] message, int pos)
        {
            if (message[pos].Substring(3, 1) == "0") {DTI= "Unknown"; }
            else {DTI = "Aircraft equiped with CDTI"; }
            if (message[pos].Substring(4, 1) == "0") {MDS= "Not used"; }
            else { MDS= "Used"; }
            if (message[pos].Substring(5, 1) == "0") {UAT= "Not used"; }
            else { UAT= "Used"; }
            if (message[pos].Substring(6, 1) == "0") {VDL= "Not used"; }
            else { VDL= "Used"; }
            if (message[pos].Substring(7, 1) == "0") {OTR= "Not used"; }
            else { OTR= "Used"; }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/160, Ground Vector
        /// <summary>
        /// Data Item I021/160, Ground Vector
        /// 
        /// Definition : Ground Speed and Track Angle elements of Ground Vector.
        /// Format : Four-Octet fixed length data item.
        /// </summary>

        public string Ground_Vector;
        private int Compute_Ground_Vector(string[] message, int pos)
        {
            string Ground_Speed = String.Format("{0:0.00}", Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(0, 16), 2) * Math.Pow(2, -14)) + " m/s";
            string Track_Angle = String.Format("{0:0.00}", Convert.ToInt32(string.Concat(message[pos + 2], message[pos + 3]).Substring(0, 16), 2) * (360 / (Math.Pow(2, 16)))) + "º";
            Ground_Vector = "GS: " + Ground_Speed + ", T.A: " + Track_Angle;
            pos +=  4;
            return pos;
        }

        #endregion

        #region Data Item I021/165, Rate Of Turn
        /// <summary>
        /// Data Item I021/165, Rate Of Turn
        /// 
        /// Definition: Rate of Turn, in two’s complement form.
        /// Format: Variable length data item, comprising a first part of one-octet, followed by a one-octet extent as necessary.
        /// </summary>

        public string TI;
        public string RT;
        public string Rate_of_turn;
        private int Compute_Rate_of_turn(string[] message, int pos)
        {
            string ti = message[pos].Substring(0, 2);
            if (ti == "00") { TI = "Not available"; }
            if (ti == "01") { TI = "Left"; }
            if (ti == "10") { TI = "Right"; }
            if (ti == "11") { TI = "Straight"; }
            Rate_of_turn = "Turn Indicator: " + TI;
            if (message[pos].Substring(7,1)=="1")
            {
                int angle = Convert.ToInt32(message[pos + 1].Substring(0, 7), 2);
                RT = Convert.ToString(Convert.ToDouble(angle) * 0.25) + "º/s";
                Rate_of_turn = Rate_of_turn + "Rate of Turn: " + RT;
                pos++;
            }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/095, Velocity Accuracy
        /// <summary>
        /// Data Item I021/095, Velocity Accuracy
        /// 
        /// Definition: Velocity uncertainty category of the least accurate velocity component
        /// Format: One-octet fixed length Data Item
        /// </summary>
        /// 
        public string Velocity_Accuracy;
        private int Compute_Velocity_accuracy(string[] message, int pos)
        {
            Velocity_Accuracy = Convert.ToString(Convert.ToInt32(message[pos],2));
            if (Velocity_Accuracy=="") { Velocity_Accuracy = "No Data"; }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/110, Trajectory Intent
        /// <summary>
        /// Data Item I021/110, Trajectory Intent
        /// 
        /// Definition : Reports indicating the 4D intended trajectory of the aircraft
        /// Format : Compound Data Item, comprising a primary subfield of one octet, followed by the indicated subfields
        /// </summary>

        public int Trajectory_present = 0;
        public bool subfield1;
        public bool subfield2;
        public string NAV;
        public string NVB;
        public int REP;
        public string[] TCA;
        public string[] NC;
        public int[] TCP;
        public string[] Altitude;
        public string[] Latitude;
        public string[] Longitude;
        public string[] Point_Type;
        public string[] TD;
        public string[] TRA;
        public string[] TOA;
        public string[] TOV;
        public string[] TTR;
        private int Compute_Trajectory_Intent(string[] message, int pos)
        {
            Trajectory_present = 1;
            if (message[pos].Substring(0, 1) == "1") { subfield1 = true; }
            else { subfield1 = false; }
            if (message[pos].Substring(1, 1) == "1") { subfield2 = true; }
            else { subfield2 = false; }
            if (subfield1 == true)
            {
                pos++;
                if (message[pos].Substring(0, 1) == "0") { NAV = "Trajectory Intent Data is available for this aircraft"; }
                else { NAV = "Trajectory Intent Data is not available for this aircraft "; }
                if (message[pos].Substring(1, 1) == "0") { NVB = "Trajectory Intent Data is valid"; }
                else { NVB = "Trajectory Intent Data is not valid"; }
            }
            if (subfield2 == true)
            {
                pos++;
                REP = Convert.ToInt32(message[pos], 2);
                TCA = new string[REP];
                NC = new string[REP];
                TCP = new int[REP];
                Altitude = new string[REP];
                Latitude = new string[REP];
                Longitude = new string[REP];
                Point_Type = new string[REP];
                TD = new string[REP];
                TRA = new string[REP];
                TOA = new string[REP];
                TOV = new string[REP];
                TTR = new string[REP];
                pos++;

                for (int i = 0; i < REP; i++)
                {
                    if (message[pos].Substring(0, 1) == "0") { TCA[i] = "TCP number available"; }
                    else { TCA[i] = "TCP number not available"; }
                    if (message[pos].Substring(1, 1) == "0") { NC[i] = "TCP compliance"; }
                    else { NC[i] = "TCP non-compliance"; }
                    TCP[i] = Convert.ToInt32(message[pos].Substring(2, 6));
                    pos++;
                    Altitude[i] = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 10) + " ft";
                    pos += 2;
                    Latitude[i] = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * (180 / (Math.Pow(2, 23)))) + " deg";
                    pos +=  2;
                    Longitude[i] = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * (180 / (Math.Pow(2, 23)))) + " deg";
                    pos += 2;
                    int pt = Convert.ToInt32(message[pos].Substring(0, 4), 2);
                    if (pt == 0) { Point_Type[i] = "Unknown"; }
                    else if (pt == 1) { Point_Type[i] = "Fly by waypoint (LT) "; }
                    else if (pt == 2) { Point_Type[i] = "Fly over waypoint (LT)"; }
                    else if (pt == 3) { Point_Type[i] = "Hold pattern (LT)"; }
                    else if (pt == 4) { Point_Type[i] = "Procedure hold (LT)"; }
                    else if (pt == 5) { Point_Type[i] = "Procedure turn (LT)"; }
                    else if (pt == 6) { Point_Type[i] = "RF leg (LT)"; }
                    else if (pt == 7) { Point_Type[i] = "Top of climb (VT)"; }
                    else if (pt == 8) { Point_Type[i] = "Top of descent (VT)"; }
                    else if (pt == 9) { Point_Type[i] = "Start of level (VT)"; }
                    else if (pt == 10) { Point_Type[i] = "Cross-over altitude (VT)"; }
                    else { Point_Type[i] = "Transition altitude (VT)"; }
                    string td = message[pos].Substring(4, 2);
                    if (td == "00") { TD[i] = "N/A"; }
                    else if (td == "01") { TD[i] = "Turn right"; }
                    else if (td == "10") { TD[i] = "Turn left"; }
                    else { TD[i] = "No turn"; }
                    if (message[pos].Substring(6, 1) == "0") { TRA[i] = "TTR not available"; }
                    else { TRA[i] = "TTR available"; }
                    if (message[pos].Substring(7, 1) == "0") { TOA[i] = "TOV available"; }
                    else { TOA[i] = "TOV not available"; }
                    pos++;
                    TOV[i] = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2)) + " sec";
                    pos +=  3;
                    TTR[i] = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2) * 0.01) + " Nm";
                    pos += 2;
                }
            }
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
        private int Compute_Mode_A3(string[] message, int pos)
        {
            ModeA3 = Convert.ToString(lib.ConvertDecimalToOctal(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4, 12), 2))).PadLeft(4, '0');
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/131, Signal Amplitude
        /// <summary>
        /// Data Item I021/131, Signal Amplitude
        /// 
        /// Definition: Relative strength of received signal.
        /// Format: One-Octet fixed length Data Item.
        /// </summary>

        //MESSAGE AMPLITUDE
        public string Signal_Amplitude;
        private int Compute_Message_Amplitude(string[] message, int pos)
        { 
            Signal_Amplitude = Convert.ToString(lib.ComputeA2Complement(message[pos])) + " dBm"; 
            pos++;
            return pos; 
        }

        #endregion

        #endregion
    }

}
