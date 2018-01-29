using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class LabeledPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Z { get; set; }
        public int Label { get; set; }
        public LabeledTomogram SourceTomogram { get; set; }
    }
}
