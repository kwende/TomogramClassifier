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

namespace DecisionTreeClassifier
{
    class Program
    {
        static void Train()
        {
            Console.WriteLine("Loading shit..."); 
            List<LabeledTomogram> tomograms = new List<LabeledTomogram>();
            string[] files = Directory.GetFiles(@".", "*.dat").Take(5).ToArray();
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
                MaximumNumberOfRecursionLevels = 20,
                NumberOfFeatures = 250,
                NumberOfThresholds = 25,
                OffsetXMax = 100,
                OffsetXMin = -100,
                OffsetYMax = 100,
                OffsetYMin = -100,
                OutOfRangeValue = 1000000,
                SplittingThresholdMax = .2f,
                SufficientGainLevel = 0,
                PercentageOfPixelsToUse = .1f
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
            using (FileStream fs = File.OpenRead("c:/users/ben/desktop/serialized.dat"))
            {
                DecisionTreeNode node = bf.Deserialize(fs) as DecisionTreeNode;

                LabeledTomogram tom = DataReader.ReadMRCFile("c:/users/ben/downloads/tomography2_fullsirtcliptrim.mrc", 150); 

                DecisionTreeOptions options = new DecisionTreeOptions
                {
                    // TODO: Fill in
                    MaximumNumberOfRecursionLevels = 20,
                    NumberOfFeatures = 250,
                    NumberOfThresholds = 25,
                    OffsetXMax = 100,
                    OffsetXMin = -100,
                    OffsetYMax = 100,
                    OffsetYMin = -100,
                    OutOfRangeValue = 1000000,
                    SplittingThresholdMax = .2f,
                    SufficientGainLevel = 0,
                    PercentageOfPixelsToUse = .1f
                };

                float[] labels = DecisionTreeBuilder.Predict(tom, node, options);

                Bitmap bmp = DataManipulator.PaintClassifiedPixelsOnTomogram(tom, labels);
                bmp.Save("c:/users/ben/desktop/labeled.png", ImageFormat.Png); 

                //tom = Morphology.Open(new LabeledTomogram
                //{
                //    Data = tom.Data,
                //    Width = tom.Width,
                //    Height = tom.Height,
                //    Labels = labels,
                //}, 1, 1);

                //Cluster[] clusterCenters = DbScan.Cluster(tom, 2.5f, 5);

                //tom.Labels = null;

                //Bitmap bmp = DataManipulator.Tomogram2Bitmap(tom);
                //bmp = DataManipulator.PaintCentersOnBitmap(bmp, clusterCenters);
                //bmp.Save("labeled.bmp");
            }
        }

        static void Main(string[] args)
        {
            //Train();
            Test();
        }
    }
}
