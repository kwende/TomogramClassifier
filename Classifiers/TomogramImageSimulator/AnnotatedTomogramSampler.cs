using MRCSharpLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TomogramImageSimulator
{
    public static class AnnotatedTomogramSampler
    {
        public static List<float> Sample(string annotatedBitmap, string correspondingMRCFileName, int correspondingMRCFrameNumber)
        {
            MRCFrame file = MRCParser.Parse(correspondingMRCFileName).Frames[correspondingMRCFrameNumber];

            List<float> ret = new List<float>();

            using (Bitmap bmp = (Bitmap)Image.FromFile(annotatedBitmap))
            {
                for (int y = 0, i = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++, i++)
                    {
                        Color c = bmp.GetPixel(x, y);

                        if (c.R != c.B)
                        {
                            ret.Add(file.Data[i]); 
                        }
                    }
                }
            }

            return ret;
        }
    }
}
