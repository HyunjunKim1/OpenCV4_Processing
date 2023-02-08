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

namespace ContourSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ImageProcessing();
        }

        public void ImageProcessing()
        {
            Mat src = BitmapConverter.ToMat(Properties.Resources.Image);
            Mat temp = new Mat();
            src.CopyTo(temp);
            Cv2.ImShow("scr", src);

            Mat bin = new Mat();
            Cv2.CvtColor(src, bin, ColorConversionCodes.BGR2GRAY);

            Cv2.Threshold(bin, bin, 127, 255, ThresholdTypes.Binary);

            Mat hierarchy1 = new Mat();
            Cv2.FindContours(bin, out Mat[] contour1, hierarchy1,
                RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            for (int i = 0; i < contour1.Length; i++)
            {
                Cv2.DrawContours(temp, contour1, i, Scalar.Red, 1, LineTypes.AntiAlias);
            }
            Cv2.ImShow("result", temp);
        }
    }
}
