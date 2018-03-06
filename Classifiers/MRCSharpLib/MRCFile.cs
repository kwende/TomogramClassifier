using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MRCSharpLib
{
    public class MRCFile
    {
        public List<MRCFrame> Frames { get; set; }
        public float MinPixelValue { get; set; }
        public float MaxPixelValue { get; set; }
        public float PixelSize { get; set; }
    }
}
