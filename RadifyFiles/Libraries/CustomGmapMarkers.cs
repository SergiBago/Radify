using System;
using System.Drawing;
using System.Windows.Shapes;
using GMap.NET.WindowsPresentation;
using GMap.NET;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows;
using System.Collections.Generic;

namespace PGTAWPF
{
    /// <summary>
    /// Class of own GMap markers, with all the attributes that interest us
    /// </summary>
    public class CustomActualGmapMarker : GMapMarker
    {

        public readonly string caption;
        public string Callsign;
        public int Time;
        public PointLatLng p;
        public int number;
        public string emitter;
        public string TargetAddress;
        public string DetectionMode;
        public string CAT;
        public string SAC;
        public string SIC;
        public string Flight_level;
        public string Track_number;
        public int type = 1;
        public int direction;
        public int refreshratio;

        /// <summary>
        /// We create a marker with all the necessary values. This will be useful for when we want to see the information of a marker, we will already have it in the marker itself, and we will not have to look for the information on the lists
        /// </summary>
        /// <param name="p">position of the marker in PointLatLng format</param>
        /// <param name="Callsign">Target identification of the message</param>
        /// <param name="time">Time value in int</param>
        /// <param name="num">Number of the message</param>
        /// <param name="emmiter">Indicates if the vehicle is a surface vehicle (car) if it is an aircraft (plane) or undetermined (Undetermined)</param>
        /// <param name="TargetAdd">Target address of the message</param>
        /// <param name="DetectionMode">Indicates the type of detection mode (ADS-B, MLAT or SMR)</param>
        /// <param name="CAT">Category of the message</param>
        /// <param name="SIC">SIC of the message</param>
        /// <param name="SAC">SAC of the message</param>
        /// <param name="Flight_level">Vehicle flight level</param>
        /// <param name="Track_number">Message track number</param>
        /// <param name="direction">Direction to which the vehicle is headed</param>
        /// <param name="refreshratio">Indicates the refresh rate used by the radar detected by that vehicle</param>
        public CustomActualGmapMarker(PointLatLng p, string Callsign, int time,  int num, string emmiter, string TargetAdd, string DetectionMode, string CAT, string SIC, string SAC, string Flight_level, string Track_number, int direction, int refreshratio)
            : base(p)
        {
            if (Callsign != null) { this.caption = Callsign; }
            else
            {
                if (TargetAdd != null) { this.caption = "A.: " + TargetAdd; }
                else
                {
                    if (Track_number != null) { this.caption = "N:" + Track_number; }
                    else { this.caption = "No data"; }
                }
            }
            this.DetectionMode = DetectionMode;
            this.emitter = emmiter;
            this.p = p;
            this.refreshratio = refreshratio;
            this.direction = direction;
            this.Callsign = Callsign;
            Time = time;
            this.TargetAddress = TargetAdd;
            this.Track_number = Track_number;
            this.number = num;
            this.CAT = CAT;
            this.SIC = SIC;
            if (SAC == "Data flow local to the airport") { this.SAC = "0"; }
            else { this.SAC = SAC; }
            if (Flight_level != null && Flight_level.Contains(":"))
            {
                this.Flight_level = Flight_level.Substring(Flight_level.IndexOf(':') + 1, (Flight_level.Length - Flight_level.IndexOf(':')) - 1);
            }
            else { this.Flight_level = Flight_level; }
        }

    }

    /// <summary>
    /// Class of own GMap markers, with all the attributes that interest us
    /// </summary>
    public class CustomOldGmapMarker : GMapMarker
    {
        public string caption;
        public string Callsign;
        public int Time;
        public PointLatLng p;
        public int number;
        public string emitter;
        public string TargetAddress;
        public string DetectionMode;
        public string CAT;
        public string SAC;
        public string SIC;
        public string Flight_level;
        public string Track_number;
        public int type = 0;
        public int direction;
        public int refreshratio;

        /// <summary>
        /// We create a marker with all the necessary values. This will be useful for when we want to see the information of a marker, we will already have it in the marker itself, and we will not have to look for the information on the lists
        /// </summary>
        /// <param name="p">position of the marker in PointLatLng format</param>
        /// <param name="label">Indicates whether the marker should be labeled or not. If is == 1 must not have a label </param>
        /// <param name="Callsign">Target identification of the message</param>
        /// <param name="time">Time value in int</param>
        /// <param name="num">Number of the message</param>
        /// <param name="emmiter">Indicates if the vehicle is a surface vehicle (car) if it is an aircraft (plane) or undetermined (Undetermined)</param>
        /// <param name="TargetAdd">Target address of the message</param>
        /// <param name="Detectionmode">Indicates the type of detection mode (ADS-B, MLAT or SMR)</param>
        /// <param name="CAT">Category of the message</param>
        /// <param name="SIC">SIC of the message</param>
        /// <param name="SAC">SAC of the message</param>
        /// <param name="Flight_level">Vehicle flight level</param>
        /// <param name="Track_number">Message track number</param>
        /// <param name="direction">Direction to which the vehicle is headed</param>
        /// <param name="refreshratio">Indicates the refresh rate used by the radar detected by that vehicle</param>
        public CustomOldGmapMarker(PointLatLng p, string Callsign, int label, int time, int num, string emmiter, string TargetAdd, string Detectionmode, string CAT, string SIC, string SAC, string Flight_level, string Track_number, int direction, int refreshratio)
                : base(p)
        {
            this.DetectionMode = Detectionmode;
            this.emitter = emmiter;
            this.p = p;
            if (label == 1)
            {
                caption = ""; 
            }
            else
            {
                if (Callsign != null) { this.caption = Callsign; }
                else
                {
                    if (TargetAdd != null) { this.caption = "A.: " + TargetAdd; }
                    else
                    {
                        if (Track_number != null) { this.caption = "N:" + Track_number; }
                        else { this.caption = "No data"; }
                    }
                }
            }
            this.direction = direction;
            this.Callsign = Callsign;
            this.Time = time;
            this.TargetAddress = TargetAdd;
            this.Track_number = Track_number;
            this.number = num;
            this.CAT = CAT;
            this.SIC = SIC;
            this.refreshratio = refreshratio;
            if (SAC == "Data flow local to the airport") { this.SAC = "0"; }
            else { this.SAC = SAC; }
            if (Flight_level != null && Flight_level.Contains(":"))
            {
                this.Flight_level = Flight_level.Substring(Flight_level.IndexOf(':') + 1, (Flight_level.Length - Flight_level.IndexOf(':')) - 1);
            }
            else { this.Flight_level = Flight_level; }
        }
    }

    /// <summary>
    /// Custom GmapMarker, which we will use for the labels of the lines of measure distances on the map
    /// </summary>
    public class LinesLabel : GMapMarker
    {
        public string caption;
        public int num;
        public LinesLabel(PointLatLng p, int i)
            : base(p)
        {
            this.num = i;
        }
    }

    /// <summary>
    /// Class for measuring lines on the map
    /// </summary>
    public class MeasureLine : GMapPolygon
    {

        public PointLatLng p;
        public PointLatLng p2;
        List<PointLatLng> ListPoints;
        public CustomActualGmapMarker marker1;
        public CustomActualGmapMarker marker2;
        public int num;
        public bool OldMarker = false;

        /// <summary>
        /// Create a measuring line between a marker and a specific point on the map
        /// </summary>
        /// <param name="p">Marker position</param>
        /// <param name="p2">Map point</param>
        /// <param name="ListPoints">List with the two points that create the line</param>
        /// <param name="marker1">Marker from which the distance is being measured</param>
        /// <param name="i">unique number that identifies each line</param>
        public MeasureLine(PointLatLng p, PointLatLng p2, List<PointLatLng> ListPoints, CustomActualGmapMarker marker1, int i)
            : base(ListPoints)
        {
            this.marker1 = marker1;
            this.p = marker1.p;
            this.p2 = p2;
            this.ListPoints = new List<PointLatLng>();
            this.ListPoints.Add(p);
            this.ListPoints.Add(p2);
            this.num = i;
        }

        /// <summary>
        /// Create a measuring line between two markers
        /// </summary>
        /// <param name="p">First Marker position</param>
        /// <param name="p2">Second Marker position</param>
        /// <param name="ListPoints">List with the two points that create the line</param>
        /// <param name="marker1">First marker</param>
        /// <param name="marker2">Second marker</param>
        /// <param name="i">unique number that identifies each line</param>
        /// <param name="oldmarker">Boolean indicating whether the second marker is old or new</param>
        public MeasureLine(PointLatLng p, PointLatLng p2, List<PointLatLng> ListPoints, CustomActualGmapMarker marker1, CustomActualGmapMarker marker2, int i, bool oldmarker)
    : base(ListPoints)
        {
            this.marker1 = marker1;
            this.marker2 = marker2;
            this.p = marker1.p;
            this.p2 = p2;
            this.ListPoints = new List<PointLatLng>();
            this.ListPoints.Add(p);
            this.ListPoints.Add(p2);
            this.num = i;
            OldMarker = oldmarker;
        }


        /// <summary>
        /// Calculate the distance and angle between the marker and the point or between both markers
        /// </summary>
        /// <returns>a string indicating the distance and the bearing</returns>
        public string ComputeParameters()
        {
            double lat = p2.Lat;
            double lon = p2.Lng;
            double lat2 = p.Lat;
            double lon2 = p.Lng;
            double dist = 6371000 * (Math.Acos(Math.Cos((Math.PI / 180) * (90 - lat)) * Math.Cos((Math.PI / 180) * (90 - lat2)) + Math.Sin((Math.PI / 180) * (90 - lat)) * Math.Sin((Math.PI / 180) * (90 - lat2)) * Math.Cos((Math.PI / 180) * (lon - lon2))));
            double X = p2.Lng - p.Lng;
            double Y = p2.Lat - p.Lat;
            double dir = ((Math.Atan2(Y, X) * (180 / Math.PI)) - 90);
            if (dir < 0) { dir = -dir; }
            else { dir = 360 - dir; }
            string Parameters = "d:" + String.Format("{0:0.00}", dist) + "m, A:" + String.Format("{0:0.00}", dir) + "º";
            return Parameters;
        }
    }
}

