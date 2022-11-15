using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = OpenCvSharp.Point;

namespace ShapesMatching
{
    public partial class Form1 : Form
    {
        // 무조건 24bppRgb만 윤곽선을 찾을수가 있씀
        private const PixelFormat AVAILABLE_PIXEL_FORMAT = PixelFormat.Format24bppRgb;

        public Form1()
        {
            InitializeComponent();
        }
        public static Bitmap ConvertTo24bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }

        private (Mat, Point[][], HierarchyIndex[]) FindContours(Bitmap bmp)
        {
            //Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
            Mat mat = new Mat();

            if (bmp.PixelFormat == AVAILABLE_PIXEL_FORMAT)
            {
                mat = BitmapConverter.ToMat(bmp);
                mat = DetectImageContour(mat);
            }
            else
            {
                bmp = ConvertTo24bpp(bmp);

                mat = BitmapConverter.ToMat(bmp);
                mat = DetectImageContour(mat);
            }

            //Cv2.Threshold(mat, mat, thresh: 87, maxval: 255, ThresholdTypes.BinaryInv);

            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

            Cv2.FindContours(mat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            return (mat, contours, hierarchy);
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            Point[][] contours1;
            HierarchyIndex[] hierarchy1;

            Mat pattern;
            (pattern, contours1, hierarchy1) = FindContours(Properties.Resources.pattern_nut);

            Cv2.ImShow("pattern", pattern);

            Point[][] contours2;
            HierarchyIndex[] hierarchy2;

            Mat data;
            (data, contours2, hierarchy2) = FindContours(Properties.Resources.data);

            var output = new Mat();
            Cv2.CvtColor(data, output, ColorConversionCodes.GRAY2BGR);

            Cv2.ImShow("data", data);

            // 지직 거리는 노이즈 날리기
            ////////////////////////////////////////////////////
            for (int i = 0; i < contours2.GetLength(0); ++i)
            {
                var area = Cv2.ContourArea(contours2[i].ToArray());

                if (area < 100)
                {
                    var rect = Cv2.BoundingRect(contours2[i]);
                    rect.Inflate(width: 1, height: 1);
                    output.Rectangle(rect, Scalar.Black, thickness: -1);
                }
            }
            ///////////////////////////////////////////////////

            for (int i = 0; i < contours2.GetLength(0); ++i)
            {
                var area = Cv2.ContourArea(contours2[i].ToArray());

                if (area < 100)
                {
                    continue;
                }

                var rect = Cv2.BoundingRect(contours2[i]);

                var moments = Cv2.Moments(contours2[i]);
                Point centroid = new Point();
                centroid.X = (int)(moments.M10 / moments.M00);
                centroid.Y = (int)(moments.M01 / moments.M00);

                double ratio = Cv2.MatchShapes(data[rect], pattern, ShapeMatchModes.I1);
                if(ratio < 0.05)
                {
                    output.PutText(ratio.ToString("F3"), rect.TopLeft, HersheyFonts.HersheySimplex, fontScale: 0.8, Scalar.White);
                    output.Rectangle(rect, Scalar.GreenYellow);
                }
            }

            Cv2.ImShow("output", output);
        }
        private Mat DetectImageContour(Mat mat)
        {
            Bitmap targetBitmap = BitmapConverter.ToBitmap(mat);

            int targetBitmapWidth = targetBitmap.Width;
            int targetBitmapHeight = targetBitmap.Height;

            Rectangle targetRectangle = new Rectangle(0, 0, targetBitmapWidth, targetBitmapHeight);

            BitmapData targetBitmapData = targetBitmap.LockBits(targetRectangle, ImageLockMode.ReadWrite, targetBitmap.PixelFormat);

            IntPtr targetBitmapHandle = targetBitmapData.Scan0;

            int totalPixelCount = targetBitmapWidth * targetBitmapHeight;
            int totalByteCount = totalPixelCount * 3;

            int totalByteCountPerLine = targetBitmapData.Stride;
            int actualByteCountPerLine = targetBitmapWidth * 3;

            int alignmentByteCount = totalByteCountPerLine - actualByteCountPerLine;

            totalByteCount += targetBitmapHeight * alignmentByteCount;

            byte[] sourceRGBArray = new byte[totalByteCount];

            Marshal.Copy(targetBitmapHandle, sourceRGBArray, 0, totalByteCount);

            byte[,,] targetRGBArray = new byte[targetBitmapWidth, targetBitmapHeight, 3];
            float[,] targetBrightnessArray = new float[targetBitmapWidth, targetBitmapHeight];

            int sourceIndex = 0;

            for (int y = 0; y < targetBitmapHeight; y++)
            {
                for (int x = 0; x < targetBitmapWidth; x++)
                {
                    targetRGBArray[x, y, 0] = sourceRGBArray[sourceIndex + 2]; // Red
                    targetRGBArray[x, y, 1] = sourceRGBArray[sourceIndex + 1]; // Green
                    targetRGBArray[x, y, 2] = sourceRGBArray[sourceIndex + 0]; // Blue

                    targetBrightnessArray[x, y] = Color.FromArgb
                    (
                        sourceRGBArray[sourceIndex + 2],
                        sourceRGBArray[sourceIndex + 1],
                        sourceRGBArray[sourceIndex + 0]
                    ).GetBrightness();

                    sourceIndex += 3;
                }

                sourceIndex += alignmentByteCount;
            }

            float lowerLimit = 0.04f;
            float upperLimit = 0.04f;

            float maximumValue = 0;

            for (int y = 1; y < targetBitmapHeight - 1; y++)
            {
                for (int x = 1; x < targetBitmapWidth - 1; x++)
                {
                    maximumValue = Math.Abs(targetBrightnessArray[x - 1, y - 1] - targetBrightnessArray[x + 1, y + 1]);

                    if (maximumValue < Math.Abs(targetBrightnessArray[x - 1, y + 1] - targetBrightnessArray[x + 1, y - 1]))
                    {
                        maximumValue = Math.Abs(targetBrightnessArray[x - 1, y + 1] - targetBrightnessArray[x + 1, y - 1]);
                    }

                    if (maximumValue < Math.Abs(targetBrightnessArray[x, y + 1] - targetBrightnessArray[x, y - 1]))
                    {
                        maximumValue = Math.Abs(targetBrightnessArray[x, y + 1] - targetBrightnessArray[x, y - 1]);
                    }

                    if (maximumValue < Math.Abs(targetBrightnessArray[x - 1, y] - targetBrightnessArray[x + 1, y]))
                    {
                        maximumValue = Math.Abs(targetBrightnessArray[x - 1, y] - targetBrightnessArray[x + 1, y]);
                    }

                    // 색반전
                    ///////////////////////////////////////////////////
                    if (maximumValue < lowerLimit)
                    {
                        targetRGBArray[x, y, 0] = (byte)255;
                        targetRGBArray[x, y, 1] = (byte)255;
                        targetRGBArray[x, y, 2] = (byte)255;
                    }
                    else if (maximumValue > upperLimit)
                    {
                        targetRGBArray[x, y, 0] = (byte)0;
                        targetRGBArray[x, y, 1] = (byte)0;
                        targetRGBArray[x, y, 2] = (byte)0;
                    }
                    ///////////////////////////////////////////////////
                }
            }

            for (int y = 1; y < targetBitmapHeight - 1; y++)
            {
                for (int x = 1; x < targetBitmapWidth - 1; x++)
                {
                    byte brightness = (byte)((0.299 * targetRGBArray[x, y, 0]) + (0.587 * targetRGBArray[x, y, 1]) + (0.114 * targetRGBArray[x, y, 2]));
            
                    targetRGBArray[x, y, 0] = targetRGBArray[x, y, 1] = targetRGBArray[x, y, 2] = brightness;
                }
            }
        

            sourceIndex = 0;

            for (int y = 0; y < targetBitmapHeight; y++)
            {
                for (int x = 0; x < targetBitmapWidth; x++)
                {
                    sourceRGBArray[sourceIndex + 2] = targetRGBArray[x, y, 0]; // Red
                    sourceRGBArray[sourceIndex + 1] = targetRGBArray[x, y, 1]; // Green
                    sourceRGBArray[sourceIndex + 0] = targetRGBArray[x, y, 2]; // Blue

                    sourceIndex += 3;
                }

                sourceIndex += alignmentByteCount;
            }

            Marshal.Copy(sourceRGBArray, 0, targetBitmapHandle, totalByteCount);

            targetBitmap.UnlockBits(targetBitmapData);

            return BitmapConverter.ToMat(targetBitmap);
        }
    }
}
