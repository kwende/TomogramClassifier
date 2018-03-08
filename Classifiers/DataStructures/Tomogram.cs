using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    [Serializable]
    public class Tomogram
    {
        public int[] DataClasses { get; set; }
        public float MRCScaler { get; set; }
        public float[] Data { get; set; }
        public int BackgroundDensity { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int VesicleCount { get; set; }
        public int MinimumVesicleRadius { get; set; }
        public int MaximumVesicleRadius { get; set; }
        [NonSerialized]
        public Random Random; 
    }
}
