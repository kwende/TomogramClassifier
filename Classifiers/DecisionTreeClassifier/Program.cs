using DecisionTreeClassifier.DataIO;
using DecisionTreeClassifier.DataStructures;
using System;

namespace DecisionTreeClassifier
{
    class Program
    {
        static void Main(string[] args)
        {
            LabeledTomogram tom = DataReader.ReadTomogramPair(@"C:\Users\ben\Desktop\DataSets\data\0.bin",
                @"C:\Users\ben\Desktop\DataSets\labels\0.bin", 64, 64);

            return;
        }
    }
}
