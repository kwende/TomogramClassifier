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
