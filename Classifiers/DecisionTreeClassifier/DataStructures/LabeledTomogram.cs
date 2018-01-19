using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionTreeClassifier.DataStructures
{
    public class LabeledTomogram
    {
        public float[] Data { get; set; }
        public float[] Labels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
