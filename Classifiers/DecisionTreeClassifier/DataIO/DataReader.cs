using DataStructures;
using DecisionTreeClassifier.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DecisionTreeClassifier.DataIO
{
    public static class DataReader
    {
        public static LabeledTomogram ReadDatFile(string datFile)
        {
            using (FileStream fin = File.OpenRead(datFile))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Tomogram tom = (Tomogram)bf.Deserialize(fin);

                LabeledTomogram l = new LabeledTomogram();
                l.Data = tom.Data;
                l.Width = tom.Width;
                l.Height = tom.Height;
                l.Labels = new float[l.Data.Length];

                for (int c = 0; c < l.Labels.Length; c++)
                {
                    if (tom.DataClasses[c] == -1)
                    {
                        l.Labels[c] = 1;
                    }
                    else
                    {
                        l.Labels[c] = 0;
                    }
                }

                return l;
            }
        }

        public static LabeledTomogram ReadTomogramPair(string dataFile, string labelFile, int width, int height)
        {
            byte[] dataFileBytes = File.ReadAllBytes(dataFile);
            byte[] labelFileBytes = null;

            if (!string.IsNullOrEmpty(labelFile))
            {
                labelFileBytes = File.ReadAllBytes(labelFile);
            }

            if (width * height != dataFileBytes.Length / sizeof(float) &&
                width * height != labelFileBytes.Length / sizeof(float))
            {
                throw new ArgumentOutOfRangeException("Width or height specify invalid dimensions for this data file.");
            }

            LabeledTomogram ret = new LabeledTomogram();

            ret.Data = new float[dataFileBytes.Length / sizeof(float)];
            Buffer.BlockCopy(dataFileBytes, 0, ret.Data, 0, dataFileBytes.Length);

            if (labelFileBytes != null)
            {
                ret.Labels = new float[labelFileBytes.Length / sizeof(float)];
                Buffer.BlockCopy(labelFileBytes, 0, ret.Labels, 0, labelFileBytes.Length);
            }

            ret.Width = width;
            ret.Height = height;

            return ret;
        }
    }
}
