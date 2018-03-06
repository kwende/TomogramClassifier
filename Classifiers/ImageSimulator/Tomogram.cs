using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSimulator
{
    [Serializable]
    public class Tomogram
    {
        public float MRCScaler { get; set; }
        public int[] Data { get; set; }
        public int BackgroundDensity { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int VesicleCount { get; set; }
        public int MinimumVesicleRadius { get; set; }
        public int MaximumVesicleRadius { get; set; }
        public Random Random { get; set; }
    }
}
