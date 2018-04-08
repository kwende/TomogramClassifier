using DataStructures;
using Maths;
using MRCSharpLib;
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
    public static class SamplingVoronoiDiagramBuilder
    {
        public static void SaveAsDatFile(Tomogram tomogram, string path)
        {
            using (FileStream fs = File.Create(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tomogram);
            }
        }


        public static Tomogram BuildTomogram(int width, int height,
            int backgroundDensity, int vesicleCount, MRCFile file, Random rand)
        {

            Tomogram ret = new Tomogram();
            ret.BlurRadius = 2;
            ret.Width = width + ret.BlurRadius * 2;
            ret.Height = height + ret.BlurRadius * 2;
            ret.FinalWidth = width;
            ret.FinalHeight = height;
            ret.BackgroundDensity = backgroundDensity;
            ret.VesicleCount = vesicleCount;
            ret.Random = new Random();
            ret.MinimumVesicleRadius = 8;
            ret.MaximumVesicleRadius = 15;
            ret.Data = new float[ret.Width * ret.Height];
            ret.DataClasses = new int[ret.Width * ret.Height];

            BuildBackground(ret);
            AddVesicles(ret);

            FinalizeFrame(ret, rand, file, "distribution.dat");

            return ret;
        }

        private static void FinalizeFrame(Tomogram tom, Random rand, MRCFile file,
            string serializedSamplerPath)
        {

            float minValue = file.MinPixelValue;
            float maxValue = file.MaxPixelValue;

            tom.MRCScaler = 255.0f / (maxValue - minValue);
            tom.MinimumTomogramValue = minValue;

            int[] classes = tom.DataClasses.Where(n => n != -1).Distinct().ToArray();

            Dictionary<int, float> classValues = new Dictionary<int, float>();
            for (int c = 0; c < classes.Length; c++)
            {
                MRCFrame frame = file.Frames[145];// rand.Next(0, file.Frames.Count - 1)];

                int randomX = rand.Next(511, 611);
                int randomY = rand.Next(292, 392);
                int randomIndex = randomY * frame.Width + randomX; 
                float value = frame.Data[randomIndex];
                classValues.Add(classes[c], value);
            }

            for (int y = 0, i = 0; y < tom.Height; y++)
            {
                for (int x = 0; x < tom.Width; x++, i++)
                {
                    int classNumber = tom.DataClasses[i];
                    if (classNumber >= 0)
                    {
                        tom.Data[i] = classValues[classNumber];
                    }
                }
            }


            List<float> distribution = null;
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fin = File.OpenRead(serializedSamplerPath))
            {
                distribution = bf.Deserialize(fin) as List<float>;
            }

            int[] mask = new int[tom.Width * tom.Height];

            for (int y = 0, i = 0; y < tom.Height; y++)
            {
                for (int x = 0; x < tom.Width; x++, i++)
                {
                    int classNumber = tom.DataClasses[i];
                    if (classNumber == -1)
                    {
                        tom.Data[i] = distribution[rand.Next(0, distribution.Count - 1)];
                        mask[i] = 0;
                    }
                    else
                    {
                        mask[i] = 1;
                    }
                }
            }
            //            tom.Data = blur.BlurData(tom.Data, tom.Width, tom.Height);

            GaussianBlur blur = GaussianBlur.BuildBlur(.65f, tom.BlurRadius);
            tom.Data = blur.SelectivelyBlurData(tom.Data, mask, tom.Width, tom.Height, 1f, rand);

            float[] finalData = new float[tom.FinalHeight * tom.FinalWidth];
            int[] finalDataClasses = new int[tom.FinalHeight * tom.FinalWidth]; 

            for (int y = 0, dstIndex = 0; y < tom.FinalHeight; y++)
            {
                for (int x = 0; x < tom.FinalHeight; x++, dstIndex++)
                {
                    int srcIndex = (y + tom.BlurRadius) * tom.Width + (x + tom.BlurRadius);
                    finalData[dstIndex] = tom.Data[srcIndex];
                    finalDataClasses[dstIndex] = tom.DataClasses[srcIndex]; 
                }
            }

            // hack. clean this up. 
            tom.Data = finalData;
            tom.DataClasses = finalDataClasses; 
            tom.Width = tom.FinalWidth;
            tom.Height = tom.FinalHeight;
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
                            && distance >= vesicle.Radius - tom.Random.Next(2, 4)
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
            for (int p = 1; p <= tom.BackgroundDensity; p++)
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
