using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using Size = OpenCvSharp.Size;
using Point = OpenCvSharp.Point;

namespace BlurFilters
{
    public partial class Form1 : Form
    {
        Mat src = BitmapConverter.ToMat(Properties.Resources.testImage);
        public Form1()
        {
            InitializeComponent();

            Cv2.ImShow("Origin", src);
            //Blur();
            //BoxFilter();
            //MedianFilter();
            //GaussianBlur();
            BilateralFilter();
        }

        private void Blur()
        {
            Mat blur = new Mat();
            Cv2.Blur(src, blur, new Size(9, 9), new Point(-1, -1), BorderTypes.Default);
            Cv2.ImShow("Blur", blur);
        }

        private void BoxFilter()
        {
            Mat boxfilter = new Mat();
            Cv2.BoxFilter(src, boxfilter, MatType.CV_8UC3, new Size(9,9), new Point(-1,-1), true, BorderTypes.Default);
            Cv2.ImShow("Box Filter", boxfilter);
        }

        private void MedianFilter()
        {
            Mat medianfilter = new Mat();
            Cv2.MedianBlur(src, medianfilter, 9);
            Cv2.ImShow("Median Blur", medianfilter);
        }

        private void GaussianBlur()
        {
            Mat gaussian = new Mat();
            Cv2.GaussianBlur(src, gaussian, new Size(9,9), 1, 1, BorderTypes.Default);
            Cv2.ImShow("Gaussian Blur", gaussian);
        }

        private void BilateralFilter()
        {
            Mat bilateral = new Mat();
            Cv2.BilateralFilter(src, bilateral, 9, 3, 3, BorderTypes.Default);
            Cv2.ImShow("Bilateral Filter", bilateral);
        }
    }
}
