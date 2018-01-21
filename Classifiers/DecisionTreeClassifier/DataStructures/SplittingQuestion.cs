using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionTreeClassifier.DataStructures
{
    [Serializable]
    public class SplittingQuestion
    {
        public int OffsetUX { get; set; }
        public int OffsetUY { get; set; }
        public int OffsetVX { get; set; }
        public int OffsetVY { get; set; }
        public float Threshold { get; set; }
    }
}
