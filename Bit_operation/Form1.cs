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

namespace Bit_operation
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
            Mat testMat = BitmapConverter.ToMat(Properties.Resources.testImage);
            Mat grayscale = new Mat();
            Mat result = new Mat();

            Cv2.CvtColor(testMat, grayscale, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(grayscale, grayscale, 40, 255, ThresholdTypes.Binary);

            // 기본 이미지
            Cv2.ImShow("testMat", testMat);

            // Grayscale
            Cv2.ImShow("grayscale", grayscale);

            // And 연산
            Cv2.BitwiseAnd(testMat, grayscale.CvtColor(ColorConversionCodes.GRAY2BGR), result);
            Cv2.ImShow("And", result);

            // Or 연산
            Cv2.BitwiseOr(testMat, grayscale.CvtColor(ColorConversionCodes.GRAY2BGR), result);
            Cv2.ImShow("Or", result);

            // Xor 연산
            Cv2.BitwiseXor(testMat, grayscale.CvtColor(ColorConversionCodes.GRAY2BGR), result);
            Cv2.ImShow("Xor", result);

            // Not 연산
            Cv2.BitwiseNot(testMat, result);
            Cv2.ImShow("Not", result);
        }
    }
}
