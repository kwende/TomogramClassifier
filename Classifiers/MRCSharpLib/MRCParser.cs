using System;
using System.Collections.Generic;
using System.IO;

namespace MRCSharpLib
{
    public class MRCParser
    {
        public static MRCFile Parse(string file)
        {
            MRCFile ret = new MRCFile();

            using (FileStream fin = File.OpenRead(file))
            {
                using (BinaryReader br = new BinaryReader(fin))
                {
                    int numColumns = br.ReadInt32();
                    int numRows = br.ReadInt32();
                    int numSections = br.ReadInt32();

                    int pixelType = br.ReadInt32();

                    int nxStart = br.ReadInt32();
                    int nyStart = br.ReadInt32();
                    int nzStart = br.ReadInt32();

                    int gridSizeX = br.ReadInt32();
                    int gridSizeY = br.ReadInt32();
                    int gridSizeZ = br.ReadInt32();

                    float cellSizeX = br.ReadSingle();
                    float cellSizeY = br.ReadSingle();
                    float cellSizeZ = br.ReadSingle();

                    float xSpacing = cellSizeX / gridSizeX;
                    float ySpacing = cellSizeY / gridSizeY;
                    float zSpacing = cellSizeZ / gridSizeZ;

                    ret.PixelSize = xSpacing;

                    float alpha = br.ReadSingle();
                    float beta = br.ReadSingle();
                    float gamma = br.ReadSingle();

                    int mapc = br.ReadInt32();
                    int mapr = br.ReadInt32();
                    int maps = br.ReadInt32();

                    float minPixelvalue = br.ReadSingle();
                    float maxPixelvalue = br.ReadSingle();
                    float avgPixelvalue = br.ReadSingle();

                    int ispg = br.ReadInt32();
                    int extendedHeaderLength = br.ReadInt32();

                    br.BaseStream.Seek(1024, SeekOrigin.Begin);

                    ret.MaxPixelValue = maxPixelvalue;
                    ret.MinPixelValue = minPixelvalue;

                    List<MRCFrame> frames = new List<MRCFrame>();

                    for (int c = 0; c < numSections; c++)
                    {
                        MRCFrame frame = new MRCFrame
                        {
                            Data = new float[numRows * numColumns],
                            Width = numColumns,
                            Height = numRows,
                        };

                        for (int y = 0, i = 0; y < numRows; y++)
                        {
                            for (int x = 0; x < numColumns; x++, i++)
                            {
                                if (pixelType == 2)
                                {
                                    float pixelValue = br.ReadSingle();
                                    frame.Data[i] = pixelValue;
                                }
                                else if (pixelType == 0)
                                {
                                    byte pixelValue = br.ReadByte();
                                    frame.Data[i] = pixelValue;
                                }
                            }
                        }

                        frames.Add(frame);
                    }

                    ret.Frames = frames;
                }
            }

            return ret;
        }
    }
}
