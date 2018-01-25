using DecisionTreeClassifier.DataIO;
using DecisionTreeClassifier.DataStructures;
using DecisionTreeClassifier.DecisionTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace DecisionTreeClassifier
{
    class Program
    {
        static void Train()
        {
            List<LabeledTomogram> tomograms = new List<LabeledTomogram>();
            for (int c = 0; c <= 10; c++)
            {
                string dataFile = $"C:/Users/Ben/Desktop/DataSets/data/{c}.bin";
                string labelFile = $"C:/Users/Ben/Desktop/DataSets/labels/{c}.bin";

                LabeledTomogram tom = DataReader.ReadTomogramPair(dataFile, labelFile, 64, 64);

                tomograms.Add(tom);
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

                string file = $"C:/Users/Ben/Desktop/DataSets/data/0.bin";

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

                float minFloat = tom.Data.Min();
                float maxfloat = tom.Data.Max();

                float delta = maxfloat - minFloat;
                float scaler = 255 / delta;

                using (Bitmap bmp = new Bitmap(tom.Width, tom.Height))
                {
                    for (int y = 0, i = 0; y < tom.Height; y++)
                    {
                        for (int x = 0; x < tom.Width; x++, i++)
                        {
                            if (labels[i] > 0)
                            {
                                bmp.SetPixel(x, y, System.Drawing.Color.Red);
                            }
                            else
                            {
                                byte b = (byte)((tom.Data[i] + Math.Abs(minFloat)) * scaler);

                                bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(b, b, b));
                            }
                        }
                    }

                    bmp.Save("c:/users/ben/desktop/test0.bmp"); 
                }
            }
        }

        static void Main(string[] args)
        {
            Test();
        }
    }
}
