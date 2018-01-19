using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionTreeClassifier.DataStructures
{
    [Serializable]
    public class SplittingQuestion
    {
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public float Threshold { get; set; }
    }
}
