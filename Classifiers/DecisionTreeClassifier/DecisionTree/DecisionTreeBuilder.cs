using DataStructures;
using DecisionTreeClassifier.DataStructures;
using DecisionTreeClassifier.DataStructures.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DecisionTreeClassifier.DecisionTree
{
    public static class DecisionTreeBuilder
    {

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
                    float threshold = (float)random.NextDouble() *
                        options.SplittingThresholdMax * Math.Sign(random.Next(-1, 1));

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
            int v = vY * frameWidth + vX;
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

        private static double ComputeShannonEntropy(List<LabeledPointGroup> groups)
        {
            double entropy = 0;

            // count number of classes
            double totalCount = groups.Sum(n => n.Count);

            if (totalCount > 0)
            {
                foreach (LabeledPointGroup group in groups)
                //for (int label = 0; label <= 1; label++)
                {
                    double ratio = group.Count / totalCount;
                    entropy += -(ratio * Math.Log(ratio, 2));
                }
            }

            return entropy;
        }

        static double ComputeShannonEntropy(List<LabeledPoint> trainingPoints)
        {
            double entropy = 0;

            // count number of classes
            double totalCount = trainingPoints.Count();// n => n.Label != 0);
            //IEnumerable<int> allLabels = trainingPoints.Select(n => n.Label).Distinct();

            if (totalCount > 0)
            {
                //foreach (int label in allLabels)
                for (int label = 0; label <= 1; label++)
                {
                    double ratio = trainingPoints.Count(n => n.Label == label) / totalCount;
                    entropy += -(ratio * Math.Log(ratio, 2));
                }
            }

            return entropy;
        }

        private static double ComputeGain(double currentShannonEntropy, List<LabeledPointGroup> leftGroup,
            List<LabeledPointGroup> rightGroup)
        {
            double leftEntropy = ComputeShannonEntropy(leftGroup);
            double rightEntropy = ComputeShannonEntropy(rightGroup);

            double leftLength = leftGroup.Sum(n => n.Count);
            double rightLength = rightGroup.Sum(n => n.Count);
            double totalNumberOfItems = leftLength + rightLength;

            return currentShannonEntropy - (leftEntropy * (leftLength / totalNumberOfItems) +
                rightEntropy * (rightLength / totalNumberOfItems));
        }

        private static double ComputeGain(double currentShannonEntropy, List<LabeledPoint> left,
            List<LabeledPoint> right)
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
            Console.WriteLine($"{new String('-', currentRecursionLevel)}{currentRecursionLevel}");

            if (currentRecursionLevel >= options.MaximumNumberOfRecursionLevels)
            {
                // create leaf node
                MakeLeafNode(currentNode, trainingPoints);
            }
            else
            {
                double currentShannonEntropy = ComputeShannonEntropy(trainingPoints);
                double highestGain = double.MinValue;
                SplittingQuestion bestSplittingQuestion = null;

                //for (int s = 0; s < splittingQuestions.Count; s++)
                Parallel.For(0, splittingQuestions.Count, s =>
                {
                    //Console.Write("."); 
                    //Interlocked.Increment(ref t);
                    //Console.WriteLine($"{t}/{splittingQuestions.Count}");

                    //List<LabeledPoint> leftBucket1 = new List<LabeledPoint>();
                    //List<LabeledPoint> rightBucket1 = new List<LabeledPoint>();

                    List<LabeledPointGroup> leftBucket = new List<LabeledPointGroup>();
                    leftBucket.Add(new LabeledPointGroup
                    {
                        Count = 0,
                        Class = 0
                    });
                    leftBucket.Add(new LabeledPointGroup
                    {
                        Count = 0,
                        Class = 1
                    });
                    List<LabeledPointGroup> rightBucket = new List<LabeledPointGroup>();
                    rightBucket.Add(new LabeledPointGroup
                    {
                        Count = 0,
                        Class = 0
                    });
                    rightBucket.Add(new LabeledPointGroup
                    {
                        Count = 0,
                        Class = 1
                    });

                    SplittingQuestion splittingQuestion = splittingQuestions[s];

                    for (int p = 0; p < trainingPoints.Count; p++)
                    {
                        if (random.NextDouble() < .1)
                        {
                            LabeledPoint trainingPoint = trainingPoints[p];

                            SplitDirection split = ComputeSplitDirection(trainingPoint, splittingQuestion, options);

                            if (split == SplitDirection.Left)
                            {
                                leftBucket[trainingPoint.Label].Count++;
                                //leftBucket1.Add(trainingPoint);
                            }
                            else
                            {
                                //rightBucket1.Add(trainingPoint);
                                rightBucket[trainingPoint.Label].Count++;
                            }
                        }
                    }

                    //double gain1 = ComputeGain(currentShannonEntropy, leftBucket1, rightBucket1); 
                    double gain = ComputeGain(currentShannonEntropy, leftBucket, rightBucket);

                    lock (typeof(DecisionTreeBuilder))
                    {
                        if (gain > highestGain)
                        {
                            highestGain = gain;
                            bestSplittingQuestion = splittingQuestion;
                        }
                    }
                });

                if (highestGain > options.SufficientGainLevel)
                {
                    List<LabeledPoint> bestLeftBucket = new List<LabeledPoint>();
                    List<LabeledPoint> bestRightBucket = new List<LabeledPoint>();

                    for (int p = 0; p < trainingPoints.Count; p++)
                    {
                        LabeledPoint trainingPoint = trainingPoints[p];

                        SplitDirection split = ComputeSplitDirection(trainingPoint, bestSplittingQuestion, options);

                        if (split == SplitDirection.Left)
                        {
                            bestLeftBucket.Add(trainingPoint);
                        }
                        else
                        {
                            bestRightBucket.Add(trainingPoint);
                        }
                    }

                    currentNode.Question = bestSplittingQuestion;
                    currentNode.LeftBranch = new DecisionTreeNode();
                    currentNode.RightBranch = new DecisionTreeNode();
                    currentNode.IsLeaf = false;

                    splittingQuestions =
                        GenerateSplittingQuestions(random, options);

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

        private static List<LabeledPoint> TomogramsToPoints(List<LabeledTomogram> tomograms,
            Random random, DecisionTreeOptions options)
        {
            List<LabeledPoint> points = new List<LabeledPoint>();

            foreach (LabeledTomogram tomogram in tomograms)
            {
                for (int y = 0, i = 0; y < tomogram.Height; y++)
                {
                    for (int x = 0; x < tomogram.Width; x++, i++)
                    {
                        float value = tomogram.Data[i];

                        if (random != null)
                        {
                            int label = (int)tomogram.Labels[i];
                            if (label == 1)
                            {
                                points.Add(new LabeledPoint
                                {
                                    X = x,
                                    Y = y,
                                    Z = value,
                                    Label = tomogram.Labels != null ? (int)tomogram.Labels[i] : -1,
                                    SourceTomogram = tomogram,
                                });
                            }
                            else
                            {
                                if (random.NextDouble() < options.PercentageOfPixelsToUse)
                                {
                                    points.Add(new LabeledPoint
                                    {
                                        X = x,
                                        Y = y,
                                        Z = value,
                                        Label = tomogram.Labels != null ? (int)tomogram.Labels[i] : -1,
                                        SourceTomogram = tomogram,
                                    });
                                }
                            }
                        }
                        else
                        {
                            points.Add(new LabeledPoint
                            {
                                X = x,
                                Y = y,
                                Z = value,
                                SourceTomogram = tomogram,
                            });
                        }
                    }
                }
            }

            return points;
        }

        public static DecisionTreeNode Train(List<LabeledTomogram> trainingImages, Random random, DecisionTreeOptions options)
        {
            Console.WriteLine("Building points...");
            List<LabeledPoint> trainingPoints = TomogramsToPoints(trainingImages, random, options);

            Console.WriteLine("Build splitting questions...");
            List<SplittingQuestion> splittingQuestions =
                GenerateSplittingQuestions(random, options);

            DecisionTreeNode root = new DecisionTreeNode();

            Console.WriteLine("Recurse and partition...");
            RecurseAndPartition(trainingPoints, splittingQuestions,
                1, options, root, random);

            return root;
        }

        private static int RecurseAndPredict(LabeledPoint point, DecisionTreeNode node, DecisionTreeOptions options)
        {
            if (node.IsLeaf)
            {
                return node.Class;
            }
            else
            {
                SplitDirection direction = ComputeSplitDirection(point, node.Question, options);

                if (direction == SplitDirection.Left)
                {
                    return RecurseAndPredict(point, node.LeftBranch, options);
                }
                else
                {
                    return RecurseAndPredict(point, node.RightBranch, options);
                }
            }
        }

        public static float[] Predict(LabeledTomogram image, DecisionTreeNode node, DecisionTreeOptions options)
        {
            List<LabeledPoint> points = TomogramsToPoints(
                new List<LabeledTomogram>(new LabeledTomogram[] { image }), null, null);

            List<float> labels = new List<float>();

            foreach (LabeledPoint point in points)
            {
                labels.Add(RecurseAndPredict(point, node, options));
            }

            return labels.ToArray();
        }
    }
}
