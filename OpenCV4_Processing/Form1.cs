using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Windows.Forms;

namespace OpenCV4_Processing
{

    public partial class Form1 : Form
    {
        Mat _chuckImg, _stageImg;
        Mat logo_mask;
        Mat _stageImg_cut;
        Mat Img1;
        Mat Img2;
        Mat temp;
        Mat[] Split;

        public Form1()
        {
            InitializeComponent();

            CheckMerging();
        }

        private void CheckMerging()
        {
            using(_chuckImg = new Mat())
            using(_stageImg = new Mat())
            using(logo_mask = new Mat())
            using(_stageImg_cut = new Mat())
            using(Img1 = new Mat())
            using(Img2 = new Mat())
            using(temp = new Mat())
            {
                _chuckImg = BitmapConverter.ToMat(Properties.Resources.WAFER);
                _stageImg = BitmapConverter.ToMat(Properties.Resources.STAGE);

                // 채널 분류
                Cv2.Split(_chuckImg, out Split);
                Cv2.ImShow("src", _chuckImg);
                Cv2.ImShow("Split[0]", Split[0]);     // B
                Cv2.ImShow("Split[1]", Split[1]);     // G
                Cv2.ImShow("Split[2]", Split[2]);     // R
                Cv2.ImShow("Split[3]", Split[3]);     // A

                // 합칠 이미지 배경 지우기위한 mask 
                Cv2.Threshold(Split[3], logo_mask, 240, 255, ThresholdTypes.Binary);

                // 색반전
                Cv2.BitwiseNot(logo_mask, logo_mask);

                /*
                 * 배경 이미지에서 합칠 이미지 크기만큼 잘라낼 Rect 
                 * 이후 Rect의 X,Y 위치부터 합칠 이미지 크기까지 설정
                 */
                Rect RectInfo = new Rect(new OpenCvSharp.Point(110, 540), new OpenCvSharp.Size(Split[3].Width, Split[3].Height));

                // 사각형 크기만큼 잘라냄
                _stageImg_cut = _stageImg[RectInfo];

                // Image1에 합칠 이미지 연산 후 저장
                Cv2.BitwiseAnd(_chuckImg, _chuckImg, Img1);

                // Image2에 배경 이미지 연산 후 저장
                Cv2.BitwiseAnd(_stageImg_cut, _stageImg_cut, Img2, mask: logo_mask);

                // 이미지 두개를 Temp Mat에 저장 후에 배경이미지 잘라낸 부분에 저장
                Cv2.Add(Img1, Img2, temp);
                _stageImg[RectInfo] = temp;

                // 합친 이미지 배경이미지에 저장
                pBox_Test.Image = BitmapConverter.ToBitmap(_stageImg);

                //// 이미지 메모리 해제
                //logo_mask.Release();
                //logo_mask.Dispose();
                //logo_mask = null;
                //
                //_stageImg_cut.Release();
                //_stageImg_cut.Dispose();
                //_stageImg_cut = null;
                //
                //Img1.Release();
                //Img1.Dispose();
                //Img1 = null;
                //
                //Img2.Release();
                //Img2.Dispose();
                //Img2 = null;
                //
                //temp.Release();
                //temp.Dispose();
                //temp = null;
            }
            

        }
    }
}
