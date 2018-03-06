using MRCSharpLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Tester
{
    public static class EdgeTracer
    {
        class Point2D
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        class Snapshot
        {
            const int _width = 10;
            const int _height = 10;
            const int _averageLength = 1; 

            private int[] _averaged;
            public int[] Image
            {
                get
                {
                    return _averaged;
                }
            }

            public float ComputeErrorAgainstPossibleCenter(Bitmap bmp, 
                Point2D possibleNewCenter, Point2D oldCenter)
            {
                float error = 0.0f;

                int[] bufferAroundPossibleNewCenter = BufferFromCenter(bmp, possibleNewCenter);
                int[] bufferAroundOldCenter = BufferFromCenter(bmp, oldCenter); 

                for (int c = 0; c < _averaged.Length; c++)
                {
                    error += (float)Math.Pow(_averaged[c] - bufferAroundPossibleNewCenter[c], 2);
                }

                error /= (_averaged.Length * 1.0f);

                return error;
            }

            private int[] BufferFromCenter(Bitmap bmp, Point2D center)
            {
                BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, bmp.PixelFormat);

                byte[] fullBuffer = new byte[bmd.Width * bmd.Stride];
                Marshal.Copy(bmd.Scan0, fullBuffer, 0, fullBuffer.Length);

                int[] focusedBuffer = new int[_width * _height];
                for (int y = center.Y - _height / 2, dstIndex = 0; y < center.Y + _height / 2; y++)
                {
                    for (int x = center.X - _width / 2; x < center.X + _width / 2; x++, dstIndex++)
                    {
                        int srcIndex = y * _width + x;
                        focusedBuffer[dstIndex] = (fullBuffer[srcIndex]); 
                    }
                }
                bmp.UnlockBits(bmd);

                return focusedBuffer;
            }

            private List<int[]> _previousImages = null;
            public void Update(Bitmap bmp, Point2D center)
            {
                if (_previousImages == null)
                {
                    _previousImages = new List<int[]>();
                }

                if (_previousImages.Count >= _averageLength)
                {
                    _previousImages.RemoveAt(0);
                }

                int[] focusedBuffer = BufferFromCenter(bmp, center);

                _previousImages.Add(focusedBuffer);

                if (_averaged == null)
                {
                    _averaged = new int[focusedBuffer.Length];
                }

                for (int a = 0; a < _averaged.Length; a++)
                {
                    _averaged[a] = 0;
                }

                _averaged = new int[focusedBuffer.Length];
                for (int c = 0; c < _previousImages.Count; c++)
                {
                    for (int a = 0; a < _previousImages[c].Length; a++)
                    {
                        _averaged[a] += _previousImages[c][a];
                    }
                }

                for (int a = 0; a < _averaged.Length; a++)
                {
                    _averaged[a] = (byte)System.Math.Round(_averaged[a] / (float)_previousImages.Count);
                }
            }
        }

        public static void DoIt()
        {
            using (Bitmap bmp = (Bitmap)Image.FromFile("C:/users/ben/desktop/236.png"))
            {
                Point2D oldCenter = new Point2D { X = 330, Y = 365 };

                bool[] visited = new bool[bmp.Width * bmp.Height];

                int visitedIndex = oldCenter.Y * bmp.Width + oldCenter.X;
                visited[visitedIndex] = true;

                Snapshot sh = new Snapshot(); 
                sh.Update(bmp, oldCenter);

                for (int a = 0; a < 1; a++)
                {
                    float lowestError = float.MaxValue;
                    Point2D bestCenter = new Point2D();

                    for (int y1 = oldCenter.Y - 1; y1 < oldCenter.Y + 1; y1++)
                    {
                        for (int x1 = oldCenter.X - 1; x1 < oldCenter.X + 1; x1++)
                        {
                            int index = y1 * bmp.Width + x1;

                            if (!visited[index])
                            {
                                float error =
                                    sh.ComputeErrorAgainstPossibleCenter(bmp, new Point2D { X = x1, Y = y1 }, oldCenter);

                                if (error < lowestError)
                                {
                                    lowestError = error;
                                    bestCenter = new Point2D { X = x1, Y = y1 };
                                }
                            }
                        }
                    }

                    visited[bestCenter.Y * bmp.Width + bestCenter.X] = true;
                    oldCenter = bestCenter; 
                    sh.Update(bmp, bestCenter); 
                }

                for (int a = 0; a < visited.Length; a++)
                {
                    if (visited[a])
                    {
                        int y1 = a / bmp.Width;
                        int x1 = a % bmp.Width;

                        bmp.SetPixel(x1, y1, Color.Red);
                    }
                }

                bmp.Save("C:/users/ben/desktop/processed.bmp");
            }

            //const string TomogramDirectory = @"C:\Users\Ben\Desktop\tomograms";

            //Console.WriteLine("Loading main file..");
            //MRCFile file = MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.mrc"));

            //MRCFrame frame = file.Frames[236]; 
        }
    }
}
