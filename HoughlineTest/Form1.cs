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

namespace HoughlineTest
{
    public partial class Form1 : Form
    {
        Mat Img;

        public Form1()
        {
            InitializeComponent();

            Img = BitmapConverter.ToMat(Properties.Resources.WAFER);
            HoughLine(Img);
        }

        private void HoughLine(Mat Org)
        {
            LineSegmentPolar[] lines;

            using (Mat InputImg = Org.Clone())
            using (Mat img = Org.Clone())
            using (Mat img_gray = new Mat())
            {
                Mat result = img;

                // 검색에 용이하도록 그레이스케일로 변환
                Cv2.CvtColor(img, img_gray, ColorConversionCodes.BGR2GRAY);

                // 캐니엣지처리 먼저 해준다.
                Cv2.Canny(img_gray, img_gray, 50, 200);

                // 마지막 파라미터 Threshold 값 조정으로 임계설정을 해준다.
                lines = Cv2.HoughLines(img_gray, 1, Math.PI / 180, 200);

                if (lines.Length <= 0)
                    return;
                
                // 찾은 라인들을 그려준다.
                for (int i = 0; i < lines.Length; i++)
                {
                    float rho = lines[i].Rho;
                    float theta = lines[i].Theta;

                    double a = Math.Cos(theta);
                    double b = Math.Sin(theta);
                    double x0 = a * rho;
                    double y0 = b * rho;

                    OpenCvSharp.Point p1 = new OpenCvSharp.Point(Math.Round(x0 + 1000 * (-b)), Math.Round(y0 + 1000 * a));
                    OpenCvSharp.Point p2 = new OpenCvSharp.Point(Math.Round(x0 - 1000 * (-b)), Math.Round(y0 - 1000 * a));

                    Cv2.Line(result, p1, p2, Scalar.Red, 1);
                }
                Cv2.ImShow("Input Image", InputImg);
                Cv2.ImShow("Hough Transform", result);

                result.Release();
                result.Dispose();
            }
        }
        private void HoughLineP(Mat Org)
        {
            List<LineSegmentPoint[]> LongestLine = new List<LineSegmentPoint[]>();
            int Longest_idx = 0;
            double Longest_max = 0;

            // OpenCV4에서 새로생긴거인데 Point1, Point2 구조체
            LineSegmentPoint[] linesP;

            using (Mat InputImg = Org.Clone())
            using (Mat img = Org.Clone())
            using (Mat img_gray = new Mat())
            {
                Mat result = img;
                Cv2.CvtColor(img, img_gray, ColorConversionCodes.BGR2GRAY);

                // 허프변환 할라고 캐니엣지 처리 먼저해줌
                Cv2.Canny(img_gray, img_gray, 50, 200);

                // ===================================================================================================================
                // 일반 허프변환(HoughLine)이 아니라 확률적 허프변환(HoughLinesP)임
                // 속성 -> HoughLinesP(이미지, 픽셀 해상도 0~1(보통 1을씀), 선회전 각도(모든각도에서의 직선검출은 PI / 180), threshold, 직선 최소길이, 선위의 점들사이 거리)
                // 일반 허프변환이랑 다른점이 <직선 최소길이>랑 <선위의 점들사이 거리>를 추가 계산하기때매 구하고자하는 선분을 두번 걸러줌 그렇기때매 연산속도도 더 빠름
                // ===================================================================================================================
                linesP = Cv2.HoughLinesP(img_gray, 1, Math.PI / 180, 100, 50, 100);

                if (linesP.Length <= 0)
                    return;

                for (int i = 0; i < linesP.Length; i++)
                {
                    double EuclideanDistance;
                    // 유클리드 거리 공식으로 계산
                    // (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)
                    EuclideanDistance = ((linesP[i].P2.X - linesP[i].P1.X) * (linesP[i].P2.X - linesP[i].P1.X)) +
                        ((linesP[i].P2.Y - linesP[i].P1.Y) * (linesP[i].P2.Y - linesP[i].P1.Y));
                    EuclideanDistance = Math.Sqrt(EuclideanDistance);

                    if (Longest_max < EuclideanDistance)
                    {
                        Longest_idx = i;
                        Longest_max = EuclideanDistance;
                    }
                }

                // 가장 긴 선만 그려줌
                Cv2.Line(result, linesP[Longest_idx].P1.X, linesP[Longest_idx].P1.Y, linesP[Longest_idx].P2.X, linesP[Longest_idx].P2.Y, Scalar.Red, 1);

                int dx = linesP[Longest_idx].P2.X - linesP[Longest_idx].P1.X;
                int dy = linesP[Longest_idx].P2.Y - linesP[Longest_idx].P1.Y;

                double rad = Math.Atan2((double)dx, (double)dy);
                double degree = Math.Abs(rad * 180) / Math.PI;

                Cv2.ImShow("Input Image", InputImg);
                Cv2.ImShow("Edge", img_gray);
                Cv2.ImShow("Hough Transform", result);

                result.Release();
                result.Dispose();
            }
        }
    }
}

