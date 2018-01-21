using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionTreeClassifier.DataStructures
{
    public class LabeledPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Label { get; set; }
        public LabeledTomogram SourceTomogram { get; set; }
    }
}
