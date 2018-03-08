using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionTreeClassifier.DataStructures
{
    public class DecisionTreeOptions
    {
        public int NumberOfThresholds { get; set; }
        public int NumberOfFeatures { get; set; }
        public int OffsetXMax { get; set; }
        public int OffsetXMin { get; set; }
        public int OffsetYMax { get; set; }
        public int OffsetYMin { get; set; }
        public int MaximumNumberOfRecursionLevels { get; set; }
        public float SplittingThresholdMax { get; set; }
        public int OutOfRangeValue { get; set; }
        public double SufficientGainLevel { get; set; }
        public float PercentageOfPixelsToUse { get; set; }
    }
}
