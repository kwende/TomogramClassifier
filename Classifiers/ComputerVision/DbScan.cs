using DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerVision
{
    public static class DbScan
    {
        private static float Distance(Point2D a, Point2D b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static Point2D[] Cluster(LabeledTomogram tomogram, float epsilon, int minimumClusterSize)
        {
            int clusterCounter = 1;
            int[] clusterLabels = new int[tomogram.Labels.Length];

            List<Point2D> points = new List<Point2D>();
            for (int y = 0, i = 0; y < tomogram.Height; y++)
            {
                for (int x = 0; x < tomogram.Width; x++, i++)
                {
                    if (tomogram.Labels[i] > 0)
                    {
                        points.Add(new Point2D { X = x, Y = y });
                    }
                }
            }

            foreach (Point2D point in points)
            {
                int index = (int)(point.Y * tomogram.Width + point.X);

                if (clusterLabels[index] == 0)
                {
                    List<Point2D> neighbors = new List<Point2D>();
                    foreach (Point2D otherPoint in points)
                    {
                        if (otherPoint != point && Distance(point, otherPoint) <= epsilon)
                        {
                            neighbors.Add(otherPoint);
                        }
                    }

                    if (neighbors.Count >= minimumClusterSize)
                    {

                    }
                    else
                    {
                        // noise
                        clusterLabels[index] = -1;
                    }
                }
            }

            return null;
        }
    }
}
