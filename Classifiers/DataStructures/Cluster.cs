using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures
{
    public class Cluster
    {
        public Cluster()
        {
            Members = new List<Point2D>();
        }

        public Point2D Center { get; set; }
        public List<Point2D> Members { get; set; }
    }
}
