using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PGTAWPF
{

    /// <summary>
    ///This class includes messages from all categories. In this way, we can have all the messages in the same list/table. 
    ///The class will create a new instance of a message with the attributes common to all the categories from a message of a specific category.
    /// </summary>
    /// <param name="num">Number of the message. It is the same number for the message of this class as the message of the class of its specific category </param>
    /// <param name="CAT">Category of the message</param>
    /// <param name="SIC">SIC of the message</param>
    /// <param name="SAC">SAC of the message</param>
    /// <param name="Target_Itentification">Target identification of the message</param>
    /// <param name="Target_Address">Target address of the message</param>
    /// <param name="Time_Of_day">Time value in int</param>
    /// <param name="List_Time_of_Day">Time value in int, used for the map. This value takes into account not only the time that the message brings but also the day of which the message is, 
    /// in this way if we have files for several days the program will know what day and time the message is from thanks to this parameter</param>
    /// <param name="Track_number">Message track number</param>
    /// <param name="Latitude_in_WGS_84">Latitude value in decimal degrees used for map representation</param>
    /// <param name="Longitude_in_WGS_84">Latitude value in decimal degrees used for map representation</param>
    /// <param name="Flight_level">Vehicle flight level</param>
    /// <param name="type">Indicates if the vehicle is a surface vehicle (car) if it is an aircraft (plane) or undetermined (Undetermined)</param>
    /// <param name="DetectionMode">Indicates the type of detection mode (ADS-B, MLAT or SMR) </param>
    /// <param name="direction">Direction to which the vehicle is headed</param>
    /// <param name="refreshratio">Indicates the refresh rate used by the radar detected by that vehicle.</param>

    public class CATALL
    {
        
        public int num;
        public string CAT;
        public string SAC;
        public string SIC;
        public string Target_Identification;
        public string Target_Address;
        public int Time_Of_day;
        public int List_Time_Of_Day;
        public string Track_number;
        public double Latitude_in_WGS_84;
        public double Longitude_in_WGS_84;
        public string Flight_level;
        public string type;
        public string DetectionMode;
        public int direction;
        public int refreshratio = -1;
 
        public CATALL() { }


        /// <summary>
        /// Create an instance of class CAT all from an instance of class CAT10
        /// </summary>
        /// <param name="message">CAT10 instance from which you want to create a CAT All instance</param>
        /// <param name="First_time_of_day">Parameter that indicates the time 00:00:00 of the day of the message (if the message is from the second day the time will be 86400)</param>
        /// <param name="first_time">Indicates the time of the first message in that file. In this way, if a file lasts 2
        /// days we can identify the messages that are on the second day and host them correctly</param>
        public CATALL(CAT10 message,int First_time_of_day,int first_time)
        {
            this.num = message.num;
            this.CAT = message.CAT;
            this.SAC = message.SAC;
            this.SIC = message.SIC;
            this.Target_Identification = message.TAR;
            this.Target_Address = message.Target_Address;
            this.Track_number =message.Track_Number;
            this.Latitude_in_WGS_84 = message.LatitudeWGS_84_map;
            this.Longitude_in_WGS_84 = message.LongitudeWGS_84_map;
            this.Flight_level = message.Flight_Level;
            if (message.Time_of_day_sec < first_time) { this.List_Time_Of_Day = message.Time_of_day_sec + 86400 + First_time_of_day; }
            else { this.List_Time_Of_Day = message.Time_of_day_sec + First_time_of_day; }
            if (message.TOT == "Ground vehicle") { type = "car"; }
            else if (message.TOT == "Aircraft") { type = "plane"; }
            else { type = "undetermined"; }
            if (message.TYP == "PSR") { DetectionMode = "SMR"; }
            if (message.TYP == "Mode S MLAT") { DetectionMode = "MLAT"; }
        }

        /// <summary>
        ///  Create an instance of class CAT all from an instance of class CAT 21 version 2.1
        /// </summary>
        /// <param name="message">CAT 21 version 2.1 instance from which you want to create a CAT All instance</param>
        /// <param name="First_time_of_day">Parameter that indicates the time 00:00:00 of the day of the message (if the message is from the second day the time will be 86400)</param>
        /// <param name="first_time">Indicates the time of the first message in that file. In this way, if a file lasts 2
        /// days we can identify the messages that are on the second day and host them correctly</param>
        public CATALL(CAT21vs21 message, int First_time_of_day, int first_time)
        {
            this.num = message.num;
            this.CAT = message.CAT;
            this.SAC = message.SAC;
            this.SIC = message.SIC;
            this.Target_Identification = message.Target_Identification;
            this.Target_Address = message.Target_address;
            this.Track_number = message.Track_Number;
            this.Latitude_in_WGS_84 = message.LatitudeWGS_84_map;
            this.Longitude_in_WGS_84 = message.LongitudeWGS_84_map;
            this.Flight_level = message.Flight_Level;
            if (message.Time_of_day_sec < first_time) { this.List_Time_Of_Day = message.Time_of_day_sec + 86400 + First_time_of_day; }
            else { this.List_Time_Of_Day = message.Time_of_day_sec + First_time_of_day; }
            if (message.ECAT == "Surface emergency vehicle" || message.ECAT == "Surface service vehicle") { type = "car"; }
            else if (message.ECAT == "Light aircraft" || message.ECAT == "Small aircraft" || message.ECAT == "Medium aircraft" || message.ECAT == "Heavy aircraft"){ type = "plane";}
            else { type = "undetermined"; }
            DetectionMode = "ADSB";
        }

        /// <summary>
        /// Create an instance of class CAT all from an instance of class CAT 21 v. 0.23
        /// </summary>
        /// <param name="message">CAT 21 v. 0.23 instance from which you want to create a CAT All instance</param>
        /// <param name="First_time_of_day">Parameter that indicates the time 00:00:00 of the day of the message (if the message is from the second day the time will be 86400)</param>
        /// <param name="first_time">Indicates the time of the first message in that file. In this way, if a file lasts 2
        /// days we can identify the messages that are on the second day and host them correctly</param>
        public CATALL(CAT21vs23 message, int First_time_of_day, int first_time)
        {
            this.num = message.num;
            this.CAT = message.CAT;
            this.SAC = message.SAC;
            this.SIC = message.SIC;
            this.Target_Identification = message.Target_Identification;
            this.Target_Address = message.Target_address;
            this.Latitude_in_WGS_84 = message.LatitudeWGS_84_map;
            this.Longitude_in_WGS_84 = message.LongitudeWGS_84_map;
            this.Flight_level = message.Flight_Level;
            if (message.Time_of_day_sec < first_time) { this.List_Time_Of_Day = message.Time_of_day_sec + 86400 + First_time_of_day; }
            else { this.List_Time_Of_Day = message.Time_of_day_sec + First_time_of_day; }
            if (message.ECAT == "Surface emergency vehicle" || message.ECAT == "Surface service vehicle") { type = "car"; }
            else if (message.ECAT == "Light aircraft" || message.ECAT == "Medium aircraft" || message.ECAT == "Heavy aircraft") { type = "plane"; }
            else { type = "undetermined"; }
            DetectionMode = "ADSB";
        }


        /// <summary>
        /// Create an instance of class CAT all from an instance of class CAT 21 v. 0.23
        /// </summary>
        /// <param name="message">CAT 21 v. 0.23 instance from which you want to create a CAT All instance</param>
        /// <param name="First_time_of_day">Parameter that indicates the time 00:00:00 of the day of the message (if the message is from the second day the time will be 86400)</param>
        /// <param name="first_time">Indicates the time of the first message in that file. In this way, if a file lasts 2
        /// days we can identify the messages that are on the second day and host them correctly</param>
        public CATALL(CAT62 message, int First_time_of_day, int first_time)
        {
            this.num = message.num;
            this.CAT = message.CAT;
            this.SAC = message.SAC;
            this.SIC = message.SIC;
            this.Target_Identification = message.Target_Identification;
            this.Target_Address = message.Derived_Data_Address;
            this.Latitude_in_WGS_84 = message.LatitudeWGS_84_map;
            this.Longitude_in_WGS_84 = message.LongitudeWGS_84_map;
            this.Flight_level = message.Measured_Flight_Level;
            if (message.Time_of_day_sec < first_time)
            {
                this.List_Time_Of_Day = message.Time_of_day_sec + 86400 + First_time_of_day;
            }
            else 
            { 
                this.List_Time_Of_Day = message.Time_of_day_sec + First_time_of_day; 
            }
            if (message.Derived_Data_EMC_ECAT == "Surface emergency vehicle" || message.Derived_Data_EMC_ECAT == "Surface service vehicle") 
            {
                type = "car"; }
            else if (message.Derived_Data_EMC_ECAT == "Light aircraft" || message.Derived_Data_EMC_ECAT == "Medium aircraft" || message.Derived_Data_EMC_ECAT == "Heavy aircraft") { type = "plane"; }
            else { type = "undetermined"; }
            DetectionMode = "ADSB";
        }
    }
}
