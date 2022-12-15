using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = OpenCvSharp.Point;

namespace ShapesMatching
{
    public partial class Form1 : Form
    {
        // 무조건 24bppRgb만 윤곽선을 찾을수가 있씀
        private const PixelFormat AVAILABLE_PIXEL_FORMAT = PixelFormat.Format24bppRgb;

        public Form1()
        {
            InitializeComponent();
        }
        public static Bitmap ConvertTo24bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }

        private (Mat, Point[][], HierarchyIndex[]) FindContours(Bitmap bmp)
        {
            //Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
            Mat mat = new Mat();

            //if (bmp.PixelFormat == AVAILABLE_PIXEL_FORMAT)
            //{
            //    mat = BitmapConverter.ToMat(bmp);
            //    //mat = DetectImageContour(mat);
            //}
            //else
            //{
            //    bmp = ConvertTo24bpp(bmp);
            //
            //    mat = BitmapConverter.ToMat(bmp);
            //    //mat = DetectImageContour(mat);

            //}

            mat = BitmapConverter.ToMat(bmp);
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);

            /*##################################################################################################################################################
             * 밑에꺼 함수원형임
             *                                (윤곽 찾을 이미지, 찾은 윤곽선 좌표      , 윤곽검출 계층정보             , 검색방법           , 근사화 방법)
             * public static void FindContours(InputArray image, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes mode, ContourApproximationModes method);
             * 
             * RetrievalMode 검색방법 종류
             * ｜ External : 외부 윤곽만 검출, 계층정보 x  ｜ 
             * ｜ List     : 모든 윤곽 검출, 계층정보 x    ｜ 
             * ｜ Ccomp    : 모든 윤곽 검출, 계층정보 2단계｜
             * ｜ Tree     : 모든 윤곽 검출, 모든 계층정보 ｜
             * 
             * ContourApproximationMode 근사화 방법 종류              ｜
             * ｜ ApproxNone     : 모든 윤곽점 반환                   ｜
             * ｜ ApproxSimple   : 윤곽점들 단순화, 끝점만 반환       ｜
             * ｜ ApproxTC89L1   : 프리먼 체인 코드에서의 윤곽선 적용 ｜
             * ｜ ApproxTC89KCOS : 프리먼 체인 코드에서의 윤곽선 적용 ｜ 
             * ##################################################################################################################################################*/

            Cv2.Threshold(mat, mat, thresh: 87, maxval: 255, ThresholdTypes.Binary);
            Cv2.FindContours(mat, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

            return (mat, contours, hierarchy);
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            Point[][] contours1;
            HierarchyIndex[] hierarchy1;

            Mat pattern;
            (pattern, contours1, hierarchy1) = FindContours(Properties.Resources.Pin_roi);

            Cv2.ImShow("pattern", pattern);

            Point[][] contours2;
            HierarchyIndex[] hierarchy2;

            Mat data;
            (data, contours2, hierarchy2) = FindContours(Properties.Resources.PinImage);

            var output = new Mat();
            Cv2.CvtColor(data, output, ColorConversionCodes.GRAY2BGR);

            Cv2.ImShow("data", data);

            // 지직 거리는 노이즈 날리기
            ////////////////////////////////////////////////////
            for (int i = 0; i < contours2.GetLength(0); i++)
            {
                var area = Cv2.ContourArea(contours2[i].ToArray());

                if (area < 100)
                {
                    var rect = Cv2.BoundingRect(contours2[i]);
                    rect.Inflate(width: 1, height: 1);
                    output.Rectangle(rect, Scalar.Black, thickness: -1);
                }
            }
            ///////////////////////////////////////////////////
            int min, max;
            min = max = 0;
            double ave = 0;
            List<(double lave, int idx)> ans = new List<(double lave, int idx)>();

            // 개선된 브루트포스 알고리즘 사용
            // 윤곽선의 좌표는 우하좌상으로 들어있음
            for (int i = 0; i < contours2.GetLength(0); i++)
            {
                int cnt = contours2[i].Count();

                // 찾은게 윤곽선 좌표 개수가 말도안되게 많을 경우 무시해주기
                if (cnt >= 1000)
                    continue;

                var area = Cv2.ContourArea(contours2[i].ToArray());

                // 찾은 윤곽선 면적이 말도안되게 작을 경우 무시해주기
                if (area < 500)
                    continue;

                // 왼쪽 끝 핀 위치 구하기
                for (int j = 0; j < contours2[i].Count(); j++)
                {
                    min += contours2[i][j].X;
                }
                ave = min / cnt;

                ans.Add((ave, i));
                min = 0; ave = 0;
            }

            ans.Sort();
            //var sort = ans.OrderBy(x => x.idx).ThenBy(x => x.lave).ToList();


            /*##################################################################################################################################################
             * 밑에꺼 함수원형임
             *                (중심을 찾을 Contours 배열, BinaryImage인지 아닌지);
             * Moments Moments(IEnumerable<Point> array, bool binaryImage = false);
             * 
             * <<Moment 종류>>
             * 공간 모멘트
             * ｜ m00,m01,m10｜ 
             * ｜ m11,m20,m02｜ 
             * ｜ m30,m21,m12｜
             * ｜ m03        ｜
             * 
             * 중심 모멘트
             * ｜ mu20,mu11,mu02｜
             * ｜ mu30,mu21,mu12｜
             * ｜ mu03          ｜
             * 
             * 평준화 중심 모멘트
             * ｜ nu20,nu11,nu02｜
             * ｜ nu30,nu21,nu12｜
             * ｜ nu03          ｜
             * 
             * https://en.wikipedia.org/wiki/Image_moment
             * ##################################################################################################################################################*/

            {
                var rect = Cv2.BoundingRect(contours2[ans[0].idx]);

                // Moment 찾을 Contours
                var moments = Cv2.Moments(contours2[ans[0].idx]);
                
                // Contours의 무게중심을 찾을 좌표 
                Point Center = new Point();

                /* 질량중심 Center X,Y 구하는 공식 
                 * Center X = (m10 / m00)
                 * Center Y = (m01 / m00)
                 */
                Center.X = (int)(moments.M10 / moments.M00);
                Center.Y = (int)(moments.M01 / moments.M00);

                Cv2.Circle(output, Center, 3, Scalar.Red, -1, LineTypes.AntiAlias);

                double ratio = Cv2.MatchShapes(data[rect], pattern, ShapeMatchModes.I3);
                if(ratio < 0.5)
                {
                    output.PutText(ratio.ToString("F3"), rect.TopLeft, HersheyFonts.HersheySimplex, fontScale: 0.8, Scalar.White);
                    output.Rectangle(rect, Scalar.Blue);
                }
            }

            ans.Reverse();

            // 오른쪽 Test
            {
                var rect = Cv2.BoundingRect(contours2[ans[0].idx]);

                var moments = Cv2.Moments(contours2[ans[0].idx]);
                Point Center = new Point();
                Center.X = (int)(moments.M10 / moments.M00);
                Center.Y = (int)(moments.M01 / moments.M00);
                Cv2.Circle(output, Center, 3, Scalar.Red, -1, LineTypes.AntiAlias);
                double ratio = Cv2.MatchShapes(data[rect], pattern, ShapeMatchModes.I3);
                if (ratio < 0.5)
                {
                    output.PutText(ratio.ToString("F3"), rect.TopLeft, HersheyFonts.HersheySimplex, fontScale: 0.8, Scalar.White);
                    output.Rectangle(rect, Scalar.GreenYellow);
                }
            }

            Cv2.ImShow("output", output);
        }


        private Mat DetectImageContour(Mat mat)
        {
            Bitmap targetBitmap = BitmapConverter.ToBitmap(mat);

            int targetBitmapWidth = targetBitmap.Width;
            int targetBitmapHeight = targetBitmap.Height;

            Rectangle targetRectangle = new Rectangle(0, 0, targetBitmapWidth, targetBitmapHeight);

            BitmapData targetBitmapData = targetBitmap.LockBits(targetRectangle, ImageLockMode.ReadWrite, targetBitmap.PixelFormat);

            IntPtr targetBitmapHandle = targetBitmapData.Scan0;

            int totalPixelCount = targetBitmapWidth * targetBitmapHeight;
            int totalByteCount = totalPixelCount * 3;

            int totalByteCountPerLine = targetBitmapData.Stride;
            int actualByteCountPerLine = targetBitmapWidth * 3;

            int alignmentByteCount = totalByteCountPerLine - actualByteCountPerLine;

            totalByteCount += targetBitmapHeight * alignmentByteCount;

            byte[] sourceRGBArray = new byte[totalByteCount];

            Marshal.Copy(targetBitmapHandle, sourceRGBArray, 0, totalByteCount);

            byte[,,] targetRGBArray = new byte[targetBitmapWidth, targetBitmapHeight, 3];
            float[,] targetBrightnessArray = new float[targetBitmapWidth, targetBitmapHeight];

            int sourceIndex = 0;

            for (int y = 0; y < targetBitmapHeight; y++)
            {
                for (int x = 0; x < targetBitmapWidth; x++)
                {
                    targetRGBArray[x, y, 0] = sourceRGBArray[sourceIndex + 2]; // Red
                    targetRGBArray[x, y, 1] = sourceRGBArray[sourceIndex + 1]; // Green
                    targetRGBArray[x, y, 2] = sourceRGBArray[sourceIndex + 0]; // Blue

                    targetBrightnessArray[x, y] = Color.FromArgb
                    (
                        sourceRGBArray[sourceIndex + 2],
                        sourceRGBArray[sourceIndex + 1],
                        sourceRGBArray[sourceIndex + 0]
                    ).GetBrightness();

                    sourceIndex += 3;
                }

                sourceIndex += alignmentByteCount;
            }

            float lowerLimit = 0.04f;
            float upperLimit = 0.04f;

            float maximumValue = 0;

            for (int y = 1; y < targetBitmapHeight - 1; y++)
            {
                for (int x = 1; x < targetBitmapWidth - 1; x++)
                {
                    maximumValue = Math.Abs(targetBrightnessArray[x - 1, y - 1] - targetBrightnessArray[x + 1, y + 1]);

                    if (maximumValue < Math.Abs(targetBrightnessArray[x - 1, y + 1] - targetBrightnessArray[x + 1, y - 1]))
                    {
                        maximumValue = Math.Abs(targetBrightnessArray[x - 1, y + 1] - targetBrightnessArray[x + 1, y - 1]);
                    }

                    if (maximumValue < Math.Abs(targetBrightnessArray[x, y + 1] - targetBrightnessArray[x, y - 1]))
                    {
                        maximumValue = Math.Abs(targetBrightnessArray[x, y + 1] - targetBrightnessArray[x, y - 1]);
                    }

                    if (maximumValue < Math.Abs(targetBrightnessArray[x - 1, y] - targetBrightnessArray[x + 1, y]))
                    {
                        maximumValue = Math.Abs(targetBrightnessArray[x - 1, y] - targetBrightnessArray[x + 1, y]);
                    }

                    // 색반전
                    ///////////////////////////////////////////////////
                    if (maximumValue < lowerLimit)
                    {
                        targetRGBArray[x, y, 0] = (byte)255;
                        targetRGBArray[x, y, 1] = (byte)255;
                        targetRGBArray[x, y, 2] = (byte)255;
                    }
                    else if (maximumValue > upperLimit)
                    {
                        targetRGBArray[x, y, 0] = (byte)0;
                        targetRGBArray[x, y, 1] = (byte)0;
                        targetRGBArray[x, y, 2] = (byte)0;
                    }
                    ///////////////////////////////////////////////////
                }
            }

            for (int y = 1; y < targetBitmapHeight - 1; y++)
            {
                for (int x = 1; x < targetBitmapWidth - 1; x++)
                {
                    byte brightness = (byte)((0.299 * targetRGBArray[x, y, 0]) + (0.587 * targetRGBArray[x, y, 1]) + (0.114 * targetRGBArray[x, y, 2]));
            
                    targetRGBArray[x, y, 0] = targetRGBArray[x, y, 1] = targetRGBArray[x, y, 2] = brightness;
                }
            }
        

            sourceIndex = 0;

            for (int y = 0; y < targetBitmapHeight; y++)
            {
                for (int x = 0; x < targetBitmapWidth; x++)
                {
                    sourceRGBArray[sourceIndex + 2] = targetRGBArray[x, y, 0]; // Red
                    sourceRGBArray[sourceIndex + 1] = targetRGBArray[x, y, 1]; // Green
                    sourceRGBArray[sourceIndex + 0] = targetRGBArray[x, y, 2]; // Blue

                    sourceIndex += 3;
                }

                sourceIndex += alignmentByteCount;
            }

            Marshal.Copy(sourceRGBArray, 0, targetBitmapHandle, totalByteCount);

            targetBitmap.UnlockBits(targetBitmapData);

            return BitmapConverter.ToMat(targetBitmap);
        }
    }
}
