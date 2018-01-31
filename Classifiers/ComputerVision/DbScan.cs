using DataStructures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ComputerVision
{
    public static class DbScan
    {
        private static float Distance(Point2D a, Point2D b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static Cluster[] Cluster(LabeledTomogram tomogram, float epsilon, int minimumClusterSize)
        {
            List<Cluster> clusterCenters = new List<Cluster>();

            int currentClusterLabel = 1;
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
                    List<Point2D> cluster = new List<Point2D>();
                    foreach (Point2D otherPoint in points)
                    {
                        if (Distance(point, otherPoint) <= epsilon)
                        {
                            cluster.Add(otherPoint);
                        }
                    }

                    // is the seed cluster size big enough? 
                    if (cluster.Count >= minimumClusterSize)
                    {
                        // yes, expand outwards. 
                        for (int n = 0; n < cluster.Count; n++)
                        {
                            Point2D clusterPoint = cluster[n];
                            int neighborIndex = (int)(clusterPoint.Y * tomogram.Width + clusterPoint.X);

                            // was this seen as noise originally? 
                            if (clusterLabels[neighborIndex] == -1)
                            {
                                // yes, it's not noise afterall. 
                                clusterLabels[neighborIndex] = currentClusterLabel;
                            }
                            // does this already belong to another group? 
                            else if (clusterLabels[neighborIndex] > 0)
                            {
                                // yes, so drop out. look at next point. 
                                continue;
                            }
                            // must be unlabeled. 
                            else
                            {
                                // label it. 
                                clusterLabels[neighborIndex] = currentClusterLabel;
                            }

                            List<Point2D> toAdd = new List<Point2D>();
                            // find all this guy's neighbors. 
                            foreach (Point2D otherPoint in points)
                            {
                                int otherPointIndex = (int)otherPoint.Y * tomogram.Width + (int)otherPoint.X;
                                if (Distance(clusterPoint, otherPoint) <= epsilon)
                                {
                                    toAdd.Add(otherPoint);
                                }
                            }

                            // was this new cluster large enough? 
                            if (toAdd.Count >= minimumClusterSize)
                            {
                                // yes, add it to the bunch. 
                                cluster.AddRange(toAdd);
                            }
                        }

                        // we're done. create new label. 
                        currentClusterLabel++;
                    }
                    else
                    {
                        // no, it's noise
                        clusterLabels[index] = -1;
                    }
                }
            }

            for (int c = 1; c < currentClusterLabel; c++)
            {
                Cluster cluster = new Cluster();
                Point2D averagedPoint = new Point2D();
                int number = 0;

                for (int y = 0, i = 0; y < tomogram.Height; y++)
                {
                    for (int x = 0; x < tomogram.Width; x++, i++)
                    {
                        if (clusterLabels[i] == c)
                        {
                            averagedPoint.X += x;
                            averagedPoint.Y += y;

                            number++;

                            cluster.Members.Add(new Point2D { X = x, Y = y });
                        }
                    }
                }
                cluster.Center = new Point2D
                {
                    X = averagedPoint.X / (number * 1.0f),
                    Y = averagedPoint.Y / (number * 1.0f),
                };

                clusterCenters.Add(cluster);
            }

            return clusterCenters.ToArray();
        }
    }
}
