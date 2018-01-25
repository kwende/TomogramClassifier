using DecisionTreeClassifier.DataStructures;
using System;
using System.Collections.Generic;
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

        private static LabeledTomogram FlipTomogram(LabeledTomogram input)
        {
            LabeledTomogram ret = ShallowCopy(input);

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    int sourceIndex = y * input.Width + x;
                    int destinationIndex = (input.Height - 1 - y) * input.Width + (input.Width - 1 - x);

                    ret.Data[destinationIndex] = input.Data[sourceIndex];
                    ret.Labels[destinationIndex] = input.Labels[sourceIndex];
                }
            }

            return ret;
        }

        private static LabeledTomogram RotateTomogram(LabeledTomogram input, float degrees)
        {
            LabeledTomogram ret = ShallowCopy(input);

            double aa = Math.Cos(degrees);
            double ab = -Math.Sin(degrees);
            double ba = Math.Sin(degrees);
            double bb = Math.Cos(degrees);

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    int newX = (int)Math.Round(aa * x + ab * y);
                    int newY = (int)Math.Round(ba * x + bb * y);

                    int sourceIndex = y * input.Width + x;
                    int destinationIndex = newY * input.Width + newX;

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
    }
}
