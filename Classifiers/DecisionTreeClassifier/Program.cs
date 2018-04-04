using DecisionTreeClassifier.DataIO;
using DecisionTreeClassifier.DataStructures;
using DecisionTreeClassifier.DecisionTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Drawing.Imaging;
using DataStructures;
using ComputerVision;
using MRCSharpLib;

namespace DecisionTreeClassifier
{
    class Program
    {
        static void Train()
        {
            Console.WriteLine("Loading shit...");
            List<LabeledTomogram> tomograms = new List<LabeledTomogram>();
            string[] files = Directory.GetFiles(@"/home/brush/tom4", "*.dat").ToArray();
            int i = 0;
            foreach (string file in files)
            {
                Console.WriteLine($"{i + 1}/{files.Length}");
                i++;
                //string dataFile = $"C:/Users/brush/Desktop/DataSets/data/{c}.bin";
                //string labelFile = $"C:/Users/brush/Desktop/DataSets/labels/{c}.bin";

                LabeledTomogram tom = DataReader.ReadDatFile(file);
                tomograms.AddRange(DataManipulator.FlipAndRotateTomogram(tom));
            }
            Console.WriteLine("...shit loaded.");

            DecisionTreeOptions options = new DecisionTreeOptions
            {
                // TODO: Fill in
                MaximumNumberOfRecursionLevels = 25,
                NumberOfFeatures = 250,
                NumberOfThresholds = 25,
                OffsetXMax = 25,
                OffsetXMin = -25,
                OffsetYMax = 25,
                OffsetYMin = -25,
                OutOfRangeValue = 1000000,
                SplittingThresholdMax = .2f,
                SufficientGainLevel = 0,
                PercentageOfPixelsToUse = 1f,
                //DistanceThreshold = .1f,
            };

            DecisionTreeNode node = DecisionTreeBuilder.Train(tomograms, new Random(1234), options);

            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.Create("serialized.dat"))
            {
                bf.Serialize(fs, node);
            }

            return;
        }

        static void Test()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = File.OpenRead("serialized.dat"))
            {
                DecisionTreeNode node = bf.Deserialize(fs) as DecisionTreeNode;

                DecisionTreeOptions options = new DecisionTreeOptions
                {
                    // TODO: Fill in
                    MaximumNumberOfRecursionLevels = 25,
                    NumberOfFeatures = 250,
                    NumberOfThresholds = 25,
                    OffsetXMax = 25,
                    OffsetXMin = -25,
                    OffsetYMax = 25,
                    OffsetYMin = -25,
                    OutOfRangeValue = 1000000,
                    SplittingThresholdMax = .2f,
                    SufficientGainLevel = 0,
                    PercentageOfPixelsToUse = 1f,
                    //DistanceThreshold = .1f,
                };

                LabeledTomogram tom = new LabeledTomogram();
                tom.Width = 100;
                tom.Height = 100;
                tom.Data = new float[100 * 100];

                MRCFile file = MRCParser.Parse(Path.Combine("/home/brush/tomography2_fullsirtcliptrim.mrc"));

                MRCFrame frame = file.Frames[145];

                for (int y = 264, i = 0; y < 364; y++)
                {
                    for (int x = 501; x < 601; x++, i++)
                    {
                        tom.Data[i] = frame.Data[y * frame.Width + x];
                    }
                }


                float[] labels = DecisionTreeBuilder.Predict(tom, node, options);
                //Bitmap bmp = DataManipulator.Tomogram2Bitmap(tom); 
                Bitmap bmp = Drawing.Tomogram.PaintClassifiedPixelsOnTomogram(tom, labels);
                bmp.Save("/var/www/html/static/labeled_real.png", System.Drawing.Imaging.ImageFormat.Png);

                LabeledTomogram tom2 = DataReader.ReadDatFile("/home/brush/0.dat");

                labels = DecisionTreeBuilder.Predict(tom2, node, options);
                //Bitmap bmp = DataManipulator.Tomogram2Bitmap(tom); 
                bmp = Drawing.Tomogram.PaintClassifiedPixelsOnTomogram(tom2, labels);
                bmp.Save("/var/www/html/static/labeled_simulated.png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        static void Main(string[] args)
        {
            Train();
            Test();
        }
    }
}
