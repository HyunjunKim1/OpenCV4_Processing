using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoveBackground
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = ImageProcessing(Properties.Resources.bowling).ToBitmap();
        }

        private Mat ImageProcessing(Bitmap bmp)
        {
            Mat bowling = new Mat();

            Cv2.ImShow("Before", Properties.Resources.bowling.ToMat());

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            
            Color removeTargetColor = Color.Black;
            int changeColor = Color.Transparent.ToArgb();
            double threshold = 100;
            
            unsafe
            {
                int* length = (int*)data.Scan0 + bmp.Height * bmp.Width;
                for (int* p = (int*)data.Scan0; p < length; p++)
                {
                    Color pixelColor = Color.FromArgb(*p);
            
                    double r = Math.Pow((double)(pixelColor.R - removeTargetColor.R), 2.0);
                    double g = Math.Pow((double)(pixelColor.G - removeTargetColor.G), 2.0);
                    double b = Math.Pow((double)(pixelColor.B - removeTargetColor.B), 2.0);
                    double delta = Math.Sqrt(r + g + b);
            
                    if (delta <= threshold)
                        *p = changeColor;
                }

                bmp.UnlockBits(data);
                bowling = bmp.ToMat();
                bmp.Dispose();
            }
            

            return bowling;
        }
    }
}
