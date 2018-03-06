using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace Tester
{
    public static class Convolution
    {
        class Filter
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int[] Data { get; set; }
        }

        private static float ComputeFilterSignal(Filter filter, byte[] data,
            int centerX, int centerY, int dataWidth)
        {
            float ret = 0.0f;
            for (int y = centerY - filter.Height / 2, i = 0; y < centerY + filter.Height/2; y++)
            {
                for (int x = centerX - filter.Width / 2; x < centerX + filter.Width/2; x++, i++)
                {
                    int value = data[y * dataWidth + x];
                    ret += filter.Data[i] * value;
                }
            }
            return ret;
        }

        private static Filter BuildFilter(int width, int height)
        {
            Filter filter = new Filter();

            filter.Width = width;
            filter.Height = height;
            filter.Data = new int[width * height];

            int centerX = width / 2;
            int centerY = height / 2;
            int radius = 0;

            if (width < height)
                radius = width/2;
            else
                radius = height/2;

            for (int y = 0, i = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, i++)
                {
                    float distance = (float)Math.Sqrt((y - centerY) * (y - centerY) + (x - centerX) * (x - centerX));

                    if (System.Math.Abs(distance - radius) <= 1)
                    {
                        filter.Data[i] = -1;
                    }
                    else
                    {
                        filter.Data[i] = 1;
                    }
                }
            }

            return filter;
        }

        public static void DoIt()
        {
            using (Bitmap bmp = (Bitmap)Image.FromFile("c:/users/ben/desktop/vesicles.bmp"))
            {
                BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, bmp.PixelFormat);

                byte[] buffer = new byte[bmd.Width * bmd.Stride];
                Marshal.Copy(bmd.Scan0, buffer, 0, buffer.Length);

                const int Size = 20; 

                Filter filter = BuildFilter(Size, Size);

                using (Bitmap heatMap = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb))
                {
                    List<float> responses = new List<float>();
                    for (int y = Size/2; y <= bmp.Height - Size / 2; y++)
                    {
                        for (int x = Size / 2; x < bmp.Width - Size / 2; x++)
                        {
                            float response = ComputeFilterSignal(filter, buffer, x, y, bmp.Width);

                            responses.Add(response);
                        }
                    }

                    float max = responses.Max();
                    float min = responses.Min();

                    float scaler = 255 / (max - min * 1.0f);

                    for (int y = Size / 2, i=0; y <= bmp.Height - Size / 2; y++)
                    {
                        for (int x = Size / 2; x < bmp.Width - Size / 2; x++, i++)
                        {
                            byte b = (byte)(responses[i] * scaler);
                            if(b < 110)
                            {
                                heatMap.SetPixel(x, y, Color.FromArgb(b, b, b));
                            }
                        }
                    }

                    heatMap.Save("C:/users/brush/desktop/fart.bmp"); 
                }
            }
        }
    }
}
