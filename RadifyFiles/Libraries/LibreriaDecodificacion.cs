using System;
using GMap.NET;
using MultiCAT6.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PGTAWPF
{
    /// <summary>
    /// Library with useful functions to Decode messages, that serve for all categories
    /// </summary>
    public class LibreriaDecodificacion
    {
        /// <summary>
        /// Dictionary to go from Hexadecimal to binary
        /// </summary>
        readonly private Dictionary<char, string> HexadecimalAbinario = new Dictionary<char, string>
        {
            { '0', "0000" },
            { '1', "0001" },
            { '2', "0010" },
            { '3', "0011" },
            { '4', "0100" },
            { '5', "0101" },
            { '6', "0110" },
            { '7', "0111" },
            { '8', "1000" },
            { '9', "1001" },
            { 'A', "1010" },
            { 'B', "1011" },
            { 'C', "1100" },
            { 'D', "1101" },
            { 'E', "1110" },
            { 'F', "1111" }
        };


        /// <summary>
        /// Dictionary to go from Bynary to  Hexadecimal
        /// </summary>
        readonly private Dictionary<string, char> BinarioaHexadecimal = new Dictionary<string, char>
        {
            {"0000", '0' },
            {"0001", '1'},
            {"0010",'2' },
            {"0011",'3' },
            {"0100",'4' },
            {"0101",'5' },
            {"0110",'6' },
            {"0111",'7' },
            {"1000",'8' },
            {"1001",'9' },
            {"1010",'A' },
            {"1011",'B' },
            {"1100",'C' },
            {"1101",'D' },
            {"1110",'E' },
            {"1111",'F' }
        };


        readonly private Dictionary<int, string> Decimal_a_ASCII = new Dictionary<int, string>
        {
            { 48, "0" },
            { 49, "1" },
            { 50, "2" },
            { 51, "3" },
            { 52, "4" },
            { 53, "5" },
            { 54, "6" },
            { 55, "7" },
            { 56, "8" },
            { 57, "9" },
          
            { 65, "A" },
            { 66, "B" },
            { 67, "C" },
            { 68, "D" },
            { 69, "E" },
            { 70, "F" },
            { 71, "G" },
            { 72, "H" },
            { 73, "I" },
            { 74, "J" },
            { 75, "K" },
            { 76, "L" },
            { 77, "M" },
            { 78, "N" },
            { 79, "O" },
            { 80, "P" },
            { 81, "Q" },
            { 82, "R" },
            { 83, "S" },
            { 84, "T" },
            { 85, "U" },
            { 86, "V" },
            { 87, "W" },
            { 88, "X" },
            { 89, "Y" },
            { 90, "Z" },
        };

        public string OctetoBinarioASCII(string octeto)
        {
            int num = Convert.ToInt32(octeto, 2);
            string Character = Decimal_a_ASCII[num];
            return Character;

        }

        /// <summary>
        /// Function to pass an octet from hexadecimal to binary
        /// </summary>
        /// <param name="octeto">Octet in Hexadecimal Form</param>
        /// <returns> Octet in binary </returns>
        public string HexaoctetoAbinario (string octeto)
        {
            octeto = this.Zerodelante(octeto);
            StringBuilder Octeto = new StringBuilder();
            foreach (char a in octeto)
            {
                Octeto.Append(HexadecimalAbinario[char.ToUpper(Convert.ToChar(a))]);
            }
             return Octeto.ToString();
        }

        /// <summary>
        /// Function to pass an octet from binary to hexadecimal
        /// </summary>
        /// <param name="octeto">Octet in Bynary form</param>
        /// <returns>Octet in Hexadecimal form</returns>
        public string BinarytoHexa(string octeto)
        {
            StringBuilder Octeto = new StringBuilder();
            Octeto.Append(BinarioaHexadecimal[octeto.Substring(0,4)]);
            Octeto.Append(BinarioaHexadecimal[octeto.Substring(4, 4)]);
            return Octeto.ToString();
        }

        /// <summary>
        /// Insert a 0 in front of an octet if its length is 1 hexadecimal character (an octet must always have 2 characters, if it only has 1, the first is 0)
        /// </summary>
        /// <param name="octeto">Input octet, which can be 1 or 2 characters long.</param>
        /// <returns>Output octet, with two characters</returns>
        public string Zerodelante (string octeto)
        {
            if (octeto.Length==1)
            {
                return string.Concat('0', octeto);
            }
            else
            {
                return octeto;
            }
        }

        /// <summary>
        /// Computes the FSPEC of a message
        /// </summary>
        /// <param name="message">Full message in string[] form where each string is an octet</param>
        /// <returns>FSPEC string, without the continuity bits</returns>
        public string FSPEC (string [] message)
        {
            string FSPEC1 = "";
            int pos = 3;
            bool continua = true;
            while (continua == true)
            {
                string newocteto = Convert.ToString(Convert.ToInt32(message[pos], 16), 2).PadLeft(8, '0');
                FSPEC1 +=  newocteto.Substring(0,7);
                if (newocteto.Substring(7, 1) == "1") 
                    pos++;
                else 
                    continua = false;
            }
            return FSPEC1;
        }

        /// <summary>
        /// Convert entire message to binary
        /// </summary>
        /// <param name="mensaje">message in hexadecimal form</param>
        /// <returns> message in binary form </returns>
        public string[] Passarmensajeenteroabinario(string[] mensaje)
        {
            string[] Mensajebinario = new string[mensaje.Length];
            for (int i=0; i<mensaje.Length; i++) {Mensajebinario[i] = this.HexaoctetoAbinario(mensaje[i]);}
            return Mensajebinario;
        }

        /// <summary>
        /// Calculate WGS84 coordinates from Cartesian
        /// </summary>
        /// <param name="p"> Cartesian coordenates of the object</param>
        /// <param name="SIC">SIC of the radar which detected the object, to know which radar is and where it's placed </param>
        /// <returns>Object WGS84 coordinates</returns>
        public PointLatLng ComputeWGS_84_from_Cartesian(Point p, string SIC)
        {
            PointLatLng pos = new PointLatLng();
            double X = p.X;
            double Y = p.Y;
            CoordinatesXYZ ObjectCartesian = new CoordinatesXYZ(X, Y, 0); //We pass from Point to CoordinatesXYZ to be able to work with the GeoUtils library
            PointLatLng AirportPoint = GetCoordenatesSMRMALT(Convert.ToInt32(SIC)); //We get the Radar coordinates from its SIC
            CoordinatesWGS84 AirportGeodesic = new CoordinatesWGS84(AirportPoint.Lat * (Math.PI / 180), AirportPoint.Lng * (Math.PI / 180)); //We went from PointLatLng to Coordinates WGS84 to be able to work with GeoUtils. Coordinates must be passed from degrees to radians
            GeoUtils geoUtils = new GeoUtils();
            CoordinatesWGS84 MarkerGeodesic = geoUtils.change_system_cartesian2geodesic(ObjectCartesian, AirportGeodesic); //We apply the change from CoordiantesXYZ to Coordinate GS83
            geoUtils = null;
            double LatitudeWGS_84_map = MarkerGeodesic.Lat * (180 / Math.PI);
            double LongitudeWGS_84_map = MarkerGeodesic.Lon * (180 / Math.PI);
            pos.Lat = LatitudeWGS_84_map;
            pos.Lng = LongitudeWGS_84_map;
            return pos;
        }

        /// <summary>
        /// Function to calculate complement A2. Useful for decoding many parameters, as many come in this form.
        /// </summary>
        /// <param name="bits">string with the bits</param>
        /// <returns>A2 Complement of the introduced bits string</returns>
        public double ComputeA2Complement(string bits)
        {
            if (Convert.ToString(bits[0]) == "0")
            {
                int num = Convert.ToInt32(bits, 2);
                return Convert.ToSingle(num);
            }
            else
            {
                string bitss = bits.Substring(1, bits.Length - 1);
                string newbits = "";
                int i = 0;
                while (i < bitss.Length)
                {
                    if (Convert.ToString(bitss[i]) == "1")
                        newbits +=  "0";
                    if (Convert.ToString(bitss[i]) == "0")
                        newbits +=  "1";
                    i++;
                }
                double num = Convert.ToInt32(newbits, 2);
                return -(num + 1);
            }

        }

        /// <summary>
        /// Convert from Decimal to Octal
        /// </summary>
        /// <param name="decimalNumber">Number in decimal</param>
        /// <returns>decimal imput converted to Octal </returns>
        public int ConvertDecimalToOctal(int decimalNumber)
        {
            int octalNumber = 0, i = 1;

            while (decimalNumber != 0)
            {
                octalNumber += (decimalNumber % 8) * i;
                decimalNumber /= 8;
                i *= 10;
            }

            return octalNumber;
        }

        /// <summary>
        /// Compute Char from bynary
        /// </summary>
        /// <param name="Char"></param>
        /// <returns>Computed Char</returns>
        public string Compute_Char(string Char)
        {
            int code = Convert.ToInt32(Char, 2);
            List<string> codelist = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            if (code == 0)
                return "";
            else
                return codelist[code - 1];
        }

        /// <summary>
        /// Calculate if the version of the CAT 21 message is v0.23 or v2.1
        /// </summary>
        /// <param name="message"> full message </param>
        /// <returns>0 if version =0.23 or 1 if version =2.1</returns>
        public int GetVersion (string[] message)
        {
            string[] mensaje = message;
            string FSPEC1 = this.FSPEC(mensaje);
            int longFSPEC = FSPEC1.Length / 7;
            int pos = 3 + longFSPEC;
            mensaje = this.Passarmensajeenteroabinario(mensaje);
            int SAC = Convert.ToInt32(mensaje[pos], 2);
            int SIC = Convert.ToInt32(mensaje[pos + 1], 2);
            if (SAC == 0 && SIC != 101) { return 0; }
            else { return 1; }
        }


        /// <summary>
        /// Computes the airport code from the SIC
        /// </summary>
        /// <param name="SIC"> SIC of the airport</param>
        /// <returns>Airport code</returns>
        public int GetAirporteCode(int SIC)
        {
            int i = 0;
            if (SIC==107 || SIC ==7 || SIC==219)  { i= 0; } //BARCELONA
            else if (SIC==5 || SIC== 105 || SIC==209) { i= 1; } //ASTURIAS
            else if (SIC==2 || SIC== 102 ) { i= 2; } //PALMA
            else if (SIC==6 || SIC==106 || SIC== 227 || SIC== 228) { i= 3; } //SANTIAGO
            else if (SIC==3 || SIC==4 || SIC==104) { i= 4; } //BARAJAS
            else if (SIC == 1 || SIC == 101 ) { i = 5; } //TENERIFE
            else if (SIC == 108) { i = 6; } //Malaga
            else if (SIC==203) { i=7; } //Bilbao
            else if (SIC == 206 ) { i=8; } //ALICANTE
            else if (SIC == 207) { i=9; } //GRANADA
            else if (SIC == 210) { i=10; } //LANZAROTE
            else if (SIC == 211) { i=11; } //TURRILLAS
            else if (SIC == 212) { i=12; } //Menorca
            else if (SIC == 213 || SIC==229) { i=13; } //IBIZA
            else if (SIC == 214 ) { i=14; } //VALDESPINA
            else if (SIC == 215 || SIC==221) { i=15; } //PARACUELLOS
            else if (SIC == 216) { i=16; } //RANDA
            else if (SIC == 218) { i=17; } //GERONA
            else if (SIC == 220 || SIC==222) { i=18; } //ESPIÑEIRAS
            else if (SIC == 223) { i=19; } //VEJER
            else if (SIC == 224) { i=20; } //YESTE
            else if (SIC == 225 || SIC==226) { i=21; } //VIGO
            else if (SIC == 230) { i=22; } //VALENCIA
            else if (SIC == 231) { i=23; } //SEVILLA
            return i;
        }


        /// <summary>
        /// Returns de coordenates of the SMR or MLAT from the introduced SIC code
        /// </summary>
        /// <param name="SIC"> SIC code of the radar </param>
        /// <returns>Coordinates of the radar</returns>
        public PointLatLng GetCoordenatesSMRMALT(int SIC)
        {
            PointLatLng point = new PointLatLng(0, 0);
            if (SIC == 1) { point = SMRTenerife; }
            else if (SIC == 2) { point = SMRPalma; }
            else if (SIC == 3) { point = SMRBarajas_S; }
            else if (SIC == 4) { point = SMRBarajas_N; }
            else if (SIC == 5) { point = SMRAsturias; }
            else if (SIC == 6) { point = SMRSantiago; }
            else if (SIC == 7) { point = SMRBarcelona; }
            else if (SIC == 101) { point = ARPTenerife; }
            else if (SIC == 102) { point = ARPPalma; }
            else if (SIC == 104) { point = ARPBarajas; }
            else if (SIC == 105) { point = ARPAsturias; }
            else if (SIC==106) { point = ARPSantiago; }
            else if (SIC == 107) { point = ARPBarcelona; }
            else if (SIC==108) { point = ARPMalaga; }
            return point;
        }

        PointLatLng SMRAsturias = new PointLatLng(43.56464083, -6.030623056);
        PointLatLng SMRBarcelona = new PointLatLng(41.29561833, 2.095114167);
        PointLatLng SMRPalma = new PointLatLng(39.54864778, 2.732764444);
        PointLatLng SMRSantiago = new PointLatLng(42.89805333, -8.413033056);
        PointLatLng SMRBarajas_N = new PointLatLng(40.49184306, -3.569051667);
        PointLatLng SMRBarajas_S = new PointLatLng(40.46814972, -3.568730278);
        PointLatLng SMRTenerife = new PointLatLng(28.47749583, -16.33252028);
        PointLatLng ARPPalma = new PointLatLng(39.5486842, 2.73276111);
        PointLatLng ARPAsturias = new PointLatLng(43.56356722, -6.034621111);
        PointLatLng ARPBarajas = new PointLatLng(40.47224833, -3.560945278);
        PointLatLng ARPBarcelona = new PointLatLng(41.2970767, 2.07846278); 
        PointLatLng ARPMalaga = new PointLatLng(36.67497111, - 4.499206944);
        PointLatLng ARPTenerife = new PointLatLng(28.48265333, -16.34153722);
        PointLatLng ARPSantiago = new PointLatLng(42.896335, -8.41514361);
    }
}
