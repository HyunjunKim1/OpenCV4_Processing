using OpenCvSharp.Extensions;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Size = OpenCvSharp.Size;
using Point = OpenCvSharp.Point;

namespace CLAHE
{
    public partial class Form1 : Form
    {
        Mat src = BitmapConverter.ToMat(Properties.Resources.test);

        public Form1()
        {
            InitializeComponent();

            //Run();
            Clahe();
        }

        /// <summary>
        /// CLAHE 적용 순서
        /// 1. Histogram 생성 
        /// 2. 해당 Histogram 정규화. Alpha 구간과 beta 구간으로 정규화 (이미지 화질개선)
        /// 3. 정규화는 Histogram 분포가 집중되어있는 경우 효과적이지만 아닐 경우 평탄화가 필요.
        /// 4. 평탄화 후 CLAHE 작업. (Contrast Limited Adaptive Histogram Equalization)
        /// </summary>
        
        private void Clahe()
        {
            Mat gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat dst1 = new Mat();
            Mat dst2 = new Mat();
            Mat dst3 = new Mat();
            Mat dst4 = new Mat();
            Mat dst5 = new Mat();
            Mat dst6 = new Mat();
            using (var clahe = Cv2.CreateCLAHE())
            {
                clahe.ClipLimit = 2.0;
                clahe.Apply(gray, dst1);
                clahe.ClipLimit = 8.0;
                clahe.Apply(gray, dst2);
                clahe.ClipLimit = 14.0;
                clahe.Apply(gray, dst3);


                clahe.ClipLimit = 4.5;
                clahe.TilesGridSize = new Size(4, 4);
                clahe.Apply(gray, dst4);

                clahe.ClipLimit = 4.5;
                clahe.TilesGridSize = new Size(8, 8);
                clahe.Apply(gray, dst5);

                clahe.ClipLimit = 4.5;
                clahe.TilesGridSize = new Size(12, 12);
                clahe.Apply(gray, dst6);
            }

            using (new Window("gray",  gray))
            using (new Window("clip2.0", dst1))
            using (new Window("clip8.0",  dst2))
            using (new Window("clip14.0", dst3))
            using (new Window("tile4x4", dst4))
            using (new Window("tile8x8",  dst5))
            using (new Window("tile12x12", dst6))
            {
                Cv2.WaitKey();
                Cv2.DestroyAllWindows();
            }
        }

        private void Run()
        {
            // gray scale
            Mat gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);

            // Histogram View
            const int Width = 260;
            const int Height = 200;

            Mat render = new Mat(new Size(Width, Height), MatType.CV_8UC3, Scalar.All(255));

            // Calculate histogram
            Mat hist = new Mat();
            
            // histogram size for each dimension
            int[] hdims = { 256 };
            Rangef[] ranges = { new Rangef(0, 256), };

            Cv2.CalcHist(
                new Mat[] { gray }, // Mat 이미지들
                new int[] { 0 },    // 채널. Grayscale = 0, R,G,B = 0,1,2
                null,               // Mask : all area = null
                hist,               // output
                1,                  // dims
                hdims,              // hist size
                ranges);            // range

            // Get the max value of histogram
            double minVal, maxVal;
            Cv2.MinMaxLoc(hist, out minVal, out maxVal);

            Scalar color = Scalar.All(100);

            // Scales and draws histogram
            hist = hist * (maxVal != 0 ? Height / maxVal : 0.0);
            for (int j = 0; j < hdims[0]; ++j)
            {
                int binW = (int)((double)Width / hdims[0]);
                render.Rectangle(
                    new Point(j * binW, render.Rows - (int)hist.Get<float>(j)),
                    new Point((j + 1) * binW, render.Rows),
                    color,
                    -1);
            }

            using (new Window("Image",  gray))
            using (new Window("Histogram", render))
            {
                Cv2.WaitKey();
                Cv2.DestroyAllWindows();
            }
        }
    }
}
    