using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = OpenCvSharp.Point;

namespace GeometryComparison_HuMoment
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private (Mat, Point[][], HierarchyIndex[]) FindContours(Mat mat)
        {
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

            Cv2.Threshold(mat, mat, thresh: 87, maxval: 255, ThresholdTypes.BinaryInv);

            Cv2.FindContours(mat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            Mat edge = new Mat();
            Cv2.Canny(mat, edge, threshold1: 96, threshold2: 96);

            return (edge, contours, hierarchy);
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            Point[][] contours1;
            HierarchyIndex[] hierarchy1;

            Mat pattern;
            (pattern, contours1, hierarchy1) = FindContours(BitmapConverter.ToMat(Properties.Resources.pattern_nut));

            Cv2.ImShow("pattern", pattern);

            Point[][] contours2;
            HierarchyIndex[] hierarchy2;

            Mat data;
            (data, contours2, hierarchy2) = FindContours(BitmapConverter.ToMat(Properties.Resources.data));

            var output = new Mat();
            Cv2.CvtColor(data, output, ColorConversionCodes.GRAY2BGR);

            Cv2.ImShow("data", data);

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
                //output.Circle(centroid, radius: 3, Scalar.Yellow, thickness: -1);

                double ratio = Cv2.MatchShapes(data[rect], pattern, ShapeMatchModes.I1);
                if(ratio < 0.01)
                {
                    output.PutText(ratio.ToString("F3"), rect.TopLeft, HersheyFonts.HersheySimplex, fontScale: 0.8, Scalar.White);
                    output.Rectangle(rect, Scalar.GreenYellow);
                }
                //else
                //{
                //    output.PutText(ratio.ToString("F3"), rect.TopLeft, HersheyFonts.HersheySimplex, fontScale: 0.8, Scalar.Red);
                //    output.Rectangle(rect, Scalar.GreenYellow);
                //}

                Cv2.MinEnclosingCircle(contours2[i], out Point2f center, out float fadius);
                var diff = center - centroid;
                double distance = Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
                //output.PutText(distance.ToString("F1"), centroid + new Point(10, 0), HersheyFonts.HersheySimplex, fontScale: 0.8, Scalar.Yellow, thickness: 2);
            }

            Cv2.ImShow("output", output);
        }
    }
}
