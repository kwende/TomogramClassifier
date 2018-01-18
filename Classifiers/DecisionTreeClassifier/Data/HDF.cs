using HDF5DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeClassifier.Data
{
    public static class HDF
    {
        public static void TestLoad()
        {
            H5.Open();

            H5FileId file = H5F.open(@"E:\Downloads\rawtomograms-tomography18sirtflipclipgood2_ptcls.hdf", H5F.OpenMode.ACC_RDONLY);

            int startIndex = 0;
            List<string> objectNames = new List<string>(); 
            H5G.iterate(file, "/MDF/images", (id, objectName, param) =>
            {
                objectNames.Add($"/MDF/images/{objectName}"); 

                Console.WriteLine($"{id},{objectName},{param}"); 

                return 0; 
            }, 0, ref startIndex);  

        }
    }
}
