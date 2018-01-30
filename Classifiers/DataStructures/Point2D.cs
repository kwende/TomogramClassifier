using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures
{
    public class Point2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static Point2D operator /(Point2D lhs, int rhs)
        {
            return new Point2D
            {
                X = lhs.X / (rhs * 1.0f),
                Y = lhs.Y / (rhs * 1.0f)
            };
        }
    }
}
