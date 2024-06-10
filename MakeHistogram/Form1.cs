using OpenCvSharp;
using OpenCvSharp.Extensions;
using Size = OpenCvSharp.Size;
using Point = OpenCvSharp.Point;
using OpenCvSharp.WpfExtensions;
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
using ZedGraph;

namespace MakeHistogram
{
    public partial class Form1 : Form
    {
        private MyImage myImage;
        private double[][] histogram;
        private double[][] comulativeFrequencies;
        private Bitmap[] channelImages;
        public Form1()
        {
            InitializeComponent();

            Run();
        }

        private void Run()
        {

            //OpenCVCalcImageHistogram();
            CalcImageHistogram();
        }

        private void CalcImageHistogram() 
        {
            // 이미지로 데이터 얻어오기
            using(Bitmap image = new Bitmap(Properties.Resources.testImage))
            {
                try
                {
                    myImage = new MyImage(image);
                    histogram = new double[myImage.Channel][];
                    comulativeFrequencies = new double[myImage.Channel][];
                    channelImages = new Bitmap[myImage.Channel];

                    for (int ch = 0; ch < myImage.Channel; ch++)
                    {
                        histogram[ch] = HistogramRowdata(myImage.Bits[ch]);
                        channelImages[ch] = getChannelImage(myImage.Bits[ch], ch, myImage.Channel);
                    }

                    Cv2.ImShow("B", channelImages[0].ToMat());
                    Cv2.ImShow("G", channelImages[1].ToMat());
                    Cv2.ImShow("R", channelImages[2].ToMat());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            // ZedGraph를 이용해서 히스토그램 그리기
            GraphPane pane = zG_R.GraphPane;
            GraphPane pane1 = zG_G.GraphPane;
            GraphPane pane2 = zG_B.GraphPane;
            pane.CurveList.Clear();
            pane1.CurveList.Clear();
            pane2.CurveList.Clear();

            PointPairList histlistR = new PointPairList();
            PointPairList histlistG = new PointPairList();
            PointPairList histlistB = new PointPairList();

            for (int i = 0; i < histogram[2].Length; i++)
                histlistR.Add(i, histogram[2][i]);
            for (int i = 0; i < histogram[1].Length; i++)
                histlistG.Add(i, histogram[1][i]);
            for (int i = 0; i < histogram[0].Length; i++)
                histlistB.Add(i, histogram[0][i]);

            BarItem myBarR = pane.AddBar("Intensity", histlistR, Color.Red);
            BarItem myBarG = pane1.AddBar("Intensity", histlistG, Color.Green);
            BarItem myBarB = pane2.AddBar("Intensity", histlistB, Color.Blue);

            myBarR.Bar.Fill.Type = ZedGraph.FillType.Solid;
            myBarG.Bar.Fill.Type = ZedGraph.FillType.Solid;
            myBarB.Bar.Fill.Type = ZedGraph.FillType.Solid;

            pane.XAxis.Scale.Min = 0;
            pane.XAxis.Scale.Max = histogram[2].Length;
            pane.YAxis.Scale.Min = 0;
            pane.YAxis.Scale.Max = histogram[2].Max();

            pane1.XAxis.Scale.Min = 0;
            pane1.XAxis.Scale.Max = histogram[1].Length;
            pane1.YAxis.Scale.Min = 0;
            pane1.YAxis.Scale.Max = histogram[1].Max();

            pane2.XAxis.Scale.Min = 0;
            pane2.XAxis.Scale.Max = histogram[0].Length;
            pane2.YAxis.Scale.Min = 0;
            pane2.YAxis.Scale.Max = histogram[0].Max();

            zG_R.Invalidate();
            zG_G.Invalidate(); 
            zG_B.Invalidate();
        }
        public Bitmap getChannelImage(CalculateBit bits, int ch, int numCh)
        {
            MyImage myImage = new MyImage(bits.Width, bits.Height, numCh);
            myImage.Bits[ch] = bits;

            return myImage.GetBitmap();
        }
        private double[] HistogramRowdata(CalculateBit bits)
        {
            double[] histogram = new double[256];
            for(int y = 0; y < bits.Height; y++)
                for(int x = 0; x < bits.Width; x++)
                    ++histogram[bits.GetPixel(x,y)];
            return histogram;
        }
        /// <summary>
        /// 흑백은 1채널만 사용하기에 MatType을 CV_8UC1로 설정해주고, 색을 BGR2GRAY로 설정한다.
        /// 컬러는 3채널을 사용하지만 BRG을 투명도인 Alpha도 추가해주어야한다. 즉 BGR2BGRA로 설정한다.
        /// </summary>
        private void OpenCVCalcImageHistogram()
        {
            Mat src = BitmapConverter.ToMat(Properties.Resources.testImage);
            Mat color = new Mat();
            Mat histB = new Mat();
            Mat histG = new Mat();
            Mat histR = new Mat();

            // 모든요소가 1로 초기화된 행렬 생성
            Mat resultB = Mat.Ones(new Size(256, src.Height), MatType.CV_8UC3);
            Mat resultG = Mat.Ones(new Size(256, src.Height), MatType.CV_8UC3);
            Mat resultR = Mat.Ones(new Size(256, src.Height), MatType.CV_8UC3);

            // 투명도 알파도 추가되도록 Cvt
            Cv2.CvtColor(src, color, ColorConversionCodes.BGR2BGRA);

            Cv2.CalcHist(new Mat[] { color }, new int[] { 0 }, null, histB, 1, new int[] { 256 }, new Rangef[] { new Rangef(0, 256) });
            Cv2.Normalize(histB, histB, 0, 255, NormTypes.MinMax);

            Cv2.CalcHist(new Mat[] { color }, new int[] { 1 }, null, histG, 1, new int[] { 256 }, new Rangef[] { new Rangef(0, 256) });
            Cv2.Normalize(histG, histG, 0, 255, NormTypes.MinMax);

            Cv2.CalcHist(new Mat[] { color }, new int[] { 2 }, null, histR, 1, new int[] { 256 }, new Rangef[] { new Rangef(0, 256) });
            Cv2.Normalize(histR, histR, 0, 255, NormTypes.MinMax);

            for (int i = 0; i < histB.Rows; i++)
            {
                Cv2.Line(resultB, new Point(i, src.Height), new Point(i, src.Height - histB.Get<float>(i)), Scalar.Blue);
            }
            for (int i = 0; i < histG.Rows; i++)
            {
                Cv2.Line(resultG, new Point(i, src.Height), new Point(i, src.Height - histG.Get<float>(i)), Scalar.Green);
            }
            for (int i = 0; i < histR.Rows; i++)
            {
                Cv2.Line(resultR, new Point(i, src.Height), new Point(i, src.Height - histR.Get<float>(i)), Scalar.Red);
            }

            Cv2.ImShow("img", color);
            Cv2.ImShow("Blue", resultB);
            Cv2.ImShow("Green", resultG);
            Cv2.ImShow("Red", resultR);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();

        }
    }
}
