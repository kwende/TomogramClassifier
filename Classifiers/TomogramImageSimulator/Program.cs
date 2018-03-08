using DataStructures;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TomogramImageSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 0;
            for (int c = 0; c < 1000; c++)
            //Parallel.For(0, 5, c =>
            {
                Random rand = new Random(c);

                lock (typeof(string))
                {
                    Console.Clear();
                    Interlocked.Increment(ref counter);
                    Console.WriteLine(counter.ToString());
                }

                Tomogram tom = TomogramBuilder.BuildTomogram(860, 934, 100000, rand.Next(5, 25));
                TomogramBuilder.SaveAsBitmap(tom, $"C:/users/ben/desktop/toms/{c}.bmp");
                //TomogramBuilder.SaveAsDatFile(tom, $"C:/users/ben/desktop/toms/{c}.dat");
            }//);
        }
    }
}
