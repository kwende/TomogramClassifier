using DecisionTreeClassifier.DataStructures;
using DecisionTreeClassifier.DataStructures.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecisionTreeClassifier.DecisionTree
{
    public static class DecisionTreeBuilder
    {
        static double ComputeShannonEntropy(List<LabeledPoint> trainingPoints)
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

        private static List<SplittingQuestion> GenerateSplittingQuestions(Random random, DecisionTreeOptions options)
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

        private static SplitDirection ComputeSplitDirection(LabeledPoint point, SplittingQuestion question,
            DecisionTreeOptions options)
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
            int z = point.Y * frameWidth + point.X;

            float uVal = 0f, vVal = 0f, zVal = point.SourceTomogram.Data[z];
            if (u < 0 || v < 0)
            {
                uVal = vVal = options.OutOfRangeValue;
            }
            else
            {
                uVal = point.SourceTomogram.Data[u];
                vVal = point.SourceTomogram.Data[v];
            }

            if ((uVal - vVal) < question.Threshold)
            {
                return SplitDirection.Left;
            }
            else
            {
                return SplitDirection.Right;
            }
        }

        private static double ComputeGain(double currentShannonEntropy, List<LabeledPoint> left, List<LabeledPoint> right)
        {
            double leftEntropy = ComputeShannonEntropy(left);
            double rightEntropy = ComputeShannonEntropy(right);

            double leftLength = left.Count;
            double rightLength = right.Count;
            double totalNumberOfItems = leftLength + rightLength;

            return currentShannonEntropy - (leftEntropy * (leftLength / totalNumberOfItems) +
                rightEntropy * (rightLength / totalNumberOfItems));
        }

        private static void MakeLeafNode(DecisionTreeNode currentNode, List<LabeledPoint> trainingPoints)
        {
            currentNode.IsLeaf = true;
            currentNode.Class = trainingPoints.GroupBy(n => n.Label).OrderByDescending(n => n.Count()).First().Key;

            return;
        }

        private static void RecurseAndPartition(List<LabeledPoint> trainingPoints, List<SplittingQuestion> splittingQuestions,
            int currentRecursionLevel, DecisionTreeOptions options, DecisionTreeNode currentNode, Random random)
        {
            if (currentRecursionLevel >= options.MaximumNumberOfRecursionLevels)
            {
                // create leaf node
                MakeLeafNode(currentNode, trainingPoints);
            }
            else
            {
                double currentShannonEntropy = ComputeShannonEntropy(trainingPoints);
                double highestGain = double.MinValue;
                List<LabeledPoint> bestLeftBucket = null, bestRightBucket = null;
                SplittingQuestion bestSplittingQuestion = null;

                for (int s = 0; s < splittingQuestions.Count; s++)
                {
                    List<LabeledPoint> leftBucket = new List<LabeledPoint>();
                    List<LabeledPoint> rightBucket = new List<LabeledPoint>();

                    SplittingQuestion splittingQuestion = splittingQuestions[s];

                    for (int p = 0; p < trainingPoints.Count; p++)
                    {
                        LabeledPoint trainingPoint = trainingPoints[p];

                        SplitDirection split = ComputeSplitDirection(trainingPoint, splittingQuestion, options);

                        if (split == SplitDirection.Left)
                        {
                            leftBucket.Add(trainingPoint);
                        }
                        else
                        {
                            rightBucket.Add(trainingPoint);
                        }
                    }

                    double gain = ComputeGain(currentShannonEntropy, leftBucket, rightBucket);

                    if (gain > highestGain)
                    {
                        highestGain = gain;
                        bestLeftBucket = leftBucket;
                        bestRightBucket = rightBucket;
                        bestSplittingQuestion = splittingQuestion;
                    }
                }

                if (highestGain > options.SufficientGainLevel)
                {
                    currentNode.Question = bestSplittingQuestion;
                    currentNode.LeftBranch = new DecisionTreeNode();
                    currentNode.RightBranch = new DecisionTreeNode();
                    currentNode.IsLeaf = false;

                    RecurseAndPartition(bestLeftBucket, splittingQuestions,
                        currentRecursionLevel + 1, options, currentNode.LeftBranch, random);

                    RecurseAndPartition(bestRightBucket, splittingQuestions,
                        currentRecursionLevel + 1, options, currentNode.RightBranch, random);
                }
                else
                {
                    MakeLeafNode(currentNode, trainingPoints);
                }
            }
        }

        private static List<LabeledPoint> TomogramsToPoints(List<LabeledTomogram> tomograms)
        {
            List<LabeledPoint> points = new List<LabeledPoint>();

            foreach (LabeledTomogram tomogram in tomograms)
            {
                for (int y = 0, i = 0; y < tomogram.Height; y++)
                {
                    for (int x = 0; x < tomogram.Width; x++, i++)
                    {
                        float value = tomogram.Data[i];

                        points.Add(new LabeledPoint
                        {
                            X = x,
                            Y = y,
                            Z = value,
                            Label = (int)tomogram.Labels[i],
                            SourceTomogram = tomogram,
                        });
                    }
                }
            }

            return points;
        }

        public static void Train(List<LabeledTomogram> trainingImages, Random random, DecisionTreeOptions options)
        {
            List<LabeledPoint> trainingPoints = TomogramsToPoints(trainingImages);

            List<SplittingQuestion> splittingQuestions =
                GenerateSplittingQuestions(random, options);

            DecisionTreeNode root = new DecisionTreeNode();

            RecurseAndPartition(trainingPoints, splittingQuestions,
                1, options, root, random);
        }
    }
}
