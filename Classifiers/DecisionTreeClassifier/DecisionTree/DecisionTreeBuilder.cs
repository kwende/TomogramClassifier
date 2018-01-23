using DecisionTreeClassifier.DataStructures;
using DecisionTreeClassifier.DataStructures.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionTreeClassifier.DecisionTree
{
    public class DecisionTreeBuilder
    {
        double ComputeShannonEntropy(List<LabeledPoint> trainingPoints)
        {
            double entropy = 0;

            // count number of classes
            double totalCount = trainingPoints.Count(n => n.Label != 0);
            int[] allLabels = trainingPoints.Select(n => n.Label).Distinct().ToArray();

            if (totalCount > 0)
            {
                foreach (int label in allLabels)
                {
                    double ratio = trainingPoints.Count(n => n.Label == label) / totalCount;
                    entropy += -(ratio * Math.Log(ratio, 2));
                }
            }

            return entropy;
        }

        public List<SplittingQuestion> GenerateSplittingQuestions(Random random, DecisionTreeOptions options)
        {
            List<SplittingQuestion> ret = new List<SplittingQuestion>();

            for (int c = 0; c < options.NumberOfFeatures; c++)
            {
                int uX = random.Next(options.OffsetXMin, options.OffsetXMax);
                int uY = random.Next(options.OffsetXMin, options.OffsetXMax);
                int vX = random.Next(options.OffsetXMin, options.OffsetXMax);
                int vY = random.Next(options.OffsetXMin, options.OffsetXMax);

                for (int d = 0; d < options.NumberOfThresholds; d++)
                {
                    int threshold =
                        random.Next(-options.SplittingThresholdMax, options.SplittingThresholdMax);

                    ret.Add(new SplittingQuestion
                    {
                        OffsetUX = uX,
                        OffsetUY = uY,
                        OffsetVX = vX,
                        OffsetVY = vY,
                        Threshold = threshold
                    });
                }
            }

            return ret;
        }

        private SplitDirection ComputeSplitDirection(LabeledPoint point, SplittingQuestion question)
        {
            int frameHeight = point.SourceTomogram.Height;
            int frameWidth = point.SourceTomogram.Width;

            int uY = point.Y + question.OffsetUY;
            if (uY >= frameHeight)
            {
                uY = -1;
            }
            int uX = point.X + question.OffsetUX;
            if (uX >= frameWidth)
            {
                uX = -1;
            }

            int vY = point.Y + question.OffsetVY;
            if (vY >= frameHeight)
            {
                vY = -1;
            }
            int vX = point.X + question.OffsetVX;
            if (vX >= frameWidth)
            {
                vX = -1;
            }

            int u = uY * frameWidth + uX;
            int v = vY * frameHeight + vX;

            if(u < 0 || v < 0)
            {

            }

            return SplitDirection.Left;
        }

        private void RecurseAndPartition(List<LabeledPoint> trainingPoints, int currentRecursionLevel,
            DecisionTreeOptions options, Random random)
        {
            if (currentRecursionLevel >= options.MaximumNumberOfRecursionLevels)
            {
                // create leaf node
            }
            else
            {
                double currentShannonEntropy = ComputeShannonEntropy(trainingPoints);


            }
        }

        public void Train(List<LabeledPoint> trainingPoints, Random random, DecisionTreeOptions options)
        {
            List<SplittingQuestion> splittingQuestions =
                GenerateSplittingQuestions(random, options);

            RecurseAndPartition(trainingPoints, 1, options, random);
        }
    }
}
