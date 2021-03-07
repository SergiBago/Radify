using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;



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

    public class CAT21vs21
    {
        readonly LibreriaDecodificacion lib;
        readonly string FSPEC1;
        readonly string[] mensaje;
        public string CAT ="21 v. 2.1";
        public int num;
        public int cat21v21num;
        public int airportCode;

        #region Constructors

        public CAT21vs21() { }

        public CAT21vs21(string[] mensajehexa,LibreriaDecodificacion lib)
        {
            try
            {
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
                if (FSPEC[1] == '1') { pos = this.Compute_Target_Report_Descripter(mensaje, pos); }
                if (FSPEC[2] == '1') { pos = this.Compute_Track_Number(mensaje, pos); }
                if (FSPEC[3] == '1') { pos = this.Compute_Service_Identification(mensaje, pos); }
                if (FSPEC[4] == '1') { pos = this.Compute_Time_of_Aplicabillity_Position(mensaje, pos); }
                if (FSPEC[5] == '1') { pos = this.Compute_PositionWGS_84(mensaje, pos); }
                if (FSPEC[6] == '1') { pos = this.Compute_High_Resolution_PositionWGS_84(mensaje, pos); }
                if (FSPEC.Count() > 8)
                {
                    if (FSPEC[7] == '1') { pos = this.Compute_Time_of_Aplicabillity_Velocity(mensaje, pos); }
                    if (FSPEC[8] == '1') { pos = this.Compute_Air_Speed(mensaje, pos); }
                    if (FSPEC[9] == '1') { pos = this.Compute_True_Air_Speed(mensaje, pos); }
                    if (FSPEC[10] == '1') { pos = this.Compute_Target_Address(mensaje, pos); }
                    if (FSPEC[11] == '1') { pos = this.Compute_Time_of_Message_Reception_Position(mensaje, pos); }
                    if (FSPEC[12] == '1') { pos = this.Compute_Time_of_Message_Reception_Position_High_Precision(mensaje, pos); }
                    if (FSPEC[13] == '1') { pos = this.Compute_Time_of_Message_Reception_Velocity(mensaje, pos); }
                }
                if (FSPEC.Count() > 16)
                {
                    if (FSPEC[14] == '1') { pos = this.Compute_Time_of_Message_Reception_Velocity_High_Precision(mensaje, pos); }
                    if (FSPEC[15] == '1') { pos = this.Compute_Geometric_Height(mensaje, pos); }
                    if (FSPEC[16] == '1') { pos = this.Compute_Quality_Indicators(mensaje, pos); }
                    if (FSPEC[17] == '1') { pos = this.Compute_MOPS_Version(mensaje, pos); }
                    if (FSPEC[18] == '1') { pos = this.Compute_Mode_A3(mensaje, pos); }
                    if (FSPEC[19] == '1') { pos = this.Compute_Roll_Angle(mensaje, pos); }
                    if (FSPEC[20] == '1') { pos = this.Compute_Flight_level(mensaje, pos); }
                }
                if (FSPEC.Count() > 22)
                {
                    if (FSPEC[21] == '1') { pos = this.Compute_Magnetic_Heading(mensaje, pos); }
                    if (FSPEC[22] == '1') { pos = this.Compute_Target_Status(mensaje, pos); }
                    if (FSPEC[23] == '1') { pos = this.Compute_Barometric_Vertical_Rate(mensaje, pos); }
                    if (FSPEC[24] == '1') { pos = this.Compute_Geometric_Vertical_Rate(mensaje, pos); }
                    if (FSPEC[25] == '1') { pos = this.Compute_Airborne_Ground_Vector(mensaje, pos); }
                    if (FSPEC[26] == '1') { pos = this.Compute_Track_Angle_Rate(mensaje, pos); }
                    if (FSPEC[27] == '1') { pos = this.Compute_Time_of_Asterix_Report_Transmission(mensaje, pos); }
                }
                if (FSPEC.Count() > 29)
                {
                    if (FSPEC[28] == '1') { pos = this.Compute_Target_Identification(mensaje, pos); }
                    if (FSPEC[29] == '1') { pos = this.Compute_Emitter_Category(mensaje, pos); }
                    if (FSPEC[30] == '1') { pos = this.Compute_Met_Information(mensaje, pos); }
                    if (FSPEC[31] == '1') { pos = this.Compute_Selected_Altitude(mensaje, pos); }
                    if (FSPEC[32] == '1') { pos = this.Compute_Final_State_Selected_Altitude(mensaje, pos); }
                    if (FSPEC[33] == '1') { pos = this.Compute_Trajectory_Intent(mensaje, pos); }
                    if (FSPEC[34] == '1') { pos = this.Compute_Service_Managment(mensaje, pos); }
                    
                }
                if (FSPEC.Count() > 36)
                {
                    if (FSPEC[35] == '1') { pos = this.Compute_Aircraft_Operational_Status(mensaje, pos); }
                    if (FSPEC[36] == '1') { pos = this.Compute_Surface_Capabiliteies_and_Characteristics(mensaje, pos); }
                    if (FSPEC[37] == '1') { pos = this.Compute_Message_Amplitude(mensaje, pos); }
                    if (FSPEC[38] == '1') { pos = this.Compute_Mode_S_MB_DATA(mensaje, pos); }
                    if (FSPEC[39] == '1') { pos = this.Compute_ACAS_Resolution_Advisory_Report(mensaje, pos); }
                    if (FSPEC[40] == '1') { pos = this.Compute_Receiver_ID(mensaje, pos); }
                    if (FSPEC[41] == '1') { pos = this.Compute_Data_Age(mensaje, pos); }
                }
            }
            catch
            {
                mensaje = mensajehexa;
            }
            this.lib = null;
        }

        #endregion


        #region COMPUTE PARAMETERS

        #region Data Item I021/008, Aircraft Operational Status
        /// <summary>
        /// Data Item I021/008, Aircraft Operational Status
        /// 
        /// Definition: Identification of the operational services available in the aircraft while airborne.
        /// Format: One-octet fixed length Data Item.
        /// </summary>

        public string RA;
        public string TC;
        public string TS;
        public string ARV;
        public string CDTIA;
        public string Not_TCAS;
        public string SA;
        private int Compute_Aircraft_Operational_Status(string[] message, int pos)
        {
            char[] OctetoChar = message[pos].ToCharArray(0, 8);
            if (OctetoChar[0] == '1') { RA = "TCAS RA active"; }
            else { RA = "TCAS II or ACAS RA not active"; }
            if (Convert.ToInt32(string.Concat(OctetoChar[1], OctetoChar[2]), 2) == 1) { TC = "No capability for trajectory Change Reports"; }
            else if (Convert.ToInt32(string.Concat(OctetoChar[1], OctetoChar[2]), 2) == 2) { TC = "Support fot TC+0 reports only"; }
            else if (Convert.ToInt32(string.Concat(OctetoChar[1], OctetoChar[2]), 2) == 3) { TC = "Support for multiple TC reports"; }
            else { TC = "Reserved"; }
            if (OctetoChar[3] == '0') { TS = "No capability to support Target State Reports"; }
            else { TS = "Capable of supporting target State Reports"; }
            if (OctetoChar[4] == '0') { ARV = "No capability to generate ARV-Reports"; }
            else { ARV = "Capable of generate ARV-Reports"; };
            if (OctetoChar[5] == '0') { CDTIA = "CDTI not operational"; }
            else { CDTIA = "CDTI operational"; }
            if (OctetoChar[6] == '0') { Not_TCAS = "TCAS operational"; }
            else { Not_TCAS = "TCAS not operational"; }
            if (OctetoChar[7] == '0') { SA = "Antenna Diversity"; }
            else { SA = "Single Antenna only"; }
            pos++;
            return pos;
        }

        #endregion

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
            SAC = Convert.ToString(Convert.ToInt32(message[pos],2));
            SIC = Convert.ToString(Convert.ToInt32(message[pos+1],2));
            this.airportCode = lib.GetAirporteCode(Convert.ToInt32(SIC)); //Computes airport code from SIC 
            pos += 2;
            return pos;
        }

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
            Service_Identification = Convert.ToString(Convert.ToInt32(message[pos],2));
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/016, Service Management
        /// <summary>
        /// Data Item I021/016, Service Management
        /// 
        /// Definition: Identification of services offered by a ground station (identified by a SIC code).
        /// Format: One-octet fixed length Data Item.
        /// </summary>
        public string RP;
        private int Compute_Service_Managment(string[] message, int pos) 
        {
            RP = Convert.ToString(Convert.ToDouble(Convert.ToInt32(message[pos], 2)) * 0.5) + " sec";
            pos++; 
            return pos; 
        }

        #endregion

        #region Data Item I021/020, Emitter Category
        /// <summary>
        /// Data Item I021/020, Emitter Category
        /// 
        /// Definition : Characteristics of the originating ADS-B unit.
        /// Format : One-Octet fixed length data item.
        /// </summary>
        //EMITTER CATEGORY
        public string ECAT;
        private int Compute_Emitter_Category(string[] message, int pos)
        {
            int ecat = Convert.ToInt32(message[pos], 2);
            if (Target_Identification == "7777XBEG") { ECAT = "No ADS-B Emitter Category Information"; }
            else
            {
                if (ecat == 0) { ECAT = "No ADS-B Emitter Category Information"; }
                if (ecat == 1) { ECAT = "Light aircraft"; }
                if (ecat == 2) { ECAT = "Small aircraft"; }
                if (ecat == 3) { ECAT = "Medium aircraft"; }
                if (ecat == 4) { ECAT = "High Vortex Large"; }
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
                if (ecat == 23) { ECAT = "Cluster obstacle"; }
                if (ecat == 24) { ECAT = "Line obstacle"; }
            }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/040, Target Report Descriptor
        /// <summary>
        /// Data Item I021/040, Target Report Descriptor
        /// 
        /// Definition: Type and characteristics of the data as transmitted by a system.
        /// Format: Variable Length Data Item, comprising a primary subfield of one octet, followed by one-octet extensions as necessary.
        /// </summary>

        public string ATP;
        public string ARC;
        public string RC;
        public string RAB;
        public string DCR;
        public string GBS;
        public string SIM;
        public string TST;
        public string SAA;
        public string CL;
        public string IPC;
        public string NOGO;
        public string CPR;
        public string LDPJ;
        public string RCF;
        public string FX;
        private int Compute_Target_Report_Descripter(string[] message, int pos)
        {
            char[] OctetoChar = message[pos].ToCharArray(0, 8);
            int atp = Convert.ToInt32(string.Concat(OctetoChar[0], OctetoChar[1], OctetoChar[2]), 2);
            int arc = Convert.ToInt32(string.Concat(OctetoChar[3], OctetoChar[4]), 2);
            if (atp == 0) { ATP = "24-Bit ICAO address"; }
            else if (atp == 1) { ATP = "Duplicate address"; }
            else if (atp == 2) { ATP = "Surface vehicle address"; }
            else if (atp == 3) { ATP = "Anonymous address"; }
            else { ATP = "Reserved for future use"; }
            if (arc == 0) { ARC = "25 ft "; }
            else if (arc == 1) { ARC = "100 ft"; }
            else if (arc == 2) { ARC = "Unknown"; }
            else { ARC = "Invalid"; }
            if (OctetoChar[5] == '0') { RC = "Default"; }
            else { RC = "Range Check passed, CPR Validation pending"; }
            if (OctetoChar[6] == '0') { RAB = "Report from target transponder"; }
            else { RAB = "Report from field monitor (fixed transponder)"; }
            pos ++;
            if (OctetoChar[7] == '1')
            {
                OctetoChar = message[pos].ToCharArray(0, 8);
                if (OctetoChar[0] == '0') { DCR = "No differential correction (ADS-B)"; }
                else { DCR = "Differential correction (ADS-B)"; }
                if (OctetoChar[1] == '0') { GBS = "Ground Bit not set"; }
                else { GBS = "Ground Bit set"; }
                if (OctetoChar[2] == '0') { SIM = "Actual target report"; }
                else { SIM = "Simulated target report"; }
                if (OctetoChar[3] == '0') { TST = "Default"; }
                else { TST = "Test Target"; }
                if (OctetoChar[4] == '0') { SAA = "Equipment capable to provide Selected Altitude"; }
                else { SAA = "Equipment not capable to provide Selected Altitude"; }
                int cl = Convert.ToInt32(string.Concat(OctetoChar[5], OctetoChar[6]), 2);
                if (cl == 0) { CL = "Report valid"; }
                else if (cl == 1) { CL = "Report suspect"; }
                else if (cl == 2) { CL = "No information"; }
                else { CL = "Reserved for future use"; }
                pos ++;
                if (OctetoChar[7] == '1')
                {
                    OctetoChar = message[pos].ToCharArray(0, 8);
                    if (OctetoChar[2] == '0') { IPC = "Default"; }
                    else { IPC = "Independent Position Check failed"; }
                    if (OctetoChar[3] == '0') { NOGO = "NOGO-bit not set"; }
                    else { NOGO = "NOGO-bit set"; }
                    if (OctetoChar[4] == '0') { CPR = "CPR Validation correct "; }
                    else { CPR = "CPR Validation failed"; }
                    if (OctetoChar[5] == '0') { LDPJ = "LDPJ not detected"; }
                    else { LDPJ = "LDPJ detected"; }
                    if (OctetoChar[6] == '0') { RCF = "Default"; }
                    else { RCF = "Range Check failed "; }
                    pos ++;
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
            ModeA3 = Convert.ToString(lib.ConvertDecimalToOctal(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(4,12),2))).PadLeft(4,'0');
            pos += 2;
            return pos; 
        }

        #endregion

        #region Data Item I021/071, Time of Applicability for Position
        /// <summary>
        /// Data Item I021/071, Time of Applicability for Position
        /// 
        /// Definition: Time of applicability of the reported position, in the form of elapsed time since last midnight, expressed as UTC.
        /// Format: Three-Octet fixed length data item.
        /// </summary>
        public string Time_of_Applicability_Position;
        private int Compute_Time_of_Aplicabillity_Position(string[] message, int pos)
        {
           // MessageBox.Show("Entered");
            int str = Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2);
            double segundos = (Convert.ToDouble(str) / 128);
           // Time_of_day_sec = Convert.ToInt32(Math.Truncate(segundos));
            TimeSpan tiempo = TimeSpan.FromSeconds(segundos);
            Time_of_Applicability_Position = tiempo.ToString(@"hh\:mm\:ss\:fff");
            pos += 3;
            return pos;
        }

        #endregion

        #region Data Item I021/072, Time of Applicability for Velocity
        /// <summary>
        /// Data Item I021/072, Time of Applicability for Velocity
        /// 
        /// Definition: Time of applicability(measurement) of the reported velocity, in the form of elapsed time since last midnight, expressed as UTC.
        /// Format: Three-Octet fixed length data item.
        /// </summary>
        public string Time_of_Applicability_Velocity;
        private int Compute_Time_of_Aplicabillity_Velocity(string[] message, int pos)
        {
            int str = Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2);
            double segundos = (Convert.ToDouble(str) / 128);
            TimeSpan tiempo = TimeSpan.FromSeconds(segundos);
            Time_of_Applicability_Velocity = tiempo.ToString(@"hh\:mm\:ss\:fff");
            pos += 3;
            return pos;
        }

        #endregion

        #region Data Item I021/073, Time of Message Reception for Position

        /// <summary>
        /// Data Item I021/073, Time of Message Reception for Position
        /// 
        /// Definition : Time of reception of the latest position squitter in the Ground
        /// Station, in the form of elapsed time since last midnight, expressed as UTC.
        /// Format : Three-Octet fixed length data item.
        /// </summary>
        public string Time_of_Message_Reception_Position;
        private int Compute_Time_of_Message_Reception_Position(string[] message, int pos)
        {
            int str = Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2);
            double segundos = (Convert.ToDouble(str) / 128);
            TimeSpan tiempo = TimeSpan.FromSeconds(segundos);
            Time_of_Message_Reception_Position = tiempo.ToString(@"hh\:mm\:ss\:fff");
            pos +=3;
            return pos;
        }

        #endregion

        #region Data Item I021/074, Time of Message Reception of Position–High Precision

        /// <summary>
        /// Data Item I021/074, Time of Message Reception of Position–High Precision
        /// 
        /// Definition : Time at which the latest ADS-B position information was received
        /// by the ground station, expressed as fraction of the second of the UTC Time.
        /// Format : Four-Octet fixed length data item.
        /// </summary>
        public string Time_of_Message_Reception_Position_High_Precision;
        private int Compute_Time_of_Message_Reception_Position_High_Precision(string[] message, int pos)
        {
            string octet = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3]);
            string FSI = octet.Substring(0, 2);
            string time = octet.Substring(2, 30);
            int str = Convert.ToInt32(time, 2);
            double sec = (Convert.ToDouble(str)) * Math.Pow(2, -30);
            if (FSI == "10") { sec--; }
            if (FSI == "01") { sec++; }
            Time_of_Message_Reception_Position_High_Precision = Convert.ToString(sec) + " sec";
            pos += 4;
            return pos;
        }

        #endregion

        #region Data Item I021/075, Time of Message Reception for Velocity

        /// <summary>
        /// Data Item I021/075, Time of Message Reception for Velocity
        /// 
        /// Definition : Time of reception of the latest velocity squitter in the Ground
        /// Station, in the form of elapsed time since last midnight, expressed as UTC.
        /// Format: Three-Octet fixed length data item.
        /// </summary>
        public string Time_of_Message_Reception_Velocity;
        private int Compute_Time_of_Message_Reception_Velocity(string[] message, int pos)
        {
            int str = Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2);
            double segundos = (Convert.ToDouble(str) / 128);
            TimeSpan tiempo = TimeSpan.FromSeconds(segundos);
            Time_of_Message_Reception_Velocity = tiempo.ToString(@"hh\:mm\:ss\:fff");
            pos +=  3;
            return pos;
        }

        #endregion

        #region Data Item I021/076, Time of Message Reception of Velocity–High Precision

        /// <summary>
        /// Data Item I021/076, Time of Message Reception of Velocity–High Precision
        /// 
        /// Definition : Time at which the latest ADS-B velocity information was received
        /// by the ground station, expressed as fraction of the second of the UTC Time.
        /// Format: Four-Octet fixed length data item.
        /// </summary>
        public string Time_of_Message_Reception_Velocity_High_Precision;
        private int Compute_Time_of_Message_Reception_Velocity_High_Precision(string[] message, int pos)
        {
            string octet = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3]);
            string FSI = octet.Substring(0, 2);
            string time = octet.Substring(2, 30);
            int str = Convert.ToInt32(time, 2);
            double sec = (Convert.ToDouble(str)) * Math.Pow(2, -30);
            if (FSI == "10") { sec--; }
            if (FSI == "01") { sec++; }
            Time_of_Message_Reception_Velocity_High_Precision = Convert.ToString(sec) + " sec";
            pos +=4;
            return pos;
        }

        #endregion

        #region Time of ASTERIX Report Transmission

        /// <summary>
        /// Time of ASTERIX Report Transmission
        /// 
        /// Definition : Time of the transmission of the ASTERIX category 021 report in
        /// the form of elapsed time since last midnight, expressed as UTC.
        /// Format: Three-Octet fixed length data item.
        /// </summary>
        /// <param name="Time_of_day_sec">Time in int format for the map</param>
        /// <param name="Time_of_Asterix_Report_Transmission">Time in string format for tables</param>
        public string Time_of_Asterix_Report_Transmission;
        public int Time_of_day_sec;
        private int Compute_Time_of_Asterix_Report_Transmission(string[] message, int pos)
        {
            
            int str = Convert.ToInt32(string.Concat(message[pos], message[pos + 1], message[pos + 2]), 2);
            double segundos = (Convert.ToDouble(str) / 128);
            Time_of_day_sec = Convert.ToInt32(Math.Truncate(segundos));
            TimeSpan tiempo = TimeSpan.FromSeconds(segundos);
            Time_of_Asterix_Report_Transmission = tiempo.ToString(@"hh\:mm\:ss\:fff");
            pos += 3;
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
        //TARGET ADDRESS
        public string Target_address;
        private int Compute_Target_Address(string[] message, int pos) 
        {
            Target_address = string.Concat(lib.BinarytoHexa(message[pos]), lib.BinarytoHexa(message[pos + 1]), lib.BinarytoHexa(message[pos + 2]));
            pos += 3; 
            return pos; 
        }

        #endregion

        #region Data Item I021/090, Quality Indicators
        /// <summary>
        /// Data Item I021/090, Quality Indicators
        /// 
        /// Definition: ADS-B quality indicators transmitted by a/c according to MOPS version.
        /// Format: Variable Length Data Item, comprising a primary subfield of oneoctet, followed by one-octet extensions as necessary.
        /// NOTE: Apart from the “PIC” item, all items are defined as per the respective link technology protocol version(“MOPS version”, see I021/210).
        /// </summary>

        public string Quality_Indicators;
        public string NUCr_NACv;
        public string NUCp_NIC;
        public string NICbaro;
        public string SIL;
        public string NACp;
        public string SILS;
        public string SDA;
        public string GVA;
        public int PIC;
        public string ICB;
        public string NUCp;
        public string NIC;

        private int Compute_Quality_Indicators(string[] message, int pos)
        {
            NUCr_NACv = Convert.ToString(Convert.ToInt32(message[pos].Substring(0, 3), 2));
            NUCp_NIC = Convert.ToString(Convert.ToInt32(message[pos].Substring(3, 4), 2));
            pos++;
            if (message[pos-1].Substring(7, 1) == "1")
            {
                
                NICbaro = Convert.ToString(Convert.ToInt32(message[pos].Substring(0, 1), 2));
                SIL = Convert.ToString(Convert.ToInt32(message[pos].Substring(1, 2), 2));
                NACp = Convert.ToString(Convert.ToInt32(message[pos].Substring(3, 4), 2));
                pos++;
                if (message[pos-1].Substring(7, 1) == "1")
                {
                    
                    if (message[pos].Substring(2, 1) == "0") { SILS = "Measured per flight-Hour"; }
                    else { SILS = "Measured per sample"; }
                    SDA = Convert.ToString(Convert.ToInt32(message[pos].Substring(3, 2), 2));
                    GVA = Convert.ToString(Convert.ToInt32(message[pos].Substring(5, 2), 2));
                    pos++;
                    if (message[pos-1].Substring(7, 1) == "1")
                    {
                        
                        PIC = Convert.ToInt32(message[pos].Substring(0, 4), 2);
                        if (PIC == 0) { ICB = "No integrity(or > 20.0 NM)"; NUCp = "0"; NIC = "0"; }
                        if (PIC == 1) { ICB = "< 20.0 NM"; NUCp = "1"; NIC = "1"; }
                        if (PIC == 2) { ICB = "< 10.0 NM"; NUCp = "2"; NIC = "-"; }
                        if (PIC == 3) { ICB = "< 8.0 NM"; NUCp = "-"; NIC = "2"; }
                        if (PIC == 4) { ICB = "< 4.0 NM"; NUCp = "-"; NIC = "3"; }
                        if (PIC == 5) { ICB = "< 2.0 NM"; NUCp = "3"; NIC = "4"; }
                        if (PIC == 6) { ICB = "< 1.0 NM"; NUCp = "4"; NIC = "5"; }
                        if (PIC == 7) { ICB = "< 0.6 NM"; NUCp = "-"; NIC = "6 (+ 1/1)"; }
                        if (PIC == 8) { ICB = "< 0.5 NM"; NUCp = "5"; NIC = "6 (+ 0/0)"; }
                        if (PIC == 9) { ICB = "< 0.3 NM"; NUCp = "-"; NIC = "6 (+ 0/1)"; }
                        if (PIC == 10) { ICB = "< 0.2 NM"; NUCp = "6"; NIC = "7"; }
                        if (PIC == 11) { ICB = "< 0.1 NM"; NUCp = "7"; NIC = "8"; }
                        if (PIC == 12) { ICB = "< 0.04 NM"; NUCp = ""; NIC = "9"; }
                        if (PIC == 13) { ICB = "< 0.013 NM"; NUCp = "8"; NIC = "10"; }
                        if (PIC == 14) { ICB = "< 0.004 NM"; NUCp = "9"; NIC = "11"; }
                        pos++;
                    }
                }
            }
            return pos;
        }

        #endregion

        #region Data Item I021/110, Trajectory Intent
        /// <summary>
        /// Data Item I021/110, Trajectory Intent
        /// 
        /// Definition : Reports indicating the 4D intended trajectory of the aircraft.
        /// Format : Compound Data Item, comprising a primary subfield of one octet, followed by the indicated subfields.
        /// </summary>

        //TRAJECTORY INTENT
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
                    pos +=2;
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
                    pos += 3;
                    TTR[i] = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]), 2) * 0.01) + " Nm";
                    pos += 2;
                }
            }
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
        public double LatitudeWGS_84_map=-200;
        public double LongitudeWGS_84_map=-200;
        private int Compute_PositionWGS_84(string[] message, int pos)
        {
            
            double Latitude  =lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2]))*(180/(Math.Pow(2,23)));
            pos += 3;
            double Longitude = lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2])) * (180 / (Math.Pow(2, 23)));
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
            pos += 3;
            return pos;
        }

        #endregion

        #region Data Item I021/131, High-Resolution Position in WGS-84 Co-ordinates

        /// <summary>
        /// Data Item I021/131, High-Resolution Position in WGS-84 Co-ordinates
        /// 
        /// Definition: Position in WGS-84 Co-ordinates in high resolution.
        /// Format: Eight-octet fixed length Data Item.
        /// </summary>
        public string High_Resolution_LatitudeWGS_84;
        public string High_Resolution_LongitudeWGS_84;
        private int Compute_High_Resolution_PositionWGS_84(string[] message, int pos)
        {
            
            double Latitude= lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3])) * (180 / (Math.Pow(2, 30))); pos +=  4;
            double Longitude= lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3])) * (180 / (Math.Pow(2, 30))); pos += 4;
            LatitudeWGS_84_map = Convert.ToDouble(Latitude);
            LongitudeWGS_84_map = Convert.ToDouble(Longitude);
            int Latdegres = Convert.ToInt32(Math.Truncate(Latitude));
            int Latmin = Convert.ToInt32(Math.Truncate((Latitude - Latdegres) * 60));
            double Latsec = Math.Round(((Latitude - (Latdegres + (Convert.ToDouble(Latmin) / 60))) * 3600), 5);
            int Londegres = Convert.ToInt32(Math.Truncate(Longitude));
            int Lonmin = Convert.ToInt32(Math.Truncate((Longitude - Londegres) * 60));
            double Lonsec = Math.Round(((Longitude - (Londegres + (Convert.ToDouble(Lonmin) / 60))) * 3600), 5);
            High_Resolution_LatitudeWGS_84 = Convert.ToString(Latdegres) + "º " + Convert.ToString(Latmin) + "' " + Convert.ToString(Latsec) + "''";
            High_Resolution_LongitudeWGS_84 = Convert.ToString(Londegres) + "º" + Convert.ToString(Lonmin) + "' " + Convert.ToString(Lonsec) + "''";
            return pos;
        }

        #endregion

        #region Data Item I021/132, Message Amplitude
        /// <summary>
        /// Data Item I021/132, Message Amplitude
        /// 
        /// Definition : Amplitude, in dBm, of ADS-B messages received by the ground station, coded in two’s complement.
        /// Format : One-Octet fixed length data item.
        /// </summary>
        public string Message_Amplitude;
        private int Compute_Message_Amplitude(string[] message, int pos) 
        { 
            Message_Amplitude = Convert.ToString(lib.ComputeA2Complement(message[pos])) + " dBm"; 
            pos++;
            return pos; 
        }

        #endregion

        #region Data Item I021/140, Geometric Height

        /// <summary>
        /// Data Item I021/140, Geometric Height
        /// 
        /// Definition : Minimum height from a plane tangent to the earth’s ellipsoid,
        /// defined by WGS-84, in two’s complement form.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string Geometric_Height;
        private int Compute_Geometric_Height(string[] message, int pos)
        {
            Geometric_Height = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * 6.25) + " ft";pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/145, Flight Level

        /// <summary>
        /// Data Item I021/145, Flight Level
        /// 
        /// Definition: Flight Level from barometric measurements, not QNH corrected, in two’s complement form.
        /// Format: Two-Octet fixed length data item.
        /// </summary>
        public string Flight_Level;
        private int Compute_Flight_level(string[] message, int pos)
        {
            Flight_Level = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1])) * (0.25)) + " FL";
            pos += 2; 
            return pos; 
        }

        #endregion

        #region Data Item I021/146, Selected Altitude

        /// <summary>
        /// Data Item I021/146, Selected Altitude
        /// 
        /// Definition : The Selected Altitude as provided by the avionics and
        /// corresponding either to the MCP/FCU Selected Altitude(the ATC
        /// cleared altitude entered by the flight crew into the avionics) or to
        /// the FMS Selected Altitude.
        /// Format: Two-Octet fixed length data item.
        /// </summary>

        public string SAS;
        public string Source;
        public string Sel_Altitude;
        public string Selected_Altitude;
        private int Compute_Selected_Altitude(string[] message, int pos)
        {          
            string sou = message[pos].Substring(1, 2);
            if (sou == "00") { Source = "Unknown"; }
            else if (sou == "01") { Source = "Aircraft Altitude (Holding Altitude)"; }
            else if (sou == "10") { Source = "MCP/FCU Selected Altitude"; }
            else { Source = "FMS Selected Altitude"; }
            Sel_Altitude = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos+1]).Substring(3, 13)) * 25) + " ft";
            Selected_Altitude= "SA: "+ Convert.ToString(Sel_Altitude);
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/148, Final State Selected Altitude
        /// <summary>
        /// Data Item I021/148, Final State Selected Altitude
        /// 
        /// Definition : The vertical intent value that corresponds with the ATC cleared
        /// altitude, as derived from the Altitude Control Panel(MCP/FCU).
        /// Format: Two-Octet fixed length data item.
        /// </summary>
        /// 
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
            Final_State_Altitude = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos+1]).Substring(3, 13)) * 25) + " ft";
            pos += 2;
            return pos ;
        }

        #endregion

        #region Data Item I021/150, Air Speed
        /// <summary>
        /// Data Item I021/150, Air Speed
        /// 
        /// Definition: Calculated Air Speed (Element of Air Vector).
        /// Format: Two-Octet fixed length data item.
        /// </summary>
        public string Air_Speed;
        private int Compute_Air_Speed(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") { Air_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15),2) * Math.Pow(2, -14)) + " NM/s"; }
            else { Air_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15),2) * 0.001) + " Mach"; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/151 True Airspeed

        /// <summary>
        /// Data Item I021/151 True Airspeed
        /// 
        /// Definition : True Air Speed.
        /// Format : Two-Octet fixed length data item.
        /// </summary>
        public string True_Air_Speed;
        private int Compute_True_Air_Speed(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0")
            { 
                True_Air_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15),2)) + " Knots";
            }
            else { True_Air_Speed = "Value exceeds defined rage"; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/152, Magnetic Heading
        /// <summary>
        /// Data Item I021/152, Magnetic Heading
        /// 
        /// Definition: Magnetic Heading (Element of Air Vector).
        /// Format: Two-Octet fixed length data item.
        /// </summary>

        //MAGNETIC HEADING
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
        public string Barometric_Vertical_Rate;
        private int Compute_Barometric_Vertical_Rate(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") {
                Barometric_Vertical_Rate = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(1, 15)) * 6.25) + " feet/minute"; }
            else { Barometric_Vertical_Rate = "Value exceeds defined rage"; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/157, Geometric Vertical Rate

        /// <summary>
        /// Data Item I021/157, Geometric Vertical Rate
        /// 
        /// Definition: Geometric Vertical Rate, in two’s complement form, with reference to WGS-84.
        /// Format: Two-Octet fixed length data item.
        /// </summary>
        public string Geometric_Vertical_Rate;
        private int Compute_Geometric_Vertical_Rate(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0")
            {
                Geometric_Vertical_Rate = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos], message[pos + 1]).Substring(1, 15)) * 6.25) + " feet/minute"; 
            }
            else { Geometric_Vertical_Rate = "Value exceeds defined rage"; }
            pos += 2;
            return pos;
        }

        #endregion

        #region Data Item I021/160, Airborne Ground Vector

        /// <summary>
        /// Data Item I021/160, Airborne Ground Vector
        /// 
        /// Definition : Ground Speed and Track Angle elements of Airborne Ground Vector.
        /// Format : Four-Octet fixed length data item.
        /// </summary>
        public string Ground_Speed;
        public string Track_Angle;
        public string Ground_vector;
        private int Compute_Airborne_Ground_Vector(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0")
            {
                Ground_Speed = String.Format("{0:0.00}", (Convert.ToInt32(string.Concat(message[pos], message[pos + 1]).Substring(1, 15),2) * Math.Pow(2, -14)*3600)) +  "Knts";
               // double meters = 
                Track_Angle = String.Format("{0:0.00}", Convert.ToInt32(string.Concat(message[pos + 2], message[pos + 3]).Substring(0, 16),2) * (360 / (Math.Pow(2, 16))));
                Ground_vector = "GS: " + Ground_Speed + ", T.A: "+ String.Format("{0:0.00}",Track_Angle)+"º";
            }
            else { Ground_vector= "Value exceeds defined rage"; }
            pos +=  4;
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
            Track_Number=Convert.ToString(Convert.ToInt32(string.Concat(message[pos],message[pos+1]).Substring(4,12),2)); 
            pos += 2;
            return pos;  
        }

        #endregion

        #region Data Item I021/165, Track Angle Rate

        /// <summary>
        /// Data Item I021/165, Track Angle Rate
        /// 
        /// Definition : Rate of Turn, in two’s complement form.
        /// Format : 2-Byte Fixed length data item.
        /// </summary>
        public string Track_Angle_Rate;
        private int Compute_Track_Angle_Rate(string[] message, int pos) 
        {
            Track_Angle_Rate = Convert.ToString(Convert.ToInt32(string.Concat(message[pos], message[pos]).Substring(6, 10), 2)*(1/32))+" º/s";
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
        private int Compute_Target_Identification (string[] message, int pos)
        {
            StringBuilder Identification= new StringBuilder();
            string octets = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5]);
            for (int i=0; i<8;i++) {Identification.Append(lib.Compute_Char(octets.Substring(i*6,6)));}
            string tar = Identification.ToString();
            if (tar.Length > 1) { Target_Identification = tar; }
            return pos + 6;
        }

        #endregion

        #region Data Item I021/200, Target Status
        /// <summary>
        /// Data Item I021/200, Target Status
        /// 
        /// Definition : Status of the target
        /// Format : One-octet fixed length Data Item
        /// </summary>
        public string ICF;
        public string LNAV;
        public string PS;
        public string SS;
        private int Compute_Target_Status(string[] message, int pos)
        {
            if (message[pos].Substring(0, 1) == "0") { ICF = "No intent change active"; }
            else {ICF= "Intent change flag raised"; }
            if (message[pos].Substring(1, 1) == "0") { LNAV = "LNAV Mode engaged"; }
            else { LNAV = "LNAV Mode not engaged"; }
            int ps = Convert.ToInt32(message[pos].Substring(3,3), 2);
            if (ps==0) { PS = "No emergency / not reported"; }
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

        #region Data Item I021/210, MOPS Version
        /// <summary>
        /// Data Item I021/210, MOPS Version
        /// 
        /// Definition: Identification of the MOPS version used by a/c to supply ADS-B information.
        /// Format: One-octet fixed length Data Item
        /// </summary>

        public string VNS;
        public string LTT;
        public string MOPS;
        private int Compute_MOPS_Version(string[] message, int pos)
        {
            if (message[pos].Substring(1, 1) == "0") { VNS = "The MOPS Version is supported by the GS"; }
            else { VNS = "The MOPS Version is not supported by the GS"; }
            int ltt = Convert.ToInt32( message[pos].Substring(5,3),2);
            if (ltt == 0) { LTT = "Other"; }
            else if (ltt == 1) { LTT = "UAT"; }
            else if (ltt == 2) 
            {
                int vn = Convert.ToInt32(message[pos].Substring(2, 3), 2);
                string VN= "";
                if (vn == 0) { VN = "ED102/DO-260"; }
                if (vn == 1) { VN = "DO-260A"; }
                if (vn == 2) { VN = "ED102A/DO-260B"; }
                LTT= "Version Number: " + VN; 
            }
            else if (ltt == 3) { LTT = "VDL 4"; }
            else { LTT = "Not assigned"; }
            MOPS = LTT;
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/220, Met Information

        /// <summary>
        /// Data Item I021/220, Met Information
        /// 
        /// Definition : Meteorological information.
        /// Format : Compound data item consisting of a one byte primary sub-field,
        /// followed by up to four fixed length data fields.
        /// </summary>
        public int MET_present = 0;
        public string Wind_Speed;
        public string Wind_Direction;
        public string Temperature;
        public string Turbulence;
        private int Compute_Met_Information (string[] message, int pos)
        {
            MET_present = 1;
            int posin = pos;
            int posfin = pos++;
            if (message[posin].Substring(0, 1) == "1") {Wind_Speed = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin],message[posfin]), 2)) + " Knots"; posfin += 2;}
            if (message[posin].Substring(1, 1) == "1") { Wind_Direction = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin], message[posfin]), 2)) + " degrees"; posfin += 2; }
            if (message[posin].Substring(2, 1) == "1") { Temperature = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin], message[posfin]), 2)*0.25) + " ºC"; posfin += 2; }
            if (message[posin].Substring(3, 1) == "1") { Turbulence = Convert.ToString(Convert.ToInt32(string.Concat(message[posfin], message[posfin]), 2)) + " Turbulence"; posfin+=2; }
            return posfin;
        }

        #endregion

        #region Data Item I021/230, Roll Angle

        /// <summary>
        /// Data Item I021/230, Roll Angle
        /// 
        /// Definition : The roll angle, in two’s complement form, of an aircraft executing a turn.
        /// Format: A two byte fixed length data item.
        /// </summary>

        public string Roll_Angle;
        private int Compute_Roll_Angle(string[] message, int pos)
        {
            Roll_Angle = Convert.ToString(lib.ComputeA2Complement(string.Concat(message[pos],message[pos]))*0.01) + "º"; 
            return pos++; 
        }

        #endregion

        #region Data Item I021/250, Mode S MB Data
        /// <summary>
        /// Data Item I021/250, Mode S MB Data
        /// 
        /// Definition: Mode S Comm B data as extracted from the aircraft transponder.
        /// Format: Repetitive Data Item starting with a one-octet Field Repetition 
        /// Indicator(REP) followed by at least one BDS message
        /// </summary>

        public string[] MB_Data;
        public string[] BDS1;
        public string[] BDS2;
        public int modeS_rep;
        private int Compute_Mode_S_MB_DATA(string[] message, int pos)
        {
            int modeS_rep = Convert.ToInt32(message[pos], 2);
            if (modeS_rep < 0) {MB_Data = new string[modeS_rep];BDS1 = new string[modeS_rep]; BDS2 = new string[modeS_rep]; }
            pos++;
            for (int i=0;i<modeS_rep;i++)
            {
                MB_Data[i] = String.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5], message[pos + 6]);
                BDS1[1] = message[pos + 7].Substring(0, 4);
                BDS2[1] = message[pos + 7].Substring(4, 4);
                pos +=8;
            }
            return pos;
        }

        #endregion

        #region Data Item I021/260, ACAS Resolution Advisory Report
        /// <summary>
        /// Data Item I021/260, ACAS Resolution Advisory Report
        /// 
        /// Definition: Currently active Resolution Advisory (RA), if any, generated by the
        /// ACAS associated with the transponder transmitting the RA
        /// message and threat identity data.
        /// Format: Seven-octet fixed length Data Item.
        /// </summary>

        public string TYP;
        public string STYP;
        public string ARA;
        public string RAC;
        public string RAT;
        public string MTE;
        public string TTI;
        public string TID;
        private int Compute_ACAS_Resolution_Advisory_Report(string[] message, int pos)
        {
            string messg = string.Concat(message[pos], message[pos + 1], message[pos + 2], message[pos + 3], message[pos + 4], message[pos + 5], message[pos + 6]);
            TYP = messg.Substring(0,5);
            STYP = messg.Substring(5, 3);
            ARA = messg.Substring(8, 14);
            RAC = messg.Substring(22, 4);
            RAT = messg.Substring(26, 1);
            MTE = messg.Substring(27, 1);
            TTI = messg.Substring(28, 2);
            TID = messg.Substring(30, 26);
            pos +=7;
            return pos;
        }

        #endregion

        #region Data Item I021/271, Surface Capabilities and Characteristics

        /// <summary>
        /// Data Item I021/271, Surface Capabilities and Characteristics
        /// 
        /// Definition : Operational capabilities of the aircraft while on the ground.
        /// Format : Variable Length Data Item, comprising a primary subfield of one-octet,
        /// followed by an one-octet extensions if necessary.
        /// </summary>
        /// 
        public string POA;
        public string CDTIS;
        public string B2_low;
        public string RAS;
        public string IDENT;
        public string LengthandWidth;
        private int Compute_Surface_Capabiliteies_and_Characteristics (string[] message, int pos)
        {
            
            if (message[pos].Substring(2, 1) == "0") { POA = "Position transmitted is not ADS-B position reference point"; }
            else { POA = "Position transmitted is the ADS-B position reference point"; }
            if (message[pos].Substring(3, 1) == "0") { CDTIS = "Cockpit Display of Traffic Information not operational"; }
            else { CDTIS = "Cockpit Display of Traffic Information operational"; }
            if (message[pos].Substring(4, 1) == "0") { B2_low= "Class B2 transmit power ≥ 70 Watts"; }
            else { B2_low= "Class B2 transmit power < 70 Watts"; }
            if (message[pos].Substring(5, 1) == "0") { RAS = "Aircraft not receiving ATC-services"; }
            else { RAS = "Aircraft receiving ATC services"; }
            if (message[pos].Substring(6, 1) == "0") { IDENT = "IDENT switch not active"; }
            else { IDENT = "IDENT switch active"; }
            if (message[pos].Substring(7, 1) == "1") 
            {
                pos++;
                int LaW =Convert.ToInt32(message[pos].Substring(4,4),2) ; 
                if ( LaW == 0) { LengthandWidth  = "Lenght < 15  and Width < 11.5";  }
                if (LaW == 1) { LengthandWidth = "Lenght < 15  and Width < 23"; }
                if (LaW == 2) { LengthandWidth = "Lenght < 25  and Width < 28.5"; }
                if (LaW == 3) { LengthandWidth = "Lenght < 25  and Width < 34"; }
                if (LaW == 4) { LengthandWidth = "Lenght < 35  and Width < 33"; }
                if (LaW == 5) { LengthandWidth = "Lenght < 35  and Width < 38"; }
                if (LaW == 6) { LengthandWidth = "Lenght < 45  and Width < 39.5"; }
                if (LaW == 7) { LengthandWidth = "Lenght < 45  and Width < 45"; }
                if (LaW == 8) { LengthandWidth = "Lenght < 55  and Width < 45"; }
                if (LaW == 9) { LengthandWidth = "Lenght < 55  and Width < 52"; }
                if (LaW == 10) { LengthandWidth = "Lenght < 65  and Width < 59.5"; }
                if (LaW == 11) { LengthandWidth = "Lenght < 65  and Width < 67"; }
                if (LaW == 12) { LengthandWidth = "Lenght < 75  and Width < 72.5"; }
                if (LaW == 13) { LengthandWidth = "Lenght < 75  and Width < 80"; }
                if (LaW == 14) { LengthandWidth = "Lenght < 85  and Width < 80"; }
                if (LaW == 15) { LengthandWidth = "Lenght > 85  and Width > 80"; }
            }
            pos++;
            return pos;
        }

        #endregion

        #region Data Item I021/295, Data Ages
        /// <summary>
        /// Data Item I021/295, Data Ages
        /// 
        /// Definition : Ages of the data provided.
        /// Format : Compound Data Item, comprising a primary subfield of up to five
        /// octets, followed by the indicated subfields.
        /// </summary>

        public int Data_Ages_present=0;
        public string AOS;
        public string TRD;
        public string M3A;
        public string QI;
        public string TI;
        public string MAM;
        public string GH;
        public string FL;
        public string ISA;
        public string FSA;
        public string AS;
        public string TAS;
        public string MH;
        public string BVR;
        public string GVR;
        public string GV;
        public string TAR;
        public string TI_DataAge;
        public string TS_DataAge;
        public string MET;
        public string ROA;
        public string ARA_DataAge;
        public string SCC;
        private int Compute_Data_Age(string[] message, int pos)
        {
            Data_Ages_present = 1;
            int posin = pos;
            if (message[pos].Substring(7, 1) == "1")
            {
                pos++;
                if (message[pos].Substring(7, 1) == "1")
                {
                    pos++;
                    if (message[pos].Substring(7, 1) == "1")
                    {
                        pos++;
                    }
                }
            }
            pos++;
            if (message[posin].Substring(0, 1) == "1") { AOS = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            if (message[posin].Substring(1, 1) == "1") { TRD = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            if (message[posin].Substring(2, 1) == "1") { M3A = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            if (message[posin].Substring(3, 1) == "1") { QI = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            if (message[posin].Substring(4, 1) == "1") { TI = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            if (message[posin].Substring(5, 1) == "1") { MAM = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            if (message[posin].Substring(6, 1) == "1") { GH = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            if (message[posin].Substring(7, 1) == "1")
            {
                if (message[posin + 1].Substring(0, 1) == "1") { FL = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 1].Substring(1, 1) == "1") { ISA = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 1].Substring(2, 1) == "1") { FSA = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 1].Substring(3, 1) == "1") { AS = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 1].Substring(4, 1) == "1") { TAS = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 1].Substring(5, 1) == "1") { MH = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 1].Substring(6, 1) == "1") { BVR = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            }
            if (message[posin+1].Substring(7, 1) == "1")
            {
                if (message[posin + 2].Substring(0, 1) == "1") { GVR = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 2].Substring(1, 1) == "1") { GV = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 2].Substring(2, 1) == "1") { TAR = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 2].Substring(3, 1) == "1") { TI_DataAge = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 2].Substring(4, 1) == "1") { TS_DataAge = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 2].Substring(5, 1) == "1") { MET = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 2].Substring(6, 1) == "1") { ROA = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            }
            if (message[posin+2].Substring(7, 1) == "1")
            {
                if (message[posin + 3].Substring(0, 1) == "1") { ARA_DataAge = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
                if (message[posin + 3].Substring(1, 1) == "1") { SCC = Convert.ToString(Convert.ToInt32(message[pos], 2) * 0.1) + " s"; pos++; }
            }
            return pos; 
        }

        #endregion

        #region Data Item I021/400, Receiver ID

        /// <summary>
        /// Data Item I021/400, Receiver ID
        /// 
        /// Definition : Designator of Ground Station in Distributed System.
        /// Format : One-octet fixed length Data Item.
        /// </summary>
        //RECEIVER ID
        public string Receiver_ID;
        private int Compute_Receiver_ID(string[] message, int pos) 
        { 
            Receiver_ID = Convert.ToString(Convert.ToInt32(message[pos],2));
            pos++;
            return pos;
        }

        #endregion

        #endregion

    }

}
