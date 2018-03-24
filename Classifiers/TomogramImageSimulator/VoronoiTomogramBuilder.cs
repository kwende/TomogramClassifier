using DataStructures;
using Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TomogramImageSimulator
{
    public static class VoronoiTomogramBuilder
    {
        public static void SaveAsDatFile(Tomogram tomogram, string path)
        {
            using (FileStream fs = File.Create(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tomogram);
            }
        }

        public static void SaveAsBitmap(Tomogram tomogram, string path)
        {
            //Color[] colors = new Color[tomogram.BackgroundDensity];

            //for (int c = 0; c < colors.Length; c++)
            //{
            //    byte b = (byte)RandomNormal.Next(tomogram.Random, 85, 15);
            //    colors[c] = Color.FromArgb(b, b, b);
            //}

            using (Bitmap bmp = new Bitmap(tomogram.Width, tomogram.Height))
            {
                for (int y = 0, i = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++, i++)
                    {
                        byte value = (byte)(tomogram.Data[i] * (1 / tomogram.MRCScaler));
                        if (value > 0)
                        {
                            bmp.SetPixel(x, y, Color.FromArgb(value, value, value));
                        }
                    }
                }

                //for (int y = 0, i = 0; y < bmp.Height; y++)
                //{
                //    for (int x = 0; x < bmp.Width; x++, i++)
                //    {
                //        int colorIndex = tomogram.Data[i];
                //        if (colorIndex == 0)
                //        {
                //            byte b = (byte)tomogram.Random.Next(50, 60);
                //            bmp.SetPixel(x, y, Color.FromArgb(b, b, b));
                //        }
                //    }
                //}

                bmp.Save(path);
            }
        }

        public static Tomogram BuildTomogram(int width, int height,
            int backgroundDensity, int vesicleCount)
        {
            Tomogram ret = new Tomogram();
            ret.Width = width;
            ret.Height = height;
            ret.BackgroundDensity = backgroundDensity;
            ret.VesicleCount = vesicleCount;
            ret.Random = new Random();
            ret.MinimumVesicleRadius = 20;
            ret.MaximumVesicleRadius = 20;
            ret.MRCScaler = 0.00565638626f;
            ret.Data = new float[width * height];
            ret.DataClasses = new int[width * height];

            BuildBackground(ret);
            AddVesicles(ret);

            FinalizeFrame(ret);

            return ret;
        }

        private static void FinalizeFrame(Tomogram tom)
        {
            int numberOfBackgroundClasses = tom.DataClasses.Where(n => n != 0).Count();

            float[] classKey = new float[numberOfBackgroundClasses];
            for (int c = 0; c < classKey.Length; c++)
            {
                float v = (float)RandomNormal.Next(tom.Random, 85, 15);
                classKey[c] = v * tom.MRCScaler;
            }

            for (int y = 0, i = 0; y < tom.Height; y++)
            {
                for (int x = 0; x < tom.Width; x++, i++)
                {
                    int classNumber = tom.DataClasses[i];
                    if (classNumber > 0 && classNumber <= tom.BackgroundDensity)
                    {
                        tom.Data[i] = classKey[classNumber];
                    }
                }
            }

            GaussianBlur blur = GaussianBlur.BuildBlur(2.0f, 4);
            tom.Data = blur.BlurData(tom.Data, tom.Width, tom.Height);

            for (int y = 0, i = 0; y < tom.Height; y++)
            {
                for (int x = 0; x < tom.Width; x++, i++)
                {
                    int classNumber = tom.DataClasses[i];
                    if (classNumber == -1)
                    {
                        float v = tom.Random.Next(50, 60);
                        tom.Data[i] = v * tom.MRCScaler;
                    }
                }
            }

            tom.Data = blur.BlurData(tom.Data, tom.Width, tom.Height);
        }

        private static void AddVesicles(Tomogram tom)
        {
            List<Vesicle> vesicles = new List<Vesicle>();
            for (int t = 0; t < tom.VesicleCount; t++)
            {
                // pick a random. 
                int centerY = tom.Random.Next(0, tom.Height - 1);
                int centerX = tom.Random.Next(0, tom.Width - 1);
                float radius = tom.Random.Next(tom.MinimumVesicleRadius, tom.MaximumVesicleRadius);
                bool found = false;

                // keep going until we find something, or break after 1000 attempts
                for (int c = 0; c < 1000 && !found; c++)
                {
                    bool intercept = false;
                    foreach (Vesicle vesicle in vesicles)
                    {
                        float distance = (float)System.Math.Sqrt((centerY - vesicle.CenterY) * (centerY - vesicle.CenterY) +
                            (centerX - vesicle.CenterX) * (centerX - vesicle.CenterX));
                        if (distance <= (vesicle.Radius + radius + 10))
                        {
                            intercept = true;
                            break;
                        }
                    }

                    if (!intercept || vesicles.Count == 0)
                    {
                        found = true;
                        vesicles.Add(new Vesicle { CenterX = centerX, CenterY = centerY, Radius = radius });
                    }
                    else
                    {
                        centerY = tom.Random.Next(0, tom.Height - 1);
                        centerX = tom.Random.Next(0, tom.Width - 1);
                        radius = tom.Random.Next(tom.MinimumVesicleRadius, tom.MaximumVesicleRadius);
                    }
                }
            }

            foreach (Vesicle vesicle in vesicles)
            {
                int centerY = vesicle.CenterY;
                int centerX = vesicle.CenterX;

                for (int y = centerY - (int)vesicle.Radius; y <= centerY + (int)vesicle.Radius; y++)
                {
                    for (int x = centerX - (int)vesicle.Radius; x <= centerX + (int)vesicle.Radius; x++)
                    {
                        float distance = (float)Math.Sqrt(
                            (centerY - y) * (centerY - y) + (centerX - x) * (centerX - x));

                        if (distance <= vesicle.Radius
                            && distance >= vesicle.Radius - tom.Random.Next(1, 3)
                            && y >= 0 && x >= 0 &&
                            y < tom.Height && x < tom.Width)
                        {
                            tom.DataClasses[y * tom.Width + x] = -1;
                        }
                    }
                }
            }
        }

        private static void BuildBackground(Tomogram tom)
        {

            Dictionary<int, List<int>> lookup = new Dictionary<int, List<int>>();

            // initialize by smattering the first ten percent. 
            for (int p = 1; p <= tom.BackgroundDensity * .1; p++)
            {
                int x = tom.Random.Next(0, tom.Width);
                int y = tom.Random.Next(0, tom.Height);
                int index = y * tom.Width + x;

                tom.DataClasses[index] = p;
                List<int> list = new List<int>();
                list.Add(index);
                lookup.Add(p, list);
            }

            // fill out
            for (; ; )
            {
                for (int p = 1; p <= tom.BackgroundDensity; p++)
                {
                    // is this class still viable? 
                    if (lookup.ContainsKey(p))
                    {
                        List<int> indices = lookup[p];
                        List<int> nextSteps = new List<int>();

                        foreach (int currentIndex in indices)
                        {
                            int currentY = currentIndex / tom.Width;
                            int currentX = currentIndex % tom.Width;

                            // identify possible next steps
                            for (int y = currentY - 1; y <= currentY + 1; y++)
                            {
                                for (int x = currentX - 1; x <= currentX + 1; x++)
                                {
                                    if (y >= 0 && y < tom.Height && x >= 0 && x < tom.Width)
                                    {
                                        int nextPossibleIndex = y * tom.Width + x;
                                        if (tom.DataClasses[nextPossibleIndex] == 0)
                                        {
                                            nextSteps.Add(nextPossibleIndex);
                                        }
                                    }
                                }
                            }

                            if (nextSteps.Count > 0)
                            {
                                break;
                            }
                        }

                        if (nextSteps.Count > 0)
                        {
                            // random step
                            int nextStep = nextSteps[tom.Random.Next(0, nextSteps.Count - 1)];
                            tom.DataClasses[nextStep] = p;

                            lookup[p].Add(nextStep);
                        }
                        else
                        {
                            lookup.Remove(p);
                        }
                    }
                }

                if (lookup.Count == 0)
                {
                    break;
                }
            }
        }
    }
}
