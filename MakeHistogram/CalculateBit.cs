using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeHistogram
{
    public class CalculateBit
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public byte[,] PixelData { get; set; }
        public CalculateBit(CalculateBit calbit) 
        {
            this.Width = calbit.Width;
            this.Height = calbit.Height;

            for (int y = 0; y < this.Height; y++) 
            {
                for(int x = 0; x < this.Width; x++) 
                {
                    SetPixel(x, y, calbit.GetPixel(x, y));
                }
            }
        }

        public CalculateBit(int width, int height)
        {
            Width = width;
            Height = height;

            PixelData = new byte[Height, Width];
        }

        public byte GetPixel(int x, int y)
        {
            return PixelData[y, x];
        }

        public void SetPixel(int x, int y, byte value)
        {
            PixelData[y, x] = value;
        }
    }
}
