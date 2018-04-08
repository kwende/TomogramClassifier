using DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TomogramImageSimulator
{
    public static class DistributionWriter
    {
        public static void WriteDistribution(Tomogram tom, string outputCSVFile)
        {
            using (StreamWriter sw = new StreamWriter(outputCSVFile))
            {
                foreach(float v in tom.Data)
                {
                    sw.WriteLine(v); 
                }
            }
        }
    }
}
