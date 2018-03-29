﻿using DataStructures;
using MRCSharpLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace TomogramImageSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<float> distribution = AnnotatedTomogramSampler.Sample(
            //    @"C:\Users\Ben\Desktop\145_painted.png",
            //    @"C:\Users\Ben\Downloads\tomography2_fullsirtcliptrim.mrc",
            //    145);

            //return; 

            //List<float> ret = AnnotatedTomogramSampler.Sample(
            //    @"C:\Users\Ben\Desktop\tomograms\145_painted.png",
            //    @"C:\Users\Ben\Downloads\tomography2_fullsirtcliptrim.mrc",
            //    145);

            //BinaryFormatter bf = new BinaryFormatter();
            //using (FileStream fs = File.Create("C:/users/ben/desktop/distribution.dat"))
            //{
            //    bf.Serialize(fs, ret);
            //}

            //return;

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
            MRCFile file = MRCParser.Parse(@"C:\Users\Ben\Downloads\tomography2_fullsirtcliptrim.mrc");
            Parallel.For(0, 40, c =>
            //int c = 0; 
            {
                Random rand = new Random(c);

                lock (typeof(string))
                {
                    Console.Clear();
                    Interlocked.Increment(ref counter);
                    Console.WriteLine(counter.ToString());
                }

                Tomogram tom = SamplingVoronoiDiagramBuilder.BuildTomogram(860, 934, 100000,
                    rand.Next(15, 40), file, rand);
                SamplingVoronoiDiagramBuilder.SaveAsBitmap(tom, $"c:/users/ben/desktop/tom3/{c}.bmp");
                SamplingVoronoiDiagramBuilder.SaveAsDatFile(tom, $"c:/users/ben/desktop/tom3/{c}.dat");
            });
        }
    }
}
