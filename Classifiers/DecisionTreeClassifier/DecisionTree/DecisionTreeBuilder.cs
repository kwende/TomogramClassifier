using DecisionTreeClassifier.DataStructures;
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
    }
}
