using System;
using System.Collections.Generic;
using System.Text;

namespace DecisionTreeClassifier.DataStructures
{
    [Serializable]
    public class DecisionTreeNode
    {
        public bool IsLeaf { get; set; }
        public bool IsMatch { get; set; }
        public DecisionTreeNode LeftBranch { get; set; }
        public DecisionTreeNode RightBranch { get; set; }
        public SplittingQuestion Question { get; set; }
    }
}
