using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = OpenCvSharp.Point;

namespace TemplateMatching
{
    public partial class Form1 : Form
    {
        private OpenCvSharp.Point ClickPoint        = new OpenCvSharp.Point();
        private OpenCvSharp.Point CurrentTopLeft    = new OpenCvSharp.Point();
        private OpenCvSharp.Point CurrentBottomRight = new OpenCvSharp.Point();
        private Pen MyPen;
        private Graphics g;

        Mat _orgMat = new Mat();
        Mat _roiMat = new Mat();

        public Form1()
        {
            InitializeComponent();

            g = this.pictureBox1.CreateGraphics();
            MyPen = new Pen(Color.Red, 2);
            MyPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap convert = new Bitmap(Properties.Resources.test1);
            pictureBox1.Image = Properties.Resources.test1;
            _orgMat = BitmapConverter.ToMat(convert);
        }


        bool _isDraw = false;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDraw = true;
                ClickPoint = new Point(e.X, e.Y);
                pictureBox1.Refresh();
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraw)
            {
                if(e.X < ClickPoint.X)
                {
                    CurrentTopLeft.X = e.X;
                    CurrentBottomRight.X = ClickPoint.X;
                }
                else
                {
                    CurrentTopLeft.X = ClickPoint.X;
                    CurrentBottomRight.X = e.X;
                }

                if (e.Y < ClickPoint.Y)
                {
                    CurrentTopLeft.Y = e.Y;
                    CurrentBottomRight.Y = ClickPoint.Y;
                }
                else
                {
                    CurrentTopLeft.Y = ClickPoint.Y;
                    CurrentBottomRight.Y = e.Y;
                }

                pictureBox1.Refresh();
                g.DrawRectangle(MyPen, CurrentTopLeft.X, CurrentTopLeft.Y, CurrentBottomRight.X - CurrentTopLeft.X,
               CurrentBottomRight.Y - CurrentTopLeft.Y);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _isDraw = false;

            SetNewRoiTemplate();
        }


        private void btn_StartFind_Click(object sender, EventArgs e)
        {
            using (Mat resultMat = new Mat())
            {
                Cv2.MatchTemplate(_orgMat, _roiMat, resultMat, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(resultMat, resultMat, 0.6, 1.0, ThresholdTypes.Tozero);

                OpenCvSharp.Point minloc, maxloc;
                double minval, maxval;
                Cv2.MinMaxLoc(resultMat, out minval, out maxval, out minloc, out maxloc);

                Rect rect = new Rect(maxloc.X, maxloc.Y, _roiMat.Width, _roiMat.Height);
                Cv2.Rectangle(_orgMat, rect, new OpenCvSharp.Scalar(0, 0, 255), 2);

                pictureBox1.Image = BitmapConverter.ToBitmap(_orgMat);

                lbl_Result.Text = $"{maxval * 100:F2}%";

                pictureBox1.Refresh();
            }
        }

        private void SetNewRoiTemplate()
        {
            Rect rect = new Rect(CurrentTopLeft.X, CurrentTopLeft.Y, CurrentBottomRight.X - CurrentTopLeft.X, CurrentBottomRight.Y - CurrentTopLeft.Y);
            _roiMat = _orgMat.SubMat(rect);

            pictureBox1.Image = Properties.Resources.test1;
        }
       
    }

}
