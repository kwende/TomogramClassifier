using DecisionTreeClassifier.DataIO;
using DecisionTreeClassifier.DataStructures;
using DecisionTreeClassifier.DecisionTree;
using System;
using System.Collections.Generic;

namespace DecisionTreeClassifier
{
    class Program
    {
        static void Main(string[] args)
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
            };

            DecisionTreeBuilder.Train(tomograms, new Random(1234), options);

            return;
        }
    }
}
