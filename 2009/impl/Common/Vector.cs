using System;

namespace ICFP2009.Common
{
    public class Vector
    {
        public Vector(double x, double y)
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

        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}", X, Y);
        }
    }
}