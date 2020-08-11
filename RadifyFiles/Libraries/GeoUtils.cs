using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DotNetMatrix;

namespace MultiCAT6.Utils
{
    /// <summary>
    /// Everything you ever needed to perform coordinate transformations an so (v2.0 09/10/15)
    /// </summary>
    // TODO: see http://www.cpearson.com/excel/latlong.htm for Great Circle Distance
    public class GeoUtils
    {
        public static readonly object PositionRadarMatrixLock = new Object();
        public static readonly object RotationRadarMatrixLock = new Object();
        /// <summary>
        /// 1 meter -> 3,28084 feet
        /// </summary>
        public const double METERS2FEET = 3.28084;
        /// <summary>
        /// 1 feet -> 0,3048 meters
        /// </summary>
        public const double FEET2METERS = 0.3048;
        /// <summary>
        /// 1 meter -> 1/1852 nautic miles
        /// </summary>
        public const double METERS2NM = 1 / GeoUtils.NM2METERS;
        /// <summary>
        /// 1 nautic mile -> 1852 metres
        /// </summary>
        public const double NM2METERS = 1852.0;
        /// <summary>
        /// 1 degree -> pi/180.0 radians
        /// </summary>
        public const double DEGS2RADS = Math.PI / 180.0;
        /// <summary>
        /// 1 radian -> 180.0/pi degrees
        /// </summary>
        public const double RADS2DEGS = 180.0 / Math.PI;
        /// <summary>
        /// semi-major axis of the Europe-50 & WGS84 ellipsoid (in metres)
        /// </summary>
        public double A = 6378137.0;
        /// <summary>
        /// semi-minor axis of the Europe-50 & WGS84 ellipsoid (in metres)
        /// </summary>
        public double B = 6356752.3142;
        /// <summary>
        /// eccentricity of the ellipsoid squared
        /// </summary>
        public double E2 = 0.00669437999013;
        //(0.081819190843*0.081819190843);
        /// <summary>
        /// almost zero for zero comparisions with floating point maths
        /// </summary>
        public const double ALMOST_ZERO = 1e-10;
        /// <summary>
        /// almost zero for zero comparisions with floating point maths
        /// </summary>
        public const double REQUIERED_PRECISION = 1e-8;
        /// <summary>
        /// center of projection for the system cartesian coordinate transformation
        /// </summary>
        public CoordinatesWGS84 centerProjection;
        /// <summary>
        /// matrix for translation, from centerProjection
        /// </summary>
        private GeneralMatrix T1;
        /// <summary>
        /// matrix for rotation coordinates, from centerProjection
        /// </summary>
        private GeneralMatrix R1;
        /// <summary>
        /// best earth radius from centerProjection
        /// </summary>
        public double R_S = 0;

        /// <summary>
        /// rotation matrix for radar cartesian to geocentric
        /// </summary>
        private Dictionary<CoordinatesWGS84, GeneralMatrix> rotationMatrixHT = null;
        /// <summary>
        /// translation matrix (vector) for radar cartesian to geocentric
        /// </summary>
        private Dictionary<CoordinatesWGS84, GeneralMatrix> translationMatrixHT = null;
        /// <summary>
        /// position matrix (vector) for each radar in system coordinates (center on centerprojection)
        /// </summary>
        private Dictionary<CoordinatesWGS84, GeneralMatrix> positionRadarMatrixHT = null;
        /// <summary>
        /// rotation matrix for each radar in system coordinates (center on centerprojection)
        /// </summary>
        private Dictionary<CoordinatesWGS84, GeneralMatrix> rotationRadarMatrixHT = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GeoUtils() { }

        /// <summary>
        /// Constructor with initializers
        /// </summary>
        /// <param name="E">eccentricity of the ellipsoid</param>
        /// <param name="A">semi-major axis of the Europe-50 ellipsoid (in metres)</param>
        public GeoUtils(double E, double A)
        {
            this.E2 = E * E;
            this.A = A;
            setCenterProjection(new CoordinatesWGS84());
        }
        /// <summary>
        /// Constructor with initializers
        /// </summary>
        /// <param name="E">eccentricity of the ellipsoid</param>
        /// <param name="A">semi-major axis of the Europe-50 ellipsoid (in metres)</param>
        /// <param name="centerProjection">center coordinates in lat,lon (radians), height (meters) for projections</param>
        public GeoUtils(double E, double A, CoordinatesWGS84 centerProjection)
        {
            this.E2 = E * E;
            this.A = A;
            setCenterProjection(centerProjection);
        }
        /// <summary>
        /// parses a line containing latitude and longitude and returns an wgs84 object
        /// </summary>
        /// <param name="line">string line with coordinates in "[-]hh:mm:ss.ssss[NS] [-]hh:mm:ss.ssss[EW]" format</param>
        /// <param name="height">height of the point</param>
        /// <returns>height, latitude and longitude object in radians</returns>
        static public CoordinatesWGS84 LatLonStringBoth2Radians(string line, double height)
        {
            CoordinatesWGS84 res = LatLonStringBoth2Radians(line);
            res.Height = height;
            return res;
        }
        /// <summary>
        /// parses a line containing latitude and longitude and returns the numeric value
        /// </summary>
        /// <param name="line">string line with coordinates in "[-]hh:mm:ss.ssss[NS] [-]hh:mm:ss.ssss[EW] xxx.yyy" format</param>
        /// <returns>latitude and longitude in radians</returns>
        static public CoordinatesWGS84 LatLonStringBoth2Radians(string line)
        {
            string pattern = @"([-+]?)([0-9]+):([0-9]+):([0-9][0-9]*[.]*[0-9]+)([NS]?)\s+([-+]?)([0-9]+):([0-9]+):([0-9][0-9]*[.]*[0-9]+)([EW]?)[\s]*([0-9][0-9]*[.]*[0-9]+)?[.]*";
            //"40:29:58.00N 003:31:26.0W";
            //"40:29:58.00 -003:31:26.0"; // both are acceptable

            Regex reggie = new Regex(pattern);
            MatchCollection matches = reggie.Matches(line);
            string latMinus = string.Empty, lonMinus = string.Empty, latNS = string.Empty, lonEW = string.Empty;
            double lat1 = 0, lat2 = 0, lat3 = 0, lon1 = 0, lon2 = 0, lon3 = 0, height = 0;
            try
            {
                // we use the invariantInfo because our double is separated 
                // by a dot, and in other countries they use commas.
                System.Globalization.NumberFormatInfo myInv = System.Globalization.NumberFormatInfo.InvariantInfo;
                latMinus = matches[0].Groups[1].Captures[0].Value;
                lat1 = Convert.ToDouble(matches[0].Groups[2].Captures[0].Value);
                lat2 = Convert.ToDouble(matches[0].Groups[3].Captures[0].Value);
                lat3 = Convert.ToDouble(matches[0].Groups[4].Captures[0].Value, myInv);
                latNS = matches[0].Groups[5].Captures[0].Value;

                lonMinus = matches[0].Groups[6].Captures[0].Value;
                lon1 = Convert.ToDouble(matches[0].Groups[7].Captures[0].Value);
                lon2 = Convert.ToDouble(matches[0].Groups[8].Captures[0].Value);
                lon3 = Convert.ToDouble(matches[0].Groups[9].Captures[0].Value, myInv);
                lonEW = matches[0].Groups[10].Captures[0].Value;

                if (matches[0].Groups[11].Captures.Count > 0)
                    height = Convert.ToDouble(matches[0].Groups[11].Captures[0].Value, myInv);
                else
                    height = 0;
            }
            catch (Exception e)
            {
                //Program.logger.Log(NSpring.Logging.Level.Exception, "expecting line(" + line + ") to be accomply with the following regex:" + Environment.NewLine +
                //    pattern + Environment.NewLine +
                //    "more or less the following valid wgs84 strings will be correctly parsed (no negative heights):" + Environment.NewLine +
                //    "-00:20:23.98 +003:45:33 897.09m" + Environment.NewLine +
                //    "0:20:23.98S 03:45:33E 897" + Environment.NewLine +
                //    "00:20:23.98S 3:45:33E");
                //Program.logger.Log(NSpring.Logging.Level.Exception, "ERROR LatLonStringBoth2Radians(string line): " + e.ToString());
                //Environment.Exit(-1);
            }
            CoordinatesWGS84 res = new CoordinatesWGS84();
            int n = 0;
            if ((latMinus.Length > 0 && latMinus.Substring(0, 1).Equals("-")) || latNS.Equals("S"))
            {
                n = 1; if (lat1 < 0) lat1 *= -1; //quitamos el menos porque ya lo vamos a hacer con el parametro n
            }
            res.Lat = GeoUtils.LatLon2Radians(lat1, lat2, lat3, n);
            n = 0;
            if ((lonMinus.Length > 0 && lonMinus.Substring(0, 1).Equals("-")) || lonEW.Equals("W"))
            {
                n = 1; if (lon1 < 0) lon1 *= -1; //quitamos el menos porque ya lo vamos a hacer con el parametro n
            }
            res.Lon = GeoUtils.LatLon2Radians(lon1, lon2, lon3, n);
            res.Height = height;
            return res;
        }

        /// <summary>
        /// converts latitude or longitude expressed as ggºmm'ss,ss'' X to degrees dd,ddddº
        /// </summary>
        /// <param name="d1">degree</param>
        /// <param name="d2">minutes</param>
        /// <param name="d3">seconds</param>
        /// <param name="ns">North(0)/East(0) or South(1)/West(1)</param>
        /// <returns>degrees</returns>
        static public double LatLon2Degrees(double d1, double d2, double d3, int ns)
        {
            double d = d1 + (d2 / 60.0) + (d3 / 3600.0);
            if (ns == 1)
                d *= -1.0;
            return d;
        }

        /// <summary>
        /// converts latitude or longitude expressed as ggºmm'ss,ss'' X to radians dd,ddddr
        /// </summary>
        /// <param name="d1">degree</param>
        /// <param name="d2">minutes</param>
        /// <param name="d3">seconds</param>
        /// <param name="ns">North(0)/East(0) or South(1)/West(1)</param>
        /// <returns>radians</returns>
        static public double LatLon2Radians(double d1, double d2, double d3, int ns)
        {
            double d = d1 + (d2 / 60.0) + (d3 / 3600.0);
            if (ns == 1)
                d *= -1.0;
            return d * GeoUtils.DEGS2RADS;
        }

        /// <summary>
        /// converts latitude or longitude expressed as ggºmm'ss,ss'' X in a string to degrees dd,ddddº
        /// </summary>
        /// <param name="s1">degree</param>
        /// <param name="s2">minutes</param>
        /// <param name="s3">seconds</param>
        /// <param name="ns">North(0)/East(0) or South(1)/West(1)</param>
        /// <returns>degrees</returns>
        static public double LatLonString2Degrees(string s1, string s2, string s3, int ns)
        {
            double d = 0;
            try
            {
                double d1 = double.Parse(s1);
                double d2 = double.Parse(s2);
                double d3 = double.Parse(s3);
                d = GeoUtils.LatLon2Degrees(d1, d2, d3, ns);
            }
            catch (FormatException) { }
            return d;
        }

        /// <summary>
        /// converts degrees dd,ddddd to latitude or longitude expressed as ggºmm'ss,ss'' X 
        /// </summary>
        /// <param name="d">full degrees</param>
        /// <param name="d1">degree</param>
        /// <param name="d2">minutes</param>
        /// <param name="d3">seconds</param>
        /// <param name="ns">North(0)/East(0) or South(1)/West(1)</param>
        /// <returns></returns>
        static public void Degrees2LatLon(double d, out double d1, out double d2, out double d3, out int ns)
        {
            if (d < 0) { d *= -1.0; ns = 1; } else { ns = 0; }
            d1 = Math.Floor(d);
            d2 = Math.Floor((d - d1) * 60.0);
            d3 = (((d - d1) * 60.0) - d2) * 60.0;
        }

        /// <summary>
        /// converts radians dd,ddddd to latitude or longitude expressed as ggºmm'ss,ss'' X 
        /// </summary>
        /// <param name="d">full radians</param>
        /// <param name="d1">degree</param>
        /// <param name="d2">minutes</param>
        /// <param name="d3">seconds</param>
        /// <param name="ns">North(0)/East(0) or South(1)/West(1)</param>
        /// <returns></returns>
        static public void Radians2LatLon(double d, out double d1, out double d2, out double d3, out int ns)
        {
            d *= GeoUtils.RADS2DEGS;
            if (d < 0) { d *= -1.0; ns = 1; } else { ns = 0; }
            d1 = Math.Floor(d);
            d2 = Math.Floor((d - d1) * 60.0);
            d3 = (((d - d1) * 60.0) - d2) * 60.0;
        }

        /// <summary>
        /// Calculates centre of coordinates from a list of radians (lat/lon).
        /// </summary>
        /// <param name="l">Arraylist of Coordinates</param>
        /// <returns>Center of coordinates</returns>
        static public CoordinatesWGS84 CenterCoordinates(List<CoordinatesWGS84> l)
        {
            double maxLat = -999, maxLon = -999, minLat = 999, minLon = 999, maxHeight = -999;
            if (l != null && l.Count > 0)
            {
                foreach (CoordinatesWGS84 c in l)
                {
                    if (maxLat < c.Lat) maxLat = c.Lat;
                    if (maxLon < c.Lon) maxLon = c.Lon;
                    if (minLat > c.Lat) minLat = c.Lat;
                    if (minLon > c.Lon) minLon = c.Lon;
                    if (maxHeight < c.Height) maxHeight = c.Height; // wont be used for setCenterProjection
                }
                CoordinatesWGS84 res = new CoordinatesWGS84();
                res.Lat = (maxLat + minLat) / 2.0;
                res.Lon = (maxLon + minLon) / 2.0;
                res.Height = maxHeight;
                return res;
            }
            else
                return (CoordinatesWGS84)null;
        }

        /// <summary>
        /// changes the coordinates from geodesic to geocentric (lat,lon to x,y,z)
        /// </summary>
        /// <param name="c">lat,lon (radians), height (meters) coordinates</param>
        /// <returns>x,y,z coordinates</returns>
        public CoordinatesXYZ change_geodesic2geocentric(CoordinatesWGS84 c)
        {
            if (c == null) return (CoordinatesXYZ)null;
            CoordinatesXYZ res = new CoordinatesXYZ();
            double nu = this.A / Math.Sqrt(1 - this.E2 * Math.Pow(Math.Sin(c.Lat), 2.0));
            res.X = (nu + c.Height) * Math.Cos(c.Lat) * Math.Cos(c.Lon);
            res.Y = (nu + c.Height) * Math.Cos(c.Lat) * Math.Sin(c.Lon);
            res.Z = (nu * (1 - this.E2) + c.Height) * Math.Sin(c.Lat);
            return res;
        }

        /// <summary>
        /// changes the coordinates from geocentric to geodesic (x,y,z to lat,lon)
        /// </summary>
        /// <param name="c">x,y,z coordinates</param>
        /// <returns>lat,lon (radians),height (meters) coordinates</returns>
        public CoordinatesWGS84 change_geocentric2geodesic(CoordinatesXYZ c)
        {
            if (c == null) return null;
            CoordinatesWGS84 res = new CoordinatesWGS84();
            // semi-minor earth axis
            //double b = this.A * Math.Sqrt(1 - this.E2);
            double b = 6356752.3142;

            if ((Math.Abs(c.X) < GeoUtils.ALMOST_ZERO) && (Math.Abs(c.Y) < GeoUtils.ALMOST_ZERO))
            {
                if (Math.Abs(c.Z) < GeoUtils.ALMOST_ZERO)
                {
                    // the point is at the center of earth :)
                    res.Lat = Math.PI / 2.0;
                }
                else
                {
                    res.Lat = (Math.PI / 2.0) * ((c.Z / Math.Abs(c.Z)) + 0.5);
                }
                res.Lon = 0;
                res.Height = Math.Abs(c.Z) - b;
                return res;
            }

            double d_xy = Math.Sqrt(c.X * c.X + c.Y * c.Y);
            // from formula 20
            res.Lat = Math.Atan((c.Z / d_xy) /
                (1 - (this.A * this.E2) / Math.Sqrt(d_xy * d_xy + c.Z * c.Z)));
            // from formula 24
            double nu = this.A / Math.Sqrt(1 - this.E2 * Math.Pow(Math.Sin(res.Lat), 2.0));
            // from formula 20
            res.Height = (d_xy / Math.Cos(res.Lat)) - nu;

            // iteration from formula 20b
            double Lat_over;
            if (res.Lat >= 0) { Lat_over = -0.1; } else { Lat_over = 0.1; }

            int loop_count = 0;
            while ((Math.Abs(res.Lat - Lat_over) > GeoUtils.REQUIERED_PRECISION)
                && (loop_count < 50))
            {
                loop_count++;
                Lat_over = res.Lat;
                res.Lat = Math.Atan((c.Z * (1 + res.Height / nu)) /
                    (d_xy * ((1 - this.E2) + (res.Height / nu))));
                nu = this.A / Math.Sqrt(1 - this.E2 * Math.Pow(Math.Sin(res.Lat), 2.0));
                res.Height = d_xy / Math.Cos(res.Lat) - nu;
            }
            res.Lon = Math.Atan2(c.Y, c.X);
            // if (loop_count == 50) { // exception }
            return res;
        }
        /// <summary>
        /// set the center coordinates for the system projections.
        /// </summary>
        /// <param name="c">coordinate center (lat, lon (radians), h (meters))</param>
        public CoordinatesWGS84 setCenterProjection(CoordinatesWGS84 c)
        {
            if (c == null) return null;

            // we create a new instance of c2. we need to modify c, and we don't
            // want the change to be bounced back to the caller. (classes are always 
            // passed as ref)
            // we set the height = 0 because the center of our projections will be
            // the ground. this is because all the height are referred to ground (AMSL?),
            // not to the top of a mountain.
            CoordinatesWGS84 c2 = new CoordinatesWGS84(c.Lat, c.Lon, 0); //c.Height);
            this.centerProjection = c2;
            double nu = this.A / Math.Sqrt(1 - this.E2 * Math.Pow(Math.Sin(c2.Lat), 2.0));

            this.R_S = (this.A * (1.0 - this.E2)) /
                Math.Pow(1 - this.E2 * Math.Pow(Math.Sin(c2.Lat), 2.0), 1.5);

            // alternative implementation as per wikipedia article.
            // doesn't give the same result! probably doesn't work. NOT TO BE USED, NEVER!
            //R(f)^2 = ( a^4 cos(f)^2 + b^4 sin(f)^2 ) / ( a^2 cos(f)^2 + b^2 sin(f)^2 ).
            //this.R_S = Math.Sqrt((Math.Pow((this.A * this.A * Math.Cos(c2.Lat)), 2) +
            //    Math.Pow((this.B * this.B * Math.Sin(c2.Lat)), 2)) / (
            //    Math.Pow((this.A * Math.Cos(c2.Lat)), 2) +
            //    Math.Pow((this.B * Math.Sin(c2.Lat)), 2)
            //    ));

            this.T1 = GeoUtils.CalculateTranslationMatrix(c2, this.A, this.E2);
            this.R1 = GeoUtils.CalculateRotationMatrix(c2.Lat, c2.Lon);

            return this.centerProjection;
        }

        /// <summary>
        /// get the center coordinates for the system projections.
        /// </summary>
        public CoordinatesWGS84 getCenterProjection() { return this.centerProjection; }

        /// <summary>
        /// changes the coordinates from geocentric to cartesian (x,y,z to x,y,z projected)
        /// </summary>
        /// <param name="geo">x,y,z geocentric coordinates</param>
        /// <returns>x,y,z projected</returns>
        public CoordinatesXYZ change_geocentric2system_cartesian(CoordinatesXYZ geo)
        {

            if (this.centerProjection == null || this.R1 == null ||
                this.T1 == null || geo == null) return (CoordinatesXYZ)null;

            double[][] coefInput = { new double[1], new double[1], new double[1] };
            coefInput[0][0] = geo.X; coefInput[1][0] = geo.Y; coefInput[2][0] = geo.Z;
            GeneralMatrix inputMatrix = new GeneralMatrix(coefInput, 3, 1);

            inputMatrix.SubtractEquals(this.T1);
            GeneralMatrix R2 = this.R1.Multiply(inputMatrix);

            CoordinatesXYZ res = new CoordinatesXYZ(R2.GetElement(0, 0),
                                        R2.GetElement(1, 0),
                                        R2.GetElement(2, 0));
            return res;
        }


        public CoordinatesXYZ change_geodesic2system_cartesian(CoordinatesWGS84 ObjectPos, CoordinatesWGS84 RadarPos)
        {
            CoordinatesXYZ ObjectGeocentric = change_geodesic2geocentric(ObjectPos);
            //CoordinatesXYZ RadarGeocentric = change_geodesic2geocentric(RadarPos);
            //CoordinatesXYZ ObjectCentered = new CoordinatesXYZ(ObjectGeocentric.X - RadarGeocentric.X, ObjectGeocentric.Y - RadarGeocentric.Y, ObjectGeocentric.Z - RadarGeocentric.Z);
            this.setCenterProjection(RadarPos);
            CoordinatesXYZ ObjectCartesian = change_geocentric2system_cartesian(ObjectGeocentric);
            return ObjectCartesian;
        }
        /// <summary>
        /// changes the coordinates from cartesian to geocentric (x,y,z projected to x,y,z)
        /// </summary>
        /// <param name="car">x,y,z projected coordinates</param>
        /// <returns>x,y,z geocentric coordinates</returns>
        public CoordinatesXYZ change_system_cartesian2geocentric(CoordinatesXYZ car)
        {

            if (car == null) return (CoordinatesXYZ)null;

            double[][] coefInput = { new double[1], new double[1], new double[1] };
            coefInput[0][0] = car.X; coefInput[1][0] = car.Y; coefInput[2][0] = car.Z;
            GeneralMatrix inputMatrix = new GeneralMatrix(coefInput, 3, 1);

            GeneralMatrix R2 = this.R1.Transpose();
            GeneralMatrix R3 = R2.Multiply(inputMatrix);
            R3.AddEquals(this.T1);

            CoordinatesXYZ res = new CoordinatesXYZ(R3.GetElement(0, 0),
                                                    R3.GetElement(1, 0),
                                                    R3.GetElement(2, 0));
            return res;
        }


        public CoordinatesWGS84 change_system_cartesian2geodesic(CoordinatesXYZ Objectcartesian, CoordinatesWGS84 Radargeodesic)
        {
            this.setCenterProjection(Radargeodesic);
            CoordinatesXYZ Objectgeocentric = change_system_cartesian2geocentric(Objectcartesian);
            CoordinatesWGS84 Objectgeodesic = change_geocentric2geodesic(Objectgeocentric);
            return Objectgeodesic;
        }
        /// <summary>
        /// helper function that transforms H into Z
        /// </summary>
        /// <param name="c">x,y,h</param>
        /// <returns>z</returns>
        public double change_system_xyh2system_z(CoordinatesXYH c)
        {
            double z = 0.0;
            if (c == null) return 0.0;

            double xh = c.X / (this.R_S + c.Height);
            double yh = c.Y / (this.R_S + c.Height);
            double temp = xh * xh + yh * yh;
            if (temp > 1)
            {
                z = -(this.R_S + this.centerProjection.Height);
            }
            else
            {
                z = (this.R_S + c.Height) * Math.Sqrt(1.0 - temp) -
                    (this.R_S + this.centerProjection.Height);
            }
            return z;
        }
        /// <summary>
        /// changes coordinates from cartesian to stereographic(x,y,z projected to u,v,h)
        /// </summary>
        /// <param name="c">x,y,z projected coordinates</param>
        /// <returns>u,v,h stereographic projection</returns>
        public CoordinatesUVH change_system_cartesian2stereographic(CoordinatesXYZ c)
        {
            if (c == null) return (CoordinatesUVH)null;
            // don't know why we have to do this ¿?
            // double z = this.change_system_xyh2system_z(c);

            CoordinatesUVH res = new CoordinatesUVH();
            double d_xy2 = c.X * c.X + c.Y * c.Y;
            res.Height = Math.Sqrt(d_xy2 +
                (c.Z + this.centerProjection.Height + this.R_S) *
                (c.Z + this.centerProjection.Height + this.R_S)) - this.R_S;
            double k = (2 * this.R_S) /
                (2 * this.R_S + this.centerProjection.Height + c.Z + res.Height);
            res.U = k * c.X;
            res.V = k * c.Y;
            return res;
        }
        /// <summary>
        /// changes coordinates from stereographic to cartesian(u,v,h to x,y,z projected)
        /// </summary>
        /// <param name="c">u,v,h stereographic projection</param>
        /// <returns>x,y,z projected coordinates</returns>
        public CoordinatesXYZ change_stereographic2system_cartesian(CoordinatesUVH c)
        {

            if (c == null) return (CoordinatesXYZ)null;

            CoordinatesXYZ res = new CoordinatesXYZ();
            double d_uv2 = c.U * c.U + c.V * c.V;
            res.Z = (c.Height + this.R_S) * ((4 * this.R_S * this.R_S - d_uv2) /
                (4 * this.R_S * this.R_S + d_uv2)) -
                (this.R_S + this.centerProjection.Height);
            double k = (2 * this.R_S) / (2 * this.R_S + this.centerProjection.Height + res.Z + c.Height);
            res.X = c.U / k;
            res.Y = c.V / k;
            // we should not use Z because z=0 by the equations, but we need it 
            // if we're going back and fore
            // res.Z = 0;
            return res;
        }
        /// <summary>
        /// calculates elevation angle in radians
        /// </summary>
        /// <param name="centerCoordinates">center of calculations (only height in meters)</param>
        /// <param name="R">best earth radius for the centerCoordinates</param>
        /// <param name="rho">distance from center to target in meters</param>
        /// <param name="h">height of the target in meters</param>
        /// <returns>elevation angle in radians</returns>
        static public double CalculateElevation(CoordinatesWGS84 centerCoordinates, double R, double rho, double h)
        {
            if ((rho < GeoUtils.ALMOST_ZERO) || (R == -1.0) || (centerCoordinates == null))
            {
                // when rho < 0 and rho = 0 a division by zero could happen
                return 0;
            }
            else
            {
                double temp = (2 * R *
                    (h - centerCoordinates.Height) + h * h -
                    centerCoordinates.Height * centerCoordinates.Height - rho * rho) /
                    (2 * rho * (R + centerCoordinates.Height));
                if ((temp > -1.0) && (temp < 1.0))
                {
                    return Math.Asin(temp);
                }
                else
                {
                    return (Math.PI / 2.0);
                }
            }
        }
        /// <summary>
        /// calculates azimuth between two vectors in radians
        /// </summary>
        /// <param name="x">cartesian x</param>
        /// <param name="y">cartesian y</param>
        /// <returns>azimuth in radians</returns>
        static public double CalculateAzimuth(double x, double y)
        {
            double theta;
            if (Math.Abs(y) < GeoUtils.ALMOST_ZERO)
            {
                theta = (x / Math.Abs(x)) * Math.PI / 2.0;
            }
            else
            {
                theta = Math.Atan2(x, y);
            }

            if (theta < 0.0)
            {
                theta += 2 * Math.PI;
            }
            return theta;
        }
        /// <summary>
        /// Calculate best earth radius (radius of curvature in meridian)
        /// for the given geodesic coordinates (lat, lon) in radians
        /// </summary>
        /// <param name="geo">lat,lon</param>
        /// <returns>earth radius</returns>
        public double CalculateEarthRadius(CoordinatesWGS84 geo)
        {
            Double ret = Double.NaN;
            if (geo != null)
            {
                // Explanation about different radius calculations
                // http://www.gmat.unsw.edu.au/snap/gps/clynch_pdfs/radiigeo.pdf

                // Radius of curvature in Meridian
                // Matlib ARTAS &&
                // http://williams.best.vwh.net/avform.htm (local, flat earth approximation)
                // R1=a(1-e^2)/(1-e^2*(sin(lat0))^2)^(3/2)
                ret = (this.A * (1.0 - this.E2)) /
                    Math.Pow(1 - this.E2 * Math.Pow(Math.Sin(geo.Lat), 2.0), 1.5);

                // Radius of curvature in Prime Vertical
                // Double ret0 = this.A / (Math.Pow(1 - this.E2 * Math.Pow(Math.Sin(geo.Lat), 2), 0.5));

                // Matlib Transform (transform.c) from NLR 2.33
                // Double ret1 = this.A * (1.0 - (1.0/2.0) * this.E2 * Math.Cos(2.0*geo.Lat));
                
                // WIKIPEDIA     
                // http://gis.stackexchange.com/questions/20200/how-do-you-compute-the-earths-radius-at-a-given-geodetic-latitude
                //
                // R(f)^2 = ( (a^2 cos(f))^2 + (b^2 sin(f))^2 ) / ( (a cos(f))^2 + (b sin(f))^2 )
                // Double ret2 = Math.Pow(
                //    (Math.Pow(this.A * this.A * Math.Cos(geo.Lat), 2) +
                //    Math.Pow(this.B * this.B * Math.Sin(geo.Lat), 2)) /
                //    (Math.Pow(this.A * Math.Cos(geo.Lat), 2) +
                //    Math.Pow(this.B * Math.Sin(geo.Lat), 2))
                //    , 0.5);
            }

            return ret;

        }
        /// <summary>
        /// Calculates a rotation matrix to be used in several members of the GeoUtils class
        /// </summary>
        /// <param name="lat">latitude angle in radians</param>
        /// <param name="lon">longitude angle in radians</param>
        /// <returns>rotation GeneralMatrix</returns>
        static public GeneralMatrix CalculateRotationMatrix(double lat, double lon)
        {
            double[][] coefR1 = { new double[3], new double[3], new double[3] };

            coefR1[0][0] = -(Math.Sin(lon));
            coefR1[0][1] = Math.Cos(lon);
            coefR1[0][2] = 0;
            coefR1[1][0] = -(Math.Sin(lat) * Math.Cos(lon));
            coefR1[1][1] = -(Math.Sin(lat) * Math.Sin(lon));
            coefR1[1][2] = Math.Cos(lat);
            coefR1[2][0] = Math.Cos(lat) * Math.Cos(lon);
            coefR1[2][1] = Math.Cos(lat) * Math.Sin(lon);
            coefR1[2][2] = Math.Sin(lat);
            GeneralMatrix m = new GeneralMatrix(coefR1, 3, 3);
            return m;
        }
        /// <summary>
        /// calculates the translation matrix needed in several members of the GeoUtils class
        /// </summary>
        /// <param name="c">radarPosition coordiantes lat lon(rads), height(meters)</param>
        /// <param name="A">semi-major axis of earth ellipsoid (in metres)</param>
        /// <param name="E2">eccentricity of the ellipsoid squared</param>
        /// <returns>position GeneralMatrix</returns>
        static public GeneralMatrix CalculateTranslationMatrix(CoordinatesWGS84 c, double A, double E2)
        {
            double nu = A / Math.Sqrt(1 - E2 * Math.Pow(Math.Sin(c.Lat), 2.0));
            double[][] coefT1 = { new double[1], new double[1], new double[1] };
            coefT1[0][0] = (nu + c.Height) * Math.Cos(c.Lat) * Math.Cos(c.Lon);
            coefT1[1][0] = (nu + c.Height) * Math.Cos(c.Lat) * Math.Sin(c.Lon);
            coefT1[2][0] = (nu * (1 - E2) + c.Height) * Math.Sin(c.Lat);
            GeneralMatrix m = new GeneralMatrix(coefT1, 3, 1);
            return m;
        }
        /// <summary>
        /// calculates the position matrix needed in several members of the GeoUtils class
        /// </summary>
        /// <param name="T1">translation matrix to the system center</param>
        /// <param name="t">translation matrix to the radar</param>
        /// <param name="r">rotation matrix to the radar</param>
        /// <returns>position GeneralMatrix</returns>
        static public GeneralMatrix CalculatePositionRadarMatrix(GeneralMatrix T1, GeneralMatrix t, GeneralMatrix r)
        {

            GeneralMatrix R1 = T1.Subtract(t);
            GeneralMatrix res = r.Multiply(R1);

            return res;
        }
        /// <summary>
        /// calculates the rotation matrix needed in several members of the GeoUtils class
        /// </summary>
        /// <param name="R1">rotation matrix to the system center</param>
        /// <param name="r">rotation matrix to the radar</param>
        /// <returns>position GeneralMatrix</returns>
        static public GeneralMatrix CalculateRotationRadarMatrix(GeneralMatrix R1, GeneralMatrix r)
        {

            GeneralMatrix R2 = R1.Transpose();
            GeneralMatrix res = r.Multiply(R2);
            return res;
        }
        /// <summary>
        /// changes coordinates from radar spherical (rho, theta, elevation) to radar local cartesian (x,y,z) SR7
        /// </summary>
        /// <param name="polarCoordinates">rho(m), theta(radians), elevation(radians)</param>
        /// <returns>x,y,z in meters</returns>
        static public CoordinatesXYZ change_radar_spherical2radar_cartesian(CoordinatesPolar polarCoordinates)
        {
            if (polarCoordinates == null) return (CoordinatesXYZ)null;

            CoordinatesXYZ res = new CoordinatesXYZ();

            res.X = polarCoordinates.Rho * Math.Cos(polarCoordinates.Elevation) *
                Math.Sin(polarCoordinates.Theta);
            res.Y = polarCoordinates.Rho * Math.Cos(polarCoordinates.Elevation) *
                Math.Cos(polarCoordinates.Theta);
            res.Z = polarCoordinates.Rho * Math.Sin(polarCoordinates.Elevation);

            return res;
        }
        /// <summary>
        /// changes coordinates from radar local cartesian (x,y,z) to radar spherical (rho, theta, elevation)
        /// </summary>
        /// <param name="cartesianCoordinates">x,y,z (meters)</param>
        /// <returns>rho(meters), theta (radians), elevation (radians)</returns>
        static public CoordinatesPolar change_radar_cartesian2radar_spherical(CoordinatesXYZ cartesianCoordinates)
        {
            if (cartesianCoordinates == null) return (CoordinatesPolar)null;

            CoordinatesPolar res = new CoordinatesPolar();

            res.Rho = Math.Sqrt(cartesianCoordinates.X * cartesianCoordinates.X +
                cartesianCoordinates.Y * cartesianCoordinates.Y +
                cartesianCoordinates.Z * cartesianCoordinates.Z);
            res.Theta = GeoUtils.CalculateAzimuth(cartesianCoordinates.X, cartesianCoordinates.Y);
            res.Elevation = Math.Asin(cartesianCoordinates.Z / res.Rho);
            return res;
        }
        /// <summary>
        /// changes coordinates from radar local cartesian (x,y,z meters) to geocentric coordinates (x,y,z meters) SR10
        /// </summary>
        /// <param name="radarCoordinates">radar coordinates in lat lon (rads)</param>
        /// <param name="cartesianCoordinates">object with cartesian coordinates (x,y,z meters)</param>
        /// <returns>geocentric coordinates (x,y,z meters)</returns>
        public CoordinatesXYZ change_radar_cartesian2geocentric(CoordinatesWGS84 radarCoordinates, CoordinatesXYZ cartesianCoordinates)
        {
            //at_radar_local_to_geocentric
            GeneralMatrix translationMatrix = ObtainTranslationMatrix(radarCoordinates);
            GeneralMatrix rotationMatrix = ObtainRotationMatrix(radarCoordinates);

            double[][] coefInput = { new double[1], new double[1], new double[1] };
            coefInput[0][0] = cartesianCoordinates.X;
            coefInput[1][0] = cartesianCoordinates.Y;
            coefInput[2][0] = cartesianCoordinates.Z;
            GeneralMatrix inputMatrix = new GeneralMatrix(coefInput, 3, 1);

            GeneralMatrix R1 = rotationMatrix.Transpose();
            GeneralMatrix R2 = R1.Multiply(inputMatrix);
            R2.AddEquals(translationMatrix);

            CoordinatesXYZ res = new CoordinatesXYZ(R2.GetElement(0, 0),
                                    R2.GetElement(1, 0),
                                    R2.GetElement(2, 0));
            return res;

        }
        /// <summary>
        /// changes coordinates from geocentric (x,y,z meters) to radar local cartesian (x,y,z meters)
        /// </summary>
        /// <param name="radarCoordinates">radar coordinates in lat lon (rads)</param>
        /// <param name="geocentricCoordinates">object with geocentric coordinates (x,y,z meters)</param>
        /// <returns>radar local cartesian coords (x,y,z meters)centered on radar</returns>
        public CoordinatesXYZ change_geocentric2radar_cartesian(CoordinatesWGS84 radarCoordinates, CoordinatesXYZ geocentricCoordinates)
        {
            //at_radar_local_to_geocentric
            GeneralMatrix translationMatrix = ObtainTranslationMatrix(radarCoordinates);
            GeneralMatrix rotationMatrix = ObtainRotationMatrix(radarCoordinates);

            double[][] coefInput = { new double[1], new double[1], new double[1] };
            coefInput[0][0] = geocentricCoordinates.X;
            coefInput[1][0] = geocentricCoordinates.Y;
            coefInput[2][0] = geocentricCoordinates.Z;
            GeneralMatrix inputMatrix = new GeneralMatrix(coefInput, 3, 1);

            inputMatrix.SubtractEquals(translationMatrix);
            GeneralMatrix R1 = rotationMatrix.Multiply(inputMatrix);

            CoordinatesXYZ res = new CoordinatesXYZ(R1.GetElement(0, 0),
                                    R1.GetElement(1, 0),
                                    R1.GetElement(2, 0));
            return res;

        }
        /// <summary>
        /// changes coordinates from radar cartesian local (x,y,z) to system cartesian projected (x,y,z)
        /// </summary>
        /// <param name="radarCoordinates">radar coordiantes in lat, lon (rads)</param>
        /// <param name="cartesianCoordinates">object with cartesian coordinates in x,y,z (meters)</param>
        /// <returns>cartesian projected coordinates (x,y,z projected meters)</returns>
        public CoordinatesXYZ change_radar_cartesian2system_cartesian(CoordinatesWGS84 radarCoordinates, CoordinatesXYZ cartesianCoordinates)
        {
            //at_radar_local_to_system
            GeneralMatrix positionRadarMatrix = ObtainPositionRadarMatrix(radarCoordinates);
            GeneralMatrix rotationRadarMatrix = ObtainRotationRadarMatrix(radarCoordinates);

            double[][] coefInput = { new double[1], new double[1], new double[1] };
            coefInput[0][0] = cartesianCoordinates.X;
            coefInput[1][0] = cartesianCoordinates.Y;
            coefInput[2][0] = cartesianCoordinates.Z;
            GeneralMatrix inputMatrix = new GeneralMatrix(coefInput, 3, 1);

            inputMatrix.SubtractEquals(positionRadarMatrix);
            GeneralMatrix R1 = rotationRadarMatrix.Multiply(inputMatrix);

            CoordinatesXYZ res = new CoordinatesXYZ(R1.GetElement(0, 0),
                                                    R1.GetElement(1, 0),
                                                    R1.GetElement(2, 0));
            return res;
        }
        /// <summary>
        /// changes coordinates from system cartesian projected (x,y,z) to radar cartesian local (x,y,z)
        /// </summary>
        /// <param name="radarCoordinates">radar coordiantes in lat,lon (rads)</param>
        /// <param name="cartesianCoordinates">object with system cartesian projected (x,y,z projected meters)</param>
        /// <returns>radar cartesian coordinates (x,y,z meters)</returns>
        public CoordinatesXYZ change_system_cartesian2radar_cartesian(CoordinatesWGS84 radarCoordinates, CoordinatesXYZ cartesianCoordinates)
        {
            //at_system_to_radar_local
            GeneralMatrix positionRadarMatrix = ObtainPositionRadarMatrix(radarCoordinates);
            GeneralMatrix rotationRadarMatrix = ObtainRotationRadarMatrix(radarCoordinates);

            double[][] coefInput = { new double[1], new double[1], new double[1] };
            coefInput[0][0] = cartesianCoordinates.X;
            coefInput[1][0] = cartesianCoordinates.Y;
            coefInput[2][0] = cartesianCoordinates.Z;
            GeneralMatrix inputMatrix = new GeneralMatrix(coefInput, 3, 1);

            GeneralMatrix R1 = rotationRadarMatrix.Multiply(inputMatrix);
            R1.AddEquals(positionRadarMatrix);

            CoordinatesXYZ res = new CoordinatesXYZ(R1.GetElement(0, 0),
                                                    R1.GetElement(1, 0),
                                                    R1.GetElement(2, 0));
            return res;
        }
        /// <summary>
        /// builds or looksup a matrix for the transformation equations from the hashtable
        /// </summary>
        /// <returns>translation matrix</returns>
        private GeneralMatrix ObtainRotationMatrix(CoordinatesWGS84 radarCoordinates)
        {
            GeneralMatrix rotationMatrix = null;
            if (this.rotationMatrixHT == null)
                this.rotationMatrixHT = new Dictionary<CoordinatesWGS84, GeneralMatrix>(16);
            if (this.rotationMatrixHT.ContainsKey(radarCoordinates))
            {
                rotationMatrix = this.rotationMatrixHT[radarCoordinates];
            }
            else
            {
                rotationMatrix = GeoUtils.CalculateRotationMatrix(radarCoordinates.Lat, radarCoordinates.Lon);
                this.rotationMatrixHT.Add(radarCoordinates, rotationMatrix);
            }
            return rotationMatrix;
        }
        /// <summary>
        /// builds or looksup a matrix for the transformation equations from the hashtable
        /// </summary>
        /// <returns>translation matrix</returns>
        private GeneralMatrix ObtainTranslationMatrix(CoordinatesWGS84 radarCoordinates)
        {
            GeneralMatrix translationMatrix = null;
            if (this.translationMatrixHT == null)
                this.translationMatrixHT = new Dictionary<CoordinatesWGS84, GeneralMatrix>(16);
            if (this.translationMatrixHT.ContainsKey(radarCoordinates))
            {
                translationMatrix = this.translationMatrixHT[radarCoordinates];
            }
            else
            {
                translationMatrix = GeoUtils.CalculateTranslationMatrix(radarCoordinates, this.A, this.E2);
                this.translationMatrixHT.Add(radarCoordinates, translationMatrix);
            }
            return translationMatrix;
        }
        /// <summary>
        /// builds or looksup a matrix for the transformation equations from the hashtable
        /// </summary>
        /// <returns>position matrix for the radar in system coordinates</returns>
        private GeneralMatrix ObtainPositionRadarMatrix(CoordinatesWGS84 radarCoordinates)
        {
            GeneralMatrix p = null;
            lock (PositionRadarMatrixLock)
            {
                if (this.positionRadarMatrixHT == null)
                    this.positionRadarMatrixHT = new Dictionary<CoordinatesWGS84, GeneralMatrix>(16);
                if (this.positionRadarMatrixHT.ContainsKey(radarCoordinates))
                {
                    p = this.positionRadarMatrixHT[radarCoordinates];
                }
                else
                {
                    p = GeoUtils.CalculatePositionRadarMatrix(this.T1,
                        ObtainTranslationMatrix(radarCoordinates),
                        ObtainRotationMatrix(radarCoordinates));
                    this.positionRadarMatrixHT.Add(radarCoordinates, p);
                }
            }
            return p;
        }
        /// <summary>
        /// builds or looksup a matrix for the rotation equations from the hashtable
        /// </summary>
        /// <returns>position matrix for the radar in system coordinates</returns>
        private GeneralMatrix ObtainRotationRadarMatrix(CoordinatesWGS84 radarCoordinates)
        {
            GeneralMatrix p = null;
            lock (RotationRadarMatrixLock)
            {
                if (this.rotationRadarMatrixHT == null)
                    this.rotationRadarMatrixHT = new Dictionary<CoordinatesWGS84, GeneralMatrix>(16);
                if (this.rotationRadarMatrixHT.ContainsKey(radarCoordinates))
                {
                    p = this.rotationRadarMatrixHT[radarCoordinates];
                }
                else
                {
                    p = GeoUtils.CalculateRotationRadarMatrix(this.R1,
                        ObtainRotationMatrix(radarCoordinates));
                    this.rotationRadarMatrixHT.Add(radarCoordinates, p);
                }
            }
            return p;
        }
    }
    /// <summary>
    /// Helper class with Coordinates (contains definitions for Rho,Theta, Longitude,Latitude, X, Y)
    /// </summary>
    public class Coordinates
    {
    }
    /// <summary>
    /// Support for polar coordinates (rho, theta and elevation)
    /// </summary>
    public class CoordinatesPolar : Coordinates
    {
        private double rho; private double theta;
        private double elevation;
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Rho { get { return rho; } set { rho = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Theta { get { return theta; } set { theta = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Elevation { get { return elevation; } set { elevation = value; } }
        /// <summary>
        /// default constructor
        /// </summary>
        public CoordinatesPolar() { }
        /// <summary>
        /// useful constructor
        /// </summary>
        /// <param name="rho">x in meters</param>
        /// <param name="theta">theta in radians</param>
        /// <param name="elevation">elevation in meters</param>
        public CoordinatesPolar(double rho, double theta, double elevation) { this.Rho = rho; this.Theta = theta; this.Elevation = elevation; }
        /// <summary>
        /// Writes a summary of the class to a string
        /// </summary>
        /// <param name="c">class to summarize</param>
        /// <returns>string with latitude and longitude</returns>
        public static string ToString(CoordinatesPolar c)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.AppendFormat(" R: {0:f4}m T: {1:f4}rad E: {2:f4}rad", c.Rho, c.Theta, c.Elevation);
            return s.ToString();
        }
        /// <summary>
        /// Writes a summary of the class to a string, using NM in rho and degrees as theta
        /// </summary>
        /// <param name="c">class to summarize</param>
        /// <returns>string with latitude and longitude</returns>
        public static string ToStringStandard(CoordinatesPolar c)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.AppendFormat(" R: {0:f4}NM T: {1:f4}º E: {2:f4}º", c.Rho * GeoUtils.METERS2NM, c.Theta * GeoUtils.RADS2DEGS, c.Elevation * GeoUtils.RADS2DEGS);
            return s.ToString();
        }
    }
    /// <summary>
    /// support for cartesian coordinates (x y z)
    /// </summary>
    public class CoordinatesXYZ : Coordinates
    {
        private double x; private double y;
        private double z;
        /// <summary>
        /// getter and setter
        /// </summary>
        public double X { get { return x; } set { x = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Y { get { return y; } set { y = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Z { get { return z; } set { z = value; } }
        /// <summary>
        /// default constructor
        /// </summary>
        public CoordinatesXYZ() { }
        /// <summary>
        /// useful constructor
        /// </summary>
        /// <param name="x">x in meters</param>
        /// <param name="y">y in meters</param>
        /// <param name="z">z in meters</param>
        public CoordinatesXYZ(double x, double y, double z) { this.X = x; this.Y = y; this.Z = z; }
        /// <summary>
        /// Writes a summary of the class to a string
        /// </summary>
        /// <param name="c">class to summarize</param>
        /// <returns>string with x,y and z in meters</returns>
        public static string ToString(CoordinatesXYZ c)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.AppendFormat(" X: {0:f4}m Y: {1:f4}m Z: {2:f4}m", c.X, c.Y, c.Z);
            return s.ToString();
        }
        /// <summary>
        /// Writes a summary of the class to a string
        /// </summary>
        /// <param name="c">class to summarize</param>
        /// <returns>string with x, y and z in meters</returns>
        public override string ToString()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.AppendFormat(" X: {0:f4}m Y: {1:f4}m Z: {2:f4}m", this.X, this.Y, this.Z);
            return s.ToString();
        }
    }
    /// <summary>
    /// support for stereographic coordinates (u,v,h)
    /// </summary>
    public class CoordinatesUVH : Coordinates
    {
        private double u; private double v;
        private double h;
        /// <summary>
        /// getter and setter
        /// </summary>
        public double U { get { return u; } set { u = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double V { get { return v; } set { v = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Height { get { return h; } set { h = value; } }
    }
    /// <summary>
    /// support for x y height coordinates
    /// </summary>
    public class CoordinatesXYH : Coordinates
    {
        private double x; private double y;
        private double height;
        /// <summary>
        /// getter and setter
        /// </summary>
        public double X { get { return x; } set { x = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Y { get { return y; } set { y = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Height { get { return height; } set { height = value; } }
    }
    /// <summary>
    /// support for geodesic coordinates (latitude longitude height)
    /// </summary>
    public class CoordinatesWGS84 : Coordinates
    {
        private double lat; private double lon; private double height;
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Height { get { return height; } set { height = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Lat { get { return lat; } set { lat = value; } }
        /// <summary>
        /// getter and setter
        /// </summary>
        public double Lon { get { return lon; } set { lon = value; } }
        /// <summary>
        /// default constructor
        /// </summary>
        public CoordinatesWGS84() { this.lat = 0; this.lon = 0; this.height = 0; }
        /// <summary>
        /// useful constructor
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        public CoordinatesWGS84(double lat, double lon) { this.lat = lat; this.lon = lon; this.height = 0; }

        /// <summary>
        /// useful constructor
        /// </summary>
        /// <param name="lat">latitude in degrees</param>
        /// <param name="lon">longitude in degrees</param>
        /// <param name="h">height in meters</param>
        public CoordinatesWGS84(string lat, string lon, double h)
        {
            this.lat = Convert.ToDouble(lat) * GeoUtils.DEGS2RADS;
            this.lon = Convert.ToDouble(lon) * GeoUtils.DEGS2RADS;
            this.height = h;
        }

        /// <summary>
        /// useful constructor
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        /// <param name="height">height (meters)</param>
        public CoordinatesWGS84(double lat, double lon, double height) { this.lat = lat; this.lon = lon; this.height = height; }
        /// <summary>
        /// Writes a summary of the class to a string
        /// </summary>
        /// <param name="c">class to summarize</param>
        /// <returns>string with latitude and longitude</returns>
        public override string ToString()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            double d1, d2, d3; int n;
            GeoUtils.Radians2LatLon(lat, out d1, out d2, out d3, out n);
            //s.AppendFormat("{0:d2}º{1:d2}'{2:f4}" + (n == 0 ? 'N' : 'S') + " ", (int)d1, (int)d2, d3);
            s.AppendFormat("{0:d2}:{1:d2}:{2:f4}" + (n == 0 ? 'N' : 'S') + " ", (int)d1, (int)d2, d3);
            GeoUtils.Radians2LatLon(lon, out d1, out d2, out d3, out n);
            //s.AppendFormat("{0:d2}º{1:d2}'{2:f4}" + (n == 0 ? 'E' : 'W') + " ", (int)d1, (int)d2, d3);
            s.AppendFormat("{0:d3}:{1:d2}:{2:f4}" + (n == 0 ? 'E' : 'W') + " ", (int)d1, (int)d2, d3);
            s.AppendFormat("{0:f4}m", height);
            s.Append(Environment.NewLine);
            s.AppendFormat("lat:{0:f9} lon:{1:f9}", this.Lat*GeoUtils.RADS2DEGS, this.Lon*GeoUtils.RADS2DEGS);
            return s.ToString(); 
        }
    }
}
