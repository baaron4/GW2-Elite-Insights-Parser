using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class Point2D
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        private static float Mix(float a, float b, float c)
        {
            return (1.0f - c) * a + c * b;
        }

        public static Point2D operator +(Point2D a, Point2D b)
        {
            var newPt = new Point2D(a);
            newPt.Add(b);
            return newPt;
        }

        public static Point2D operator -(Point2D a, Point2D b)
        {
            var newPt = new Point2D(a);
            newPt.Substract(b);
            return newPt;
        }
        public static Point2D operator *(Point2D a, Point2D b)
        {
            var newPt = new Point2D(a);
            newPt.Multiply(b);
            return newPt;
        }
        public static Point2D operator *(float a, Point2D b)
        {
            var newPt = new Point2D(b);
            newPt.MultiplyScalar(a);
            return newPt;
        }
        public static Point2D operator *(Point2D a, float b)
        {
            var newPt = new Point2D(a);
            newPt.MultiplyScalar(b);
            return newPt;
        }
        public static Point2D operator -(Point2D a)
        {
            var newPt = new Point2D(a);
            newPt.MultiplyScalar(-1);
            return newPt;
        }

        private void Add(Point2D a)
        {
            X += a.X;
            Y += a.Y;
        }
        private void Substract(Point2D a)
        {
            X -= a.X;
            Y -= a.Y;
        }
        private void Multiply(Point2D a)
        {
            X *= a.X;
            Y *= a.Y;
        }
        private void MultiplyScalar(float a)
        {
            X *= a;
            Y *= a;
        }

        public float DistanceToPoint(Point2D endPoint)
        {
            float distance = (float)Math.Sqrt((endPoint.X - X) * (endPoint.X - X) + (endPoint.Y - Y) * (endPoint.Y - Y));
            return distance;
        }

        public float Length()
        {
            float length = (float)Math.Sqrt(X * X + Y * Y);
            return length;
        }

        public Point2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point2D(Point2D a) : this(a.X, a.Y)
        {
        }

        public Point2D(Point3D a) : this(a.X, a.Y)
        {
        }


        public Point2D(Point2D a, Point2D b, float ratio)
        {
            X = Mix(a.X, b.X, ratio);
            Y = Mix(a.Y, b.Y, ratio);
        }

        public static float GetZRotationFromFacing(Point2D facing)
        {
            return (float)Math.Round(ParserHelper.RadianToDegree(Math.Atan2(facing.Y, facing.X)), ParserHelper.CombatReplayDataDigit);
        }
        /// <summary>
        /// Returs true if p is inside or on the edges the triangle defined by p0, p1 and p2
        /// Triangle can be clockwise or counter clock wise
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool IsInTriangle(Point2D p, Point2D p0, Point2D p1, Point2D p2)
        {
            // barycentric coordinates
            // https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
            // properly handles clockwise or counter clockwise
            var s = (p0.X - p2.X) * (p.Y - p2.Y) - (p0.Y - p2.Y) * (p.X - p2.X);
            var t = (p1.X - p0.X) * (p.Y - p0.Y) - (p1.Y - p0.Y) * (p.X - p0.X);

            if ((s < 0) != (t < 0) && s != 0 && t != 0)
            {
                return false;
            }

            var d = (p2.X - p1.X) * (p.Y - p1.Y) - (p2.Y - p1.Y) * (p.X - p1.X);
            return d == 0 || (d < 0) == (s + t <= 0);
        }

        /// <summary>
        /// Returns true if p in inside or on the edges of the triangle defined by points
        /// points must have exactly 3 values, returns false otherwise
        /// Triangle can be clockwise or counter clock wise
        /// </summary>
        /// <param name="p"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static bool IsInTriangle2D(Point2D p, IReadOnlyList<Point2D> points)
        {
            return points.Count == 3 && IsInTriangle(p, points[0], points[1], points[2]);
        }
    }
}
