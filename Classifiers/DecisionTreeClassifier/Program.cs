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

namespace DecisionTreeClassifier
{
    class Program
    {
        static void Train()
        {
            List<LabeledTomogram> tomograms = new List<LabeledTomogram>();
            for (int c = 1; c <= 10; c++)
            {
                string dataFile = $"C:/Users/brush/Desktop/DataSets/data/{c}.bin";
                string labelFile = $"C:/Users/brush/Desktop/DataSets/labels/{c}.bin";

                LabeledTomogram tom = DataReader.ReadTomogramPair(dataFile, labelFile, 64, 64);

                tomograms.AddRange(DataManipulator.FlipAndRotateTomogram(tom));
            }

            DecisionTreeOptions options = new DecisionTreeOptions
            {
                // TODO: Fill in
                MaximumNumberOfRecursionLevels = 20,
                NumberOfFeatures = 250,
                NumberOfThresholds = 25,
                OffsetXMax = 10,
                OffsetXMin = -10,
                OffsetYMax = 10,
                OffsetYMin = -10,
                OutOfRangeValue = 1000000,
                SplittingThresholdMax = .2f,
                SufficientGainLevel = 0
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

                string file = $"C:/Users/brush/Desktop/DataSets/Data/0.bin";

                LabeledTomogram tom = DataReader.ReadTomogramPair(file, null, 64, 64);

                DecisionTreeOptions options = new DecisionTreeOptions
                {
                    // TODO: Fill in
                    MaximumNumberOfRecursionLevels = 20,
                    NumberOfFeatures = 250,
                    NumberOfThresholds = 25,
                    OffsetXMax = 10,
                    OffsetXMin = -10,
                    OffsetYMax = 10,
                    OffsetYMin = -10,
                    OutOfRangeValue = 1000000,
                    SplittingThresholdMax = .2f,
                    SufficientGainLevel = 0
                };

                int[] labels = DecisionTreeBuilder.Predict(tom, node, options);

                Bitmap bmp = DataManipulator.Tomogram2Bitmap(tom, labels);
                bmp.Save("labeled.bmp");
            }
        }

        static void Main(string[] args)
        {
            Train();
            Test();
        }
    }
}
