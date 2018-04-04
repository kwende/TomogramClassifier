using DataStructures;
using MRCSharpLib;
using System;
using System.Drawing;
using System.Linq;

namespace Drawing
{
    public static class TomogramDrawing
    {
        public static Bitmap Tomogram2Bitmap(Tomogram tom, bool paintNegatives)
        {
            Bitmap bmp = new Bitmap(tom.Width, tom.Height);
            for (int y = 0, i = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++, i++)
                {
                    byte value = (byte)((tom.Data[i] +
                        System.Math.Abs(tom.MinimumTomogramValue)) * tom.MRCScaler);
                    if (value > 0)
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(value, value, value));
                    }
                    else if(paintNegatives)
                    {
                        bmp.SetPixel(x, y, Color.Red);
                    }
                }
            }
            return bmp;
        }

        public static Bitmap PaintClassifiedPixelsOnTomogram(LabeledTomogram tom, float[] labels)
        {
            tom.Labels = new float[tom.Width * tom.Height];
            Bitmap bmp = LabeledTomogram2Bitmap(tom);

            //using (Bitmap bmp = Tomogram2Bitmap(tom))
            {
                for (int y = 0, i = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++, i++)
                    {
                        if (labels[i] > 0)
                        {
                            bmp.SetPixel(x, y, Color.Red);
                        }
                    }
                }
            }

            return bmp;
        }

        public static Bitmap MRCFrame2Bitmap(MRCFile file, int frameNumber)
        {
            MRCFrame frame = file.Frames[frameNumber];

            float minFloat = file.MinPixelValue;
            float maxfloat = file.MaxPixelValue;

            float delta = maxfloat - minFloat;
            float scaler = 255 / delta;

            Bitmap bmp = new Bitmap(frame.Width, frame.Height);
            for (int y = 0, i = 0; y < frame.Height; y++)
            {
                for (int x = 0; x < frame.Width; x++, i++)
                {
                    byte b = (byte)((frame.Data[i] + Math.Abs(minFloat)) * scaler);
                    bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(b, b, b));
                }
            }

            return bmp;
        }

        public static Bitmap PaintCentersOnBitmap(Bitmap bmp, Cluster[] clusters)
        {
            Color[] Colors = new Color[]
            {
                Color.Red, Color.LightBlue, Color.Green, Color.Pink, Color.Orange, Color.Yellow, Color.Purple
            };

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int c = 0; c < clusters.Length; c++)
                {
                    g.DrawRectangle(new Pen(Colors[c], 2), new Rectangle
                    {
                        Width = 5,
                        Height = 5,
                        X = (int)clusters[c].Center.X - 2,
                        Y = (int)clusters[c].Center.Y
                    });
                }
            }


            //for (int c = 0; c < Colors.Length && c < clusters.Length; c++)
            //{
            //    Cluster cluster = clusters[c];

            //    foreach (Point2D point in cluster.Members)
            //    {
            //        bmp.SetPixel((int)point.X, (int)point.Y, Colors[c]);
            //    }
            //}

            return bmp;
        }

        public static Bitmap LabeledTomogram2Bitmap(LabeledTomogram tomogram)
        {
            float minFloat = tomogram.Data.Min();
            float maxfloat = tomogram.Data.Max();

            float delta = maxfloat - minFloat;
            float scaler = 255 / delta;

            float[] labels = tomogram.Labels;

            Bitmap bmp = new Bitmap(tomogram.Width, tomogram.Height);
            for (int y = 0, i = 0; y < tomogram.Height; y++)
            {
                for (int x = 0; x < tomogram.Width; x++, i++)
                {
                    if (labels != null && labels[i] > 0)
                    {
                        bmp.SetPixel(x, y, System.Drawing.Color.Red);
                    }
                    else
                    {
                        byte b = (byte)((tomogram.Data[i] + Math.Abs(minFloat)) * scaler);

                        bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(b, b, b));
                    }
                }
            }

            return bmp;
        }
    }
}
