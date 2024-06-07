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
using System.Windows.Forms.VisualStyles;

namespace MakeHistogram
{
    public partial class Form1 : Form
    {
        Mat[] calcGrayHist;
        Mat input;
        Mat output;
        public Form1()
        {
            InitializeComponent();

            CalcImageHistogram();
        }

        private void CalcImageHistogram()
        {
            input = new Mat();
            output = new Mat();
            calcGrayHist = new Mat[5];
            int[] channel = new int[1];
            int[] histSize = new int[256];
            Rangef[] ranges = new Rangef[256];

            Cv2.CalcHist(calcGrayHist, channel, input, output, 0, histSize, ranges);

            pictureBox1.Image = BitmapConverter.ToBitmap(output);
        }
    }
}
