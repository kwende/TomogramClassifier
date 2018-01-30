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
        public static LabeledTomogram Open(LabeledTomogram tomogram, int erodeElementRadius, int dilateElementRadius)
        {
            return Dilate(Erode(tomogram, erodeElementRadius), dilateElementRadius);
        }

        public static LabeledTomogram Dilate(LabeledTomogram tomogram, int elementRadius)
        {
            float[] labelsCopy = new float[tomogram.Labels.Length];
            for (int y = 0, i = 0; y < tomogram.Height; y++)
            {
                for (int x = 0; x < tomogram.Width; x++, i++)
                {
                    float label = tomogram.Labels[i];

                    if (label > 0)
                    {
                        labelsCopy[i] = label;

                        if (y > elementRadius - 1 && y < tomogram.Height - elementRadius &&
                            x > elementRadius - 1 && x < tomogram.Width - elementRadius)
                        {
                            for (int y1 = y - elementRadius; y1 < y + elementRadius; y1++)
                            {
                                for (int x1 = x - elementRadius; x1 < x + elementRadius; x1++)
                                {
                                    int index = y1 * tomogram.Width + x1;

                                    if (tomogram.Labels[index] == 0)
                                    {
                                        labelsCopy[index] = label;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            tomogram.Labels = labelsCopy;

            return tomogram;
        }

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
                            for (int y1 = y - elementRadius; y1 < y + elementRadius && keep; y1++)
                            {
                                for (int x1 = x - elementRadius; x1 < x + elementRadius && keep; x1++)
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

            tomogram.Labels = labelsCopy;

            return tomogram;
        }
    }
}
