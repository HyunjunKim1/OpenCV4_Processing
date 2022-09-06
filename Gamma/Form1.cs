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

namespace Gamma
{
    public partial class Form1 : Form
    {
        Mat _gamma;
        public Form1()
        {
            InitializeComponent();

            _gamma = BitmapConverter.ToMat(Properties.Resources.test);

            pBox_Test.Image = BitmapConverter.ToBitmap(GammaTest(_gamma));
        }

        public Mat GammaTest(Mat src)
        {
            _gamma = new Mat();

            double GammaValue = 2;

            byte[] lut = new byte[256];

            for (int i = 0; i < lut.Length; i++)
                lut[i] = (byte)(Math.Pow(i / 255.0, 1.0 / GammaValue) * 255.0);

            Cv2.LUT(src, lut, _gamma);

            return _gamma;
        }
    }
}
