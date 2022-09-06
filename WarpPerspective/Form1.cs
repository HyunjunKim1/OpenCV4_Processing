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

namespace WarpPerspective
{
    public partial class Form1 : Form
    {
        Mat _warp;
        public Form1()
        {
            InitializeComponent();

            _warp = BitmapConverter.ToMat(Properties.Resources.train);

            pBox_Test.Image = BitmapConverter.ToBitmap(WarpPerspectiveTest(_warp));
        }

        private Mat WarpPerspectiveTest(Mat warp)
        {
            float width = warp.Width;
            float height = warp.Height;

            Point2f[] warpPoint = new Point2f[4];
            Point2f[] rstPoint = new Point2f[4];

            warpPoint[0] = new Point2f(200f, 800.0f);
            warpPoint[1] = new Point2f(45.0f, 400.0f);
            warpPoint[2] = new Point2f(435.0f, 200.0f);
            warpPoint[3] = new Point2f(575.0f, 400.0f);

            rstPoint[0] = new Point2f(0.0f, 0.0f);
            rstPoint[1] = new Point2f(45.0f, height);
            rstPoint[2] = new Point2f(width, 0.0f);
            rstPoint[3] = new Point2f(width, height);

            Mat Matrix = Cv2.GetPerspectiveTransform(warpPoint, rstPoint);
            Cv2.WarpPerspective(warp, _warp, Matrix, warp.Size(),InterpolationFlags.Linear, BorderTypes.Default);

            return _warp;
        }
    }
}
