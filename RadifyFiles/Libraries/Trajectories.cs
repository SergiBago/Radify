using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GMap.NET;


namespace PGTAWPF
{
    /// <summary>
    /// Point class with time.
    /// 
    /// Useful when creating the trajectories, since each trajectory will have the typical parameters of a message, 
    /// and also a list of points. We want that these points, apart from having a location, also have time, 
    /// to be able to use them in the calculation of direction that is applied when loading files.
    /// </summary>
    public class PointWithTime
    {
        public PointLatLng point = new PointLatLng();
        public int time;
        public PointWithTime(PointLatLng p, int t)
        {
            this.point = p;
            this.time = t;
        }
    }

    /// <summary>
    /// Trajectories class
    /// 
    /// Used for the decodification and computing direction and refresh rate of radars, and also used for the export to KML function.
    /// </summary>
    public class Trajectories
    {
        public string CAT;
        public string SAC;
        public string SIC;
        public string Target_Identification;
        public string Target_Address;
        public int Last_Time_Of_day;
        public int First_time_of_day;
        public string Track_number;
        public string type;
        public string DetectionMode;
        public List<PointLatLng> ListPoints = new List<PointLatLng>();
        public List<PointWithTime> ListTimePoints = new List<PointWithTime>();

        /// <summary>
        /// Creates a new instance of the trajectories class
        /// </summary>
        /// <param name="Callsign">Vehicle Target Identification</param>
        /// <param name="Time">First time. Used in export KML so, we can know which time is the first point (we will also search for last point time later)</param>
        /// <param name="lat">Latitude of first point</param>
        /// <param name="lon">Longitude of first point</param>
        /// <param name="emitter">Indicates if the vehicle is a surface vehicle (car) if it is an aircraft (plane) or undetermined (Undetermined)</param>
        /// <param name="TargetAddress">Target address of the message</param>
        /// <param name="DetectionMode">Indicates the type of detection mode (ADS-B, MLAT or SMR)</param>
        /// <param name="CAT">Category of the message</param>
        /// <param name="SAC">SAC of the message</param>
        /// <param name="SIC">SIC of the message</param>
        /// <param name="Track_number">Message track number</param>
        public Trajectories(string Callsign, int Time, double lat,double lon, string emitter, string TargetAddress, string DetectionMode, string CAT, string SAC, string SIC, string Track_number)
        {
            PointLatLng p = new PointLatLng(lat, lon);
            this.ListPoints.Add(p);
            PointWithTime pt = new PointWithTime(p, Time);
            this.ListTimePoints.Add(pt);
            this.CAT = CAT;
            this.Target_Identification = Callsign;
            this.SAC = SAC;
            this.SIC = SIC;
            this.Target_Address = TargetAddress;
            this.First_time_of_day = Time;
            if (emitter == "car") { this.type = "Surface vehicle"; }
            else if (emitter == "plane") { this.type = "Aircraft"; }
            else {this.type = emitter; }
            this.DetectionMode = DetectionMode;
            this.Track_number = Track_number;
            this.ListPoints.Add(p);
        }

        /// <summary>
        /// Adds a new point whithout time to the trajectory
        /// </summary>
        /// <param name="lat">Latitude of the point</param>
        /// <param name="lon">Longitude of the point</param>
        /// <param name="time">Time of the point (used so in case this is the last point we know when the trajectory ends) (for KML export)</param>
        public void AddPoint(double lat,double lon,int time)
        {
            PointLatLng p = new PointLatLng(lat,lon);
            this.ListPoints.Add(p);
            this.Last_Time_Of_day = time;
        }

        /// <summary>
        /// Adds a new point whith time to the trajectory
        /// </summary>
        /// <param name="lat">Latitude of the point</param>
        /// <param name="lon">Longitude of the point</param>
        /// <param name="time">Time of the point</param>
        public void AddTimePoint(double lat, double lon, int time)
        {   
            PointLatLng p = new PointLatLng(lat, lon);
            PointWithTime TimePoint = new PointWithTime(p, time);
            ListTimePoints.Add(TimePoint);

        }

        /// <summary>
        /// Counts Time Points list length
        /// </summary>
        /// <returns>Time Point List Length</returns>
        public int CountTimepoint()
        {
            return this.ListTimePoints.Count();
        }

        /// <summary>
        /// Gets the string to wrote in a file, so KML trajectory is created
        /// </summary>
        /// <returns>string to wrote in file to create this KML trajectory</returns>
        public string GetTrajectorieKML()
        {
            StringBuilder KMLBuilder = new StringBuilder();
            string caption;
            if (Target_Identification!= null) { caption = Target_Identification; }
            else if (Target_Address != null ) { caption = "T.A:"+Target_Address; }
            else if (Track_number != null ) { caption = "T.N:"+Track_number; }
            else { caption = "No Data"; }
            string color="ff000000";
            if (DetectionMode == "SMR") { color = "ff00ff00"; }
            else if (DetectionMode == "MLAT") { color = "ff00ffff"; }
            else if (DetectionMode == "ADSB") { color = "ff0080ff"; }
            else if (DetectionMode == "CAT 62") { color = "fffc046d"; }
            KMLBuilder.AppendLine("<Placemark>");
            KMLBuilder.AppendLine("<Style id='yellowLineGreenPoly'>");
            KMLBuilder.AppendLine("<LineStyle>");
            KMLBuilder.AppendLine("<color>"+color+"</color>");
            KMLBuilder.AppendLine("<width>4</width>");
            KMLBuilder.AppendLine("</LineStyle>");
            KMLBuilder.AppendLine("<PolyStyle>");
            KMLBuilder.AppendLine("<color>7f00ff00</color>");
            KMLBuilder.AppendLine("</PolyStyle>");
            KMLBuilder.AppendLine("</Style>");
            KMLBuilder.AppendLine("<name>"+caption+"</name>");
            KMLBuilder.AppendLine("<description>"+ GetDescription() +"</description>");
            KMLBuilder.AppendLine("<styleUrl>#yellowLineGreenPoly</styleUrl>");
            KMLBuilder.AppendLine("<LineString>");
            KMLBuilder.AppendLine(KMLcoordenates());
            KMLBuilder.AppendLine("</LineString>");
            KMLBuilder.AppendLine("</Placemark>");
            return KMLBuilder.ToString();
        }

        /// <summary>
        /// Gets description of KML trajectory.
        /// 
        /// Used in GetTrajectorieKML to get the description the KML trajectory must get.
        /// Description has the most significant parameters of a vehicle, as CAT,SIC,SAC,Target ID, Target Add, Detection mode,...
        /// </summary>
        /// <returns></returns>
        private string GetDescription()
        {
            StringBuilder Description = new StringBuilder();
            if (CAT != null) { Description.Append("CAT: " + CAT); }
            if (SAC != null) {Description.Append("; SAC: " + SAC); }
            if (SIC != null) { Description.Append("; SIC: " + SIC); }
            if (Target_Identification != null) { Description.Append("; Target Id: " +Target_Identification); }
            Description.Append("; Detection Mode: " + DetectionMode);
            if (Target_Address != null) { Description.Append("; Target Address: " + Target_Address); }
            if (Track_number != null ) { Description.Append("; Track Number: " + Track_number); }
            if (type!= null ) { Description.Append("; Type of vehicle: " + type); }
            Description.Append(";First detected time: " +ComputeTime(First_time_of_day));
            Description.Append(";Last detected time: " + ComputeTime(Last_Time_Of_day));
            return Description.ToString();
        }

        /// <summary>
        /// Gets a string with time format from the int number of seconds
        /// </summary>
        /// <param name="time">number of seconds pased since 00:00:00</param>
        /// <returns>string in format 00:00:00 with time</returns>
        private string ComputeTime(int time)
        {
            int showingtime;
            if (time > 86400) { showingtime = time - 86400; }
            else
            {
                showingtime = time;
            }
            int hour = Convert.ToInt32(Math.Truncate(Convert.ToDouble(showingtime / 3600)));
            int min = Convert.ToInt32(Math.Truncate(Convert.ToDouble((showingtime - (hour * 3600)) / 60)));
            int sec = Convert.ToInt32(Math.Truncate(Convert.ToDouble((showingtime - ((hour * 3600) + (min * 60))))));
            string hours= Convert.ToString(hour).PadLeft(2, '0');
            string minutes = Convert.ToString(min).PadLeft(2, '0');
            string seconds = Convert.ToString(sec).PadLeft(2, '0');
            return (hours+":" + minutes + ":" + seconds);
        }

        /// <summary>
        /// Returns a string in KML format with the coordinates of all the trajectory points
        /// </summary>
        /// <returns>string in KML format with the coordinates of all the trajectory points</returns>
        private string KMLcoordenates()
        {
            StringBuilder KMLcoor = new StringBuilder();
            KMLcoor.AppendLine("<coordinates>");
            foreach (PointLatLng p in ListPoints)
            {
                string Lat = Convert.ToString(p.Lat).Replace(",", ".");
                string Lon = Convert.ToString(p.Lng).Replace(",", ".");
                KMLcoor.AppendLine(Lon + "," + Lat);
            }
            KMLcoor.AppendLine("</coordinates>");
            return KMLcoor.ToString();
        }

      

    }
}
