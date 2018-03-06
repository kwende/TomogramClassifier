using MRCSharpLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Tester
{
    public static class LabeledEdgeFinder
    {
        public static void DoIt()
        {
            const string TomogramDirectory = @"C:\Users\Ben\Desktop\tomograms";

            Console.WriteLine("Loading main file..");
            MRCFile file = MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.mrc"));

            Console.WriteLine("Loading label files...");
            List<MRCFile> labelFiles = new List<MRCFile>();
            labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels.mrc")));
            labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels2.mrc")));
            labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels3.mrc")));
            labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels4.mrc")));
            labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels5.mrc")));
            labelFiles.Add(MRCParser.Parse(Path.Combine(TomogramDirectory, "tomography2_fullsirtcliptrim.labels7.mrc")));

            float scaler = 255 / (file.MaxPixelValue - file.MinPixelValue);

            Color[] colors = new Color[]
            {
                Color.Red, Color.Green, Color.Pink, Color.Orange, Color.Yellow, Color.Violet
            };

            for (int z = 0; z < file.Frames.Count; z++)
            {
                Console.WriteLine($"File {z}/{file.Frames.Count}");
                MRCFrame frame = file.Frames[z];
                using (Bitmap bmp = new Bitmap(frame.Width, frame.Height))
                {
                    for (int y = 1; y < frame.Height - 1; y++)
                    {
                        for (int x = 1; x < frame.Width - 1; x++)
                        {
                            //if(x == 196 && y == 123 && z == 58)
                            //{
                            //    Console.WriteLine("."); 
                            //}

                            try
                            {
                                int i = y * frame.Width + x;

                                bool labeled = false;
                                int colorIndex = 0;
                                //foreach (MRCFile label in labelFiles)
                                //{
                                //    if (label.Frames[z].Data[i] > 0)
                                //    {
                                //        bool isEdge = false;
                                //        for (int y1 = y - 1; y1 <= y + 1; y1++)
                                //        {
                                //            for (int x1 = x - 1; x1 <= x + 1; x1++)
                                //            {
                                //                int offsetIndex = y1 * frame.Width + x1;

                                //                if (label.Frames[z].Data[offsetIndex] == 0)
                                //                {
                                //                    isEdge = true;
                                //                    break;
                                //                }
                                //            }
                                //        }

                                //        if (isEdge)
                                //        {
                                //            byte b = (byte)(frame.Data[i] * scaler);
                                //            bmp.SetPixel((int)(x), (int)(y), colors[colorIndex]);
                                //            labeled = true;
                                //        }
                                //    }

                                //    colorIndex++;
                                //}

                                if (!labeled)
                                {
                                    float v = frame.Data[i] + Math.Abs(file.MinPixelValue);
                                    byte b = (byte)(v * scaler);
                                    bmp.SetPixel((int)(x), (int)(y), Color.FromArgb(b, b, b));
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    bmp.Save(Path.Combine(TomogramDirectory, $"{z}.png"), ImageFormat.Png);
                }
            }
        }
    }
}
