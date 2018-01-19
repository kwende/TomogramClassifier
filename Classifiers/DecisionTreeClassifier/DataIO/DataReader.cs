using DecisionTreeClassifier.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DecisionTreeClassifier.DataIO
{
    public static class DataReader
    {
        public static LabeledTomogram ReadTomogramPair(string dataFile, string labelFile, int width, int height)
        {
            byte[] dataFileBytes = File.ReadAllBytes(dataFile);
            byte[] labelFileBytes = File.ReadAllBytes(labelFile);

            if (width * height != dataFileBytes.Length / sizeof(float) &&
                width * height != labelFileBytes.Length / sizeof(float))
            {
                throw new ArgumentOutOfRangeException("Width or height specify invalid dimensions for this data file.");
            }

            LabeledTomogram ret = new LabeledTomogram();

            ret.Data = new float[dataFileBytes.Length / sizeof(float)];
            ret.Labels = new float[labelFileBytes.Length / sizeof(float)];

            Buffer.BlockCopy(dataFileBytes, 0, ret.Data, 0, dataFileBytes.Length);
            Buffer.BlockCopy(labelFileBytes, 0, ret.Labels, 0, labelFileBytes.Length);

            ret.Width = width;
            ret.Height = height;

            return ret;
        }
    }
}
