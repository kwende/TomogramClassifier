using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVision
{
    public static class Morphology
    {
        public static LabeledTomogram Erode(LabeledTomogram tomogram, int elementRadius)
        {
            float[] labelsCopy = new float[tomogram.Labels.Length];
            for (int y = 0, i = 0; y < tomogram.Height; y++)
            {
                for (int x = 0; x < tomogram.Width; x++, i++)
                {
                    float label = tomogram.Labels[i];

                    if (label > 0)
                    {
                        if (y > elementRadius - 1 && y < tomogram.Height - elementRadius &&
                            x > elementRadius - 1 && x < tomogram.Width - elementRadius)
                        {
                            bool keep = true;
                            for (int y1 = y - elementRadius; y1 < y + elementRadius && keep; y++)
                            {
                                for (int x1 = x - elementRadius; x1 < x + elementRadius && keep; x++)
                                {
                                    int index = y1 * tomogram.Width + x1;

                                    if (tomogram.Labels[index] != label)
                                    {
                                        keep = false;
                                    }
                                }
                            }

                            if (keep)
                            {
                                labelsCopy[i] = label;
                            }
                        }
                    }
                }
            }

            return tomogram;
        }
    }
}
