using System;

namespace ICFP2009.Common
{
    public class Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X
        {
            get; set;
        }

        public double Y
        {
            get; set;
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}", X, Y);
        }

        public Point Clone()
        {
            Point cloned = new Point(X, Y);
            return cloned;
        }
    }
}