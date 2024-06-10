using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeHistogram
{
    internal class MyImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Channel { get; set; }
        public string ImageFileName { get; set; }

        public List<CalculateBit> Bits = new List<CalculateBit>();

        public MyImage(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0,0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, bmp.PixelFormat);

            switch(bmp.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    Channel = 1; break;
                case PixelFormat.Format16bppGrayScale:
                    Channel = 2; break;
                case PixelFormat.Format24bppRgb:
                    Channel = 3; break;
                case PixelFormat.Format32bppArgb:
                    Channel = 4; break;
                default:
                    Channel = 1; break;
            }

            byte[] pixels = new byte[bmp.Width * bmp.Height * Channel];
            Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
            bmp.UnlockBits(data);

            Width = bmp.Width;
            Height = bmp.Height;

            for (int i = 0; i < Height; i++)
                Bits.Add(new CalculateBit(Width, Height));

            int pos = 0;
            for (int j = 0; j < Height; j++)
                for(int i = 0; i < Width; i++)
                    for(int ch = 0; ch < Channel; ch++)
                        Bits[ch].SetPixel(i, j, pixels[pos++]);

            bmp.Dispose();
        }
        public MyImage(int w, int h, int ch)
        {
            Channel = ch;
            Width = w;
            Height = h;
            ImageFileName = "";

            for (int i = 0; i < ch; i++)
                Bits.Add(new CalculateBit(w, h));
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp;
            switch (Channel)
            {
                case 1: 
                    bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed); break;
                case 2:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format16bppGrayScale); break;
                case 3:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb); break;
                case 4:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb); break;
                default:
                    bmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed); break;
            }
            byte[] pixels = new byte[Width * Height * Channel];

            int pos = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    for (int ch = 0; ch < Channel; ch++)
                        pixels[pos++] = Bits[ch].GetPixel(x, y);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);

            bmp.UnlockBits(data);

            return bmp;
        }
    }
}
