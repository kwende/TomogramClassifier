﻿using DataStructures;
using DecisionTreeClassifier.DataStructures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DecisionTreeClassifier.DataIO
{
    public static class DataManipulator
    {
        private static LabeledTomogram ShallowCopy(LabeledTomogram tom)
        {
            return new LabeledTomogram
            {
                Width = tom.Width,
                Height = tom.Height,
                Data = new float[tom.Data.Length],
                Labels = new float[tom.Labels.Length],
            };
        }

        public static LabeledTomogram FlipTomogram(LabeledTomogram input)
        {
            LabeledTomogram ret = ShallowCopy(input);

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    int sourceIndex = y * input.Width + x;
                    int destinationIndex = y * input.Width + (input.Width - 1 - x);

                    ret.Data[destinationIndex] = input.Data[sourceIndex];
                    ret.Labels[destinationIndex] = input.Labels[sourceIndex];
                }
            }

            return ret;
        }

        public static LabeledTomogram RotateTomogram(LabeledTomogram input, float degrees)
        {
            LabeledTomogram ret = ShallowCopy(input);

            double aa = Math.Cos(degrees * Math.PI / 180);
            double ab = -Math.Sin(degrees * Math.PI / 180);
            double ba = Math.Sin(degrees * Math.PI / 180);
            double bb = Math.Cos(degrees * Math.PI / 180);

            int centerX = input.Width / 2;
            int centerY = input.Height / 2;

            int halfWidth = input.Width / 2;
            int halfHeight = input.Height / 2;

            for (int y = -halfHeight; y < halfHeight; y++)
            {
                for (int x = -halfWidth; x < halfWidth; x++)
                {
                    int newX = (int)Math.Round(aa * x + ab * y);
                    int newY = (int)Math.Round(ba * x + bb * y);

                    int sourceIndex = (y + halfHeight) * input.Width + (x + halfWidth);
                    int destinationIndex = (newY + halfHeight) * input.Width + (newX + halfWidth);

                    if (sourceIndex >= 0 && sourceIndex < input.Data.Length &&
                        destinationIndex >= 0 && destinationIndex < input.Data.Length)
                    {
                        ret.Data[destinationIndex] = input.Data[sourceIndex];
                        ret.Labels[destinationIndex] = input.Labels[sourceIndex];
                    }
                }
            }

            return ret;
        }

        public static List<LabeledTomogram> FlipAndRotateTomogram(LabeledTomogram tomogram)
        {
            List<LabeledTomogram> ret = new List<LabeledTomogram>();
            ret.Add(tomogram);

            ret.Add(RotateTomogram(tomogram, 90));
            ret.Add(RotateTomogram(tomogram, 180));
            ret.Add(RotateTomogram(tomogram, 270));

            LabeledTomogram flippedTomogram = FlipTomogram(tomogram);
            ret.Add(flippedTomogram);

            ret.Add(RotateTomogram(flippedTomogram, 90));
            ret.Add(RotateTomogram(flippedTomogram, 180));
            ret.Add(RotateTomogram(flippedTomogram, 270));

            return ret;
        }

        public static Bitmap Tomogram2Bitmap(LabeledTomogram tomogram, int[] labels = null)
        {
            float minFloat = tomogram.Data.Min();
            float maxfloat = tomogram.Data.Max();

            float delta = maxfloat - minFloat;
            float scaler = 255 / delta;

            Bitmap bmp = new Bitmap(tomogram.Width, tomogram.Height);
            for (int y = 0, i = 0; y < tomogram.Height; y++)
            {
                for (int x = 0; x < tomogram.Width; x++, i++)
                {
                    if (labels != null && labels[i] > 0)
                    {
                        bmp.SetPixel(x, y, System.Drawing.Color.Red);
                    }
                    else
                    {
                        byte b = (byte)((tomogram.Data[i] + Math.Abs(minFloat)) * scaler);

                        bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(b, b, b));
                    }
                }
            }

            return bmp;
        }
    }
}
