using DataStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TomogramImageSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<int> indices = new List<int>();
            //using (Bitmap bmp = (Bitmap)Image.FromFile("C:/users/ben/desktop/template.png"))
            //{
            //    for (int y = 0, i = 0; y < bmp.Height; y++)
            //    {
            //        for (int x = 0; x < bmp.Width; x++, i++)
            //        {
            //            Color c = bmp.GetPixel(x, y);
            //            if (c.R == 255 && c.G == 255 && c.B == 255)
            //            {
            //                indices.Add(i);
            //            }
            //        }
            //    }
            //}

            //string toWrite = "mask = [" + string.Join(",", indices.ToArray()) + "]";
            //File.WriteAllText("C:/users/ben/desktop/mask.py", toWrite);

            int counter = 0;
            //for (int c = 0; c < 1000; c++)
            Parallel.For(0, 40, c =>
            {
                Random rand = new Random(c);

                lock (typeof(string))
                {
                    Console.Clear();
                    Interlocked.Increment(ref counter);
                    Console.WriteLine(counter.ToString());
                }

                Tomogram tom = SamplingVoronoiDiagramBuilder.BuildTomogram(860, 934, 100000, 
                    rand.Next(15, 40), @"/home/brush/tomography2_fullsirtcliptrim.mrc", rand);
                SamplingVoronoiDiagramBuilder.SaveAsBitmap(tom, $"/home/brush/tom3/{c}.bmp");
                SamplingVoronoiDiagramBuilder.SaveAsDatFile(tom, $"/home/brush/desktop/tom3/{c}.dat");
            });
        }
    }
}
