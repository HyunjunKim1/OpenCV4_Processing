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

namespace CannyEdge
{
    public partial class Form1 : Form
    {
        Mat src = BitmapConverter.ToMat(Properties.Resources.WAFER);
        public Form1()
        {
            InitializeComponent();

            CannyEdge();
        }

        private void CannyEdge()
        {
            Mat rst = new Mat();

            Cv2.Canny(
                src,    // 입력이미지
                rst,    // 출력이미지
                100,    // 하위 임계값
                200,    // 상위 임계값
                3,      // 소벨 연산 마스크 크기
                false); // L2 그래디언트

            Cv2.ImShow("Canny Edge", rst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}
