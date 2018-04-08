using DataStructures;
using Drawing;
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

            //List<float> distribution = AnnotatedTomogramSampler.Sample("C:/users/ben/desktop/145_painted.png",
            //    @"C:\Users\Ben\Desktop\tomography2_fullsirtcliptrim.mrc",
            //    145);
            //BinaryFormatter bf = new BinaryFormatter();
            //using (FileStream fout = File.Create("c:/users/ben/desktop/distribution.dat"))
            //{
            //    bf.Serialize(fout, distribution); 
            //}

            int counter = 0;
            //MRCFile file = MRCParser.Parse(@"D:\tomograms\tomography2_fullsirtcliptrim.mrc");
            MRCFile file = MRCParser.Parse(@"/home/brush/tomography2_fullsirtcliptrim.mrc");

            //Drawing.Tomogram.MRCFrame2Bitmap(file, 145).Save("C:/users/brush/desktop/sampleframe.bmp"); 

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

                Tomogram tom = SamplingVoronoiDiagramBuilder.BuildTomogram(100, 100, 3000,
                    rand.Next(15, 40), file, rand);
                TomogramDrawing.Tomogram2Bitmap(tom, false).Save($"/home/brush/tom4/{c}.bmp");
                TomogramDrawing.Tomogram2Bitmap(tom, true).Save($"/home/brush/tom4/{c}_labeled.bmp");
                SamplingVoronoiDiagramBuilder.SaveAsDatFile(tom, $"/home/brush/tom4/{c}.dat");

                //TomogramDrawing.Tomogram2Bitmap(tom, false).Save($"D:/tomograms/simulated/{c}.bmp");
                //TomogramDrawing.Tomogram2Bitmap(tom, true).Save($"D:/tomograms/simulated/{c}_labeled.bmp");
                //SamplingVoronoiDiagramBuilder.SaveAsDatFile(tom, $"D:/tomograms/simulated/{c}.dat");
            });
        }
    }
}
