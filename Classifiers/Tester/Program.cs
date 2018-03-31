using MRCSharpLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Tester
{
    class Program
    {
        private static void SavePLY(string outputFile, List<PLYVertex> vertices)
        {
            using (StreamWriter sw = new StreamWriter(outputFile))
            {
                sw.WriteLine("ply");
                sw.WriteLine("format ascii 1.0");
                sw.WriteLine("element vertex " + vertices.Count.ToString());
                sw.WriteLine("property float x");
                sw.WriteLine("property float y");
                sw.WriteLine("property float z");
                sw.WriteLine("property uchar red");
                sw.WriteLine("property uchar green");
                sw.WriteLine("property uchar blue");
                sw.WriteLine("element face " + 0.ToString());
                sw.WriteLine("property list uchar uint vertex_indices");
                sw.WriteLine("end_header");

                foreach (PLYVertex vertex in vertices)
                {
                    sw.WriteLine("{0} {1} {2} {3} {4} {5}", vertex.X, vertex.Y, vertex.Z,
                        Color.White.R, Color.White.G, Color.White.B);
                }
            }
        }

        static void Main(string[] args)
        {
            //Convolution.DoIt(); 
            //EdgeTracer.DoIt(); 
            //LabeledEdgeFinder.DoIt(); 

            const string TomogramDirectory = @"c:/users/ben/downloads/";

            Console.WriteLine("Loading main file..");
            MRCFile file = MRCParser.Parse(@"E:\Downloads\tomography2_fullsirtcliptrim.mrc");

            //MRCFrame frame = file.Frames[145];

            //float[] data = 
            //for (int y = 264; y < 363; y++)
            //{
            //    for (int x = 501; x < 600; x++)
            //    {

            //    }
            //}

            float scaler = 255 / (file.MaxPixelValue - file.MinPixelValue);

            //Console.WriteLine("Loading label files...");
            //List<MRCFile> labelFiles = new List<MRCFile>();
            //labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels.mrc")));
            //labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels2.mrc")));
            //labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels3.mrc")));
            //labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels4.mrc")));
            //labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels5.mrc")));
            //labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels7.mrc")));

            //Color[] colors = new Color[]
            //{
            //    Color.Red, Color.Green, Color.Pink, Color.Orange, Color.Yellow, Color.Violet
            //};

            //

            float skipLength = 1.0f;

            //int vertexCount = 0;
            //using (StreamWriter sw = new StreamWriter(Path.Combine(TomogramDirectory, "pointCloudEnding.ply")))
            //{
            for (int z = 0; z < file.Frames.Count; z++)
            {
                Console.WriteLine($"File {z}/{file.Frames.Count}");
                MRCFrame frame = file.Frames[z];
                using (Bitmap bmp = new Bitmap(frame.Width / (int)skipLength, frame.Height / (int)skipLength))
                {
                    for (int y = 0; y < frame.Height; y += (int)skipLength)
                    {
                        for (int x = 0; x < frame.Width; x += (int)skipLength)
                        {
                            try
                            {
                                int i = y * frame.Width + x;

                                bool labeled = false;
                                int colorIndex = 0;
                                //foreach (MRCFile label in labelFiles)
                                //{
                                //    if (label.Frames[z].Data[i] > 0)
                                //    {
                                //        byte b = (byte)(frame.Data[i] * scaler);
                                //        bmp.SetPixel((int)(x / skipLength), (int)(y / skipLength), colors[colorIndex]);
                                //        sw.WriteLine("{0} {1} {2} {3} {4} {5}",
                                //            x / skipLength,
                                //            y / skipLength,
                                //            z / skipLength,
                                //            colors[colorIndex].R, colors[colorIndex].G, colors[colorIndex].B);
                                //        //vertices.Add(new PLYVertex { X = x, Y = y, Z = z, Color = colors[colorIndex] });
                                //        labeled = true;
                                //        vertexCount++;
                                //    }

                                //    colorIndex++;
                                //}

                                //if (!labeled)
                                {
                                    float v = frame.Data[i] + Math.Abs(file.MinPixelValue);
                                    byte b = (byte)(v * scaler);
                                    bmp.SetPixel((int)(x / skipLength), (int)(y / skipLength), Color.FromArgb(b, b, b));
                                    //vertices.Add(new PLYVertex { X = x, Y = y, Z = z, Color = Color.FromArgb(b, b, b) });
                                }
                            }
                            catch
                            {
                                //sw.WriteLine("{0} {1} {2} {3} {4} {5}", x, y, z, 0, 0, 0);
                                //vertexCount++;
                                //vertices.Add(new PLYVertex { X = x, Y = y, Z = z, Color = Color.FromArgb(0, 0, 0) });
                            }
                        }
                    }
                    bmp.Save(Path.Combine("C:/users/ben/desktop/samples/", $"{z}.png"), ImageFormat.Png);
                }
            }
            //}

            //using (StreamWriter sw = new StreamWriter(Path.Combine(TomogramDirectory, "pointCloud.ply")))
            //{
            //    sw.WriteLine("ply");
            //    sw.WriteLine("format ascii 1.0");
            //    sw.WriteLine("element vertex " + vertexCount.ToString());
            //    sw.WriteLine("property float x");
            //    sw.WriteLine("property float y");
            //    sw.WriteLine("property float z");
            //    sw.WriteLine("property uchar red");
            //    sw.WriteLine("property uchar green");
            //    sw.WriteLine("property uchar blue");
            //    sw.WriteLine("element face " + 0.ToString());
            //    sw.WriteLine("property list uchar uint vertex_indices");
            //    sw.WriteLine("end_header");

            //    using (StreamReader sr = new StreamReader(Path.Combine(TomogramDirectory, "pointCloudEnding.ply")))
            //    {
            //        string l = null;
            //        while ((l = sr.ReadLine()) != null)
            //        {
            //            sw.WriteLine(l);
            //        }
            //    }

            //}

            //using (StreamWriter sw = new StreamWriter(Path.Combine(TomogramDirectory, "pointCloud.json")))
            //{
            //    using (StreamReader sr = new StreamReader(Path.Combine(TomogramDirectory, "pointCloudEnding.ply")))
            //    {
            //        sw.WriteLine("{ \"data\": [");

            //        string l = null;
            //        while ((l = sr.ReadLine()) != null)
            //        {
            //            string[] bits = l.Split(' ');

            //            sw.WriteLine($"\"{bits[0]} {bits[1]} {bits[2]}\",");
            //            sw.WriteLine($"\"{bits[3]} {bits[4]} {bits[5]}\",");
            //        }

            //        sw.WriteLine($"\"0 0 0\",");
            //        sw.WriteLine($"\"0 0 0\"");

            //        sw.WriteLine("]}");
            //    }
            //}

            //File.Delete(Path.Combine(TomogramDirectory, "pointCloudEnding.ply"));
        }
    }
}
