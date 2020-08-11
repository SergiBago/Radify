using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PGTAWPF
{
    /// <summary>
    /// Class to create the map marker drawings
    /// </summary>
    public class MarkersDrawings
    {


       
        // We find the directory where the application is running to know the path to the icons that we will use
        static string directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        /// <summary>
        /// Function to insert the label and rotate the marker drawing
        /// </summary>
        /// <param name="marker">Marker from which we want to make the shape</param>
        /// <returns>Bitmap with marker drawing, rotated, and labeled</returns>
        static public Bitmap InsertText(CustomActualGmapMarker marker)
        {
            Bitmap bmp;

            //We will take a different starting image depending on whether it is a surface vehicle, plane or undetermined
            if (marker.emitter == "car")
            { 
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "ActualCarIco.png"));
            }
            else if (marker.emitter == "plane")
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "PlaneActualMarker.png"));
            }
            else
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory,"Markers", "ActualUndeterminedMarker.png"));
            }


            Bitmap returnBitmap = new Bitmap(60, 40);// We create an empty Bitmap where we will be putting the images

            Graphics g = Graphics.FromImage(returnBitmap);//Create a grapichs g from bitmap

            g.SmoothingMode = SmoothingMode.AntiAlias; //set parameters of g graphics. Interpolation and Pixel offset mode not set to highest quality to save resources
            g.InterpolationMode = InterpolationMode.High;
            g.PixelOffsetMode = PixelOffsetMode.Default;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            if (marker.emitter == "plane" || marker.emitter == "car") //if marker is from car or plane we will rotate it. Otherwise the mareker is just a dot and it's no sense to rotate it
            {
                bmp = rotateBitmap(bmp, marker.direction);
                System.Drawing.Rectangle section = new System.Drawing.Rectangle(new System.Drawing.Point(bmp.Width / 4, bmp.Height / 4), new System.Drawing.Size(bmp.Width / 2, bmp.Height / 2));
                bmp = CropImage(bmp, section); //After rotating image we crop it to quit unsused space
                g.DrawImage(bmp, 20, 15, 20, 20); //draw rotated image into g graphics
            }

            else
            {
                g.DrawImage(bmp, 27, 23, 6, 6); //draw dot image into g graphics
            }


            Font font = new Font("Times New Roman", 12, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);
            var stringSize = g.MeasureString(marker.caption, font); 
            var localPoint = new PointF((returnBitmap.Width - stringSize.Width) / 2, 0); 

            System.Drawing.Brush color = new SolidBrush(System.Drawing.Color.FromArgb(255, (byte)0, (byte)0, (byte)0)); //Set color to match radar detection color
            if (marker.DetectionMode == "SMR") { color = new SolidBrush(System.Drawing.Color.FromArgb(70, 255, 0)); }
            if (marker.DetectionMode == "MLAT") { color = new SolidBrush(System.Drawing.Color.FromArgb(0, 151, 255)); }
            if (marker.DetectionMode == "ADSB") { color = new SolidBrush(System.Drawing.Color.FromArgb(255, 151, 0)); }


            g.DrawString(marker.caption, font, color, localPoint);

            g.Flush();
            bmp.Dispose();
            bmp = null;


            g.Dispose();

            return (returnBitmap);
        }

        /// <summary>
        /// Function to insert the label and rotate the marker drawing
        /// </summary>
        /// <param name="marker">Marker from which we want to make the shape</param>
        /// <returns>Bitmap with marker drawing, rotated, and labeled</returns>
        static public Bitmap InsertTextRedImage(CustomActualGmapMarker marker)
        {
            Bitmap bmp;

            //We will take a different starting image depending on whether it is a surface vehicle, plane or undetermined
            if (marker.emitter == "car")
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "RedCarIco.png"));
            }
            else if (marker.emitter == "plane")
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "RedPlaneIco.png"));
            }
            else
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "RedUndeterminedMarker.png"));
            }

            Bitmap returnBitmap = new Bitmap(60, 40);// We create an empty Bitmap where we will be putting the images
            Graphics g = Graphics.FromImage(returnBitmap);//Create a grapichs g from bitmap

            g.SmoothingMode = SmoothingMode.AntiAlias; //set parameters of g graphics. Interpolation and Pixel offset mode not set to highest quality to save resources
            g.InterpolationMode = InterpolationMode.High;
            g.PixelOffsetMode = PixelOffsetMode.Default;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            if (marker.emitter == "plane" || marker.emitter == "car") //if marker is from car or plane we will rotate it. Otherwise the mareker is just a dot and it's no sense to rotate it
            {
                bmp = rotateBitmap(bmp, marker.direction);
                System.Drawing.Rectangle section = new System.Drawing.Rectangle(new System.Drawing.Point(bmp.Width / 4, bmp.Height / 4), new System.Drawing.Size(bmp.Width / 2, bmp.Height / 2));
                bmp = CropImage(bmp, section); //After rotating image we crop it to quit unsused space
                g.DrawImage(bmp, 20, 15, 20, 20); //draw rotated image into g graphics
            }

            else
            {
                g.DrawImage(bmp, 27, 23, 6, 6); //draw dot image into g graphics
            }

            Font font = new Font("Times New Roman", 12, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);

            var stringSize = g.MeasureString(marker.caption, font);
            var localPoint = new PointF((returnBitmap.Width - stringSize.Width) / 2, 0); 

            System.Drawing.Brush color = new SolidBrush(System.Drawing.Color.FromArgb(255, (byte)0, (byte)0, (byte)0)); //Set color to match radar detection color
            if (marker.DetectionMode == "SMR") { color = new SolidBrush(System.Drawing.Color.FromArgb(70, 255, 0)); }
            if (marker.DetectionMode == "MLAT") { color = new SolidBrush(System.Drawing.Color.FromArgb(0, 151, 255)); }
            if (marker.DetectionMode == "ADSB") { color = new SolidBrush(System.Drawing.Color.FromArgb(255, 151, 0)); }

            g.DrawString(marker.caption, font, color, localPoint); 

            g.Flush();
            bmp.Dispose();
            bmp = null;

            g.Dispose();

            return (returnBitmap);
        }



        /// <summary>
        /// Function to insert the label and rotate the marker drawing 
        /// </summary>
        /// <param name="marker">Marker from which we want to make the shape</param>
        /// <returns>Bitmap with marker drawing, rotated, and labeled</returns>
        static public Bitmap InsertText(CustomOldGmapMarker marker)
        {
            Bitmap bmp;
            try
            {
                //We will take a different starting image depending on whether it is a surface vehicle, plane or undetermined
                if (marker.emitter == "car")
                {
                    bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "OldCarIco.png"));
                }
                else if (marker.emitter == "plane")
                {
                    bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "PlaneOldMarker.png"));
                }
                else
                {
                    bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "OldUndeterminedMarker.png"));
                }
            }
            catch
            {
                bmp = null;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                if (marker.emitter == "car")//We will take a different starting image depending on whether it is a surface vehicle, plane or undetermined
                {
                    bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "OldCarIco.png"));
                }
                else if (marker.emitter == "plane")
                {
                    bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "PlaneOldMarker.png"));
                }
                else
                {
                    bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "OldUndeterminedMarker.png"));
                }
            }

            Bitmap returnBitmap = new Bitmap(60, 40); // We create an empty Bitmap where we will be putting the images
            Graphics g = Graphics.FromImage(returnBitmap); //Create a grapichs g from bitmap

            g.SmoothingMode = SmoothingMode.AntiAlias; //set parameters of g graphics. Interpolation and Pixel offset mode not set to highest quality to save resources
            g.InterpolationMode = InterpolationMode.High;
            g.PixelOffsetMode = PixelOffsetMode.Default;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            if (marker.emitter == "plane" || marker.emitter == "car")
            {
                bmp = rotateBitmap(bmp, marker.direction);
                System.Drawing.Rectangle section = new System.Drawing.Rectangle(new System.Drawing.Point(bmp.Width / 4, bmp.Height / 4), new System.Drawing.Size(bmp.Width / 2, bmp.Height / 2));
                bmp = CropImage(bmp, section); //After rotating image we crop it to quit unsused space
                g.DrawImage(bmp, 20, 15, 20, 20); //draw rotated image into g graphics
            }

            else
            {
                g.DrawImage(bmp, 27, 23, 6, 6); //draw dot image into g graphics
            }

            Font font = new Font("Times New Roman", 12, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);
            var stringSize = g.MeasureString(marker.caption, font);
            var localPoint = new PointF((returnBitmap.Width - stringSize.Width) / 2, 0);

            System.Drawing.Brush color = new SolidBrush(System.Drawing.Color.FromArgb(255, (byte)0, (byte)0, (byte)0)); //Set color to match radar detection color
            if (marker.DetectionMode == "SMR") { color = new SolidBrush(System.Drawing.Color.FromArgb(70, 255, 0)); }
            if (marker.DetectionMode == "MLAT") { color = new SolidBrush(System.Drawing.Color.FromArgb(0, 151, 255)); }
            if (marker.DetectionMode == "ADSB") { color = new SolidBrush(System.Drawing.Color.FromArgb(255, 151, 0)); }

            bmp.Dispose();
            bmp = null;

            g.DrawString(marker.caption, font, color, localPoint); 

            g.Flush();
            g.Dispose();

            return (returnBitmap);
        }




        /// <summary>
        /// Function to rotate the marker drawing
        /// </summary>
        /// <param name="marker">Marker from which we want to make the shape</param>
        /// <returns>Bitmap with marker drawing rotated</returns>
        static public Bitmap GetNoTextBitmap(CustomOldGmapMarker marker)
        {
            Bitmap bmp;

            //We will take a different starting image depending on whether it is a surface vehicle, plane or undetermined
            if (marker.emitter == "car")
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "OldCarIco.png"));
            }
            else if (marker.emitter == "plane")
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory,"Markers", "PlaneOldMarker.png"));
            }
            else
            {
                bmp = (Bitmap)Image.FromFile(System.IO.Path.Combine(directory, "Markers", "OldUndeterminedMarker.png"));
            }

            Bitmap returnBitmap = new Bitmap(30, 30);  // We create an empty Bitmap where we will be putting the images
            Graphics g = Graphics.FromImage(returnBitmap); //Create a grapichs g from bitmap

            g.SmoothingMode = SmoothingMode.AntiAlias;  //set parameters of g graphics. Interpolation and Pixel offset mode not set to highest quality to save resources
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;


            if (marker.emitter == "plane" || marker.emitter == "car")
            {
                bmp = rotateBitmap(bmp, marker.direction);
                System.Drawing.Rectangle section = new System.Drawing.Rectangle(new System.Drawing.Point(bmp.Width / 4, bmp.Height / 4), new System.Drawing.Size(bmp.Width / 2, bmp.Height / 2));
                bmp = CropImage(bmp, section); //After rotating image we crop it to quit unsused space
                g.DrawImage(bmp, 0, 0, 30, 30); //draw rotated image into g graphics
            }

            else
            {
                g.DrawImage(bmp, 10, 10, 10, 10); //draw dot image into g graphics
            }

            g.Flush();
            g.Dispose();

            return (returnBitmap);

        }

        /// <summary>
        /// Create the label of the measurement lines
        /// </summary>
        /// <param name="marker">Measurement line label marker</param>
        /// <returns>Bitmap with the label</returns>
        static public Bitmap InsertText(LinesLabel marker)
        {
            Bitmap returnBitmap = new Bitmap(120, 10); // We create an empty Bitmap where we will put the text
            Graphics g = Graphics.FromImage(returnBitmap);//Create a grapichs g from bitmap

            g.SmoothingMode = SmoothingMode.AntiAlias; //set parameters of g graphics. Interpolation and Pixel offset mode not set to highest quality to save resources
            g.InterpolationMode = InterpolationMode.High;
            g.PixelOffsetMode = PixelOffsetMode.Default;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            Font font = new Font("Arial", 11, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);
            var stringSize = g.MeasureString(marker.caption, font);
            var localPoint = new PointF((returnBitmap.Width - stringSize.Width) / 2, (returnBitmap.Height - stringSize.Height) / 2); //

            System.Drawing.Brush color = new SolidBrush(System.Drawing.Color.FromArgb(255, (byte)0, (byte)0, (byte)0));

            g.DrawString(marker.caption, font, color, localPoint); /// locarPoint cambiado por rectf

            g.Flush();

            g.Dispose();

            return (returnBitmap);
        }


        /// <summary>
        /// Function to rotate the Bitmap
        /// </summary>
        /// <param name="b">Bitmap to rotate</param>
        /// <param name="angle">Angle to rotate</param>
        /// <returns>rotated bitmap</returns>
        static private Bitmap rotateBitmap(Bitmap b, int angle)
        {
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            Graphics g = Graphics.FromImage(returnBitmap);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            g.DrawImage(b, 0, 0, b.Width, b.Height);
            g.Dispose();
            return returnBitmap;
        }

        /// <summary>
        /// Function to crop a bitmap
        /// </summary>
        /// <param name="source">bitmap to crop</param>
        /// <param name="section">section to crop</param>
        /// <returns>Croped bitmap</returns>
        static public Bitmap CropImage(Bitmap source, System.Drawing.Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                g.Dispose();
                return bitmap;
            }
        }

        /// <summary>
        /// Function to get a BitmapImage from a bitmap
        /// </summary>
        /// <param name="bitmap"> input bitmap </param>
        /// <returns>output bitmap image</returns>
        static public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            MemoryStream memory;
            BitmapImage bitmapImage;
            using (memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                bitmapImage.StreamSource = memory;

                bitmapImage.EndInit();
                memory.Close();
                memory.Dispose();
                memory = null;
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

    }
}
