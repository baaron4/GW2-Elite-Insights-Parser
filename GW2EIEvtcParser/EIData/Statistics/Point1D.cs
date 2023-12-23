using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class Point1D
    {
        public float X { get; private set; }

        private static float Mix(float a, float b, float c)
        {
            return (1.0f - c) * a + c * b;
        }

        public static Point1D operator +(Point1D a, Point1D b)
        {
            var newPt = new Point1D(a);
            newPt.Add(b);
            return newPt;
        }

        public static Point1D operator -(Point1D a, Point1D b)
        {
            var newPt = new Point1D(a);
            newPt.Substract(b);
            return newPt;
        }
        public static Point1D operator *(Point1D a, Point1D b)
        {
            var newPt = new Point1D(a);
            newPt.Multiply(b);
            return newPt;
        }
        public static Point1D operator *(float a, Point1D b)
        {
            var newPt = new Point1D(b);
            newPt.MultiplyScalar(a);
            return newPt;
        }
        public static Point1D operator *(Point1D a, float b)
        {
            var newPt = new Point1D(a);
            newPt.MultiplyScalar(b);
            return newPt;
        }
        public static Point1D operator -(Point1D a)
        {
            var newPt = new Point1D(a);
            newPt.MultiplyScalar(-1);
            return newPt;
        }

        private void Add(Point1D a)
        {
            X += a.X;
        }
        private void Substract(Point1D a)
        {
            X -= a.X;
        }
        private void Multiply(Point1D a)
        {
            X *= a.X;
        }
        private void MultiplyScalar(float a)
        {
            X *= a;
        }

        public float DistanceToPoint(Point1D endPoint)
        {
            float distance = (float)Math.Sqrt((endPoint.X - X) * (endPoint.X - X));
            return distance;
        }

        public float Length()
        {
            float length = (float)Math.Sqrt(X * X);
            return length;
        }

        public Point1D(float x)
        {
            X = x;
        }

        public Point1D(Point1D a) : this(a.X)
        {
        }

        public Point1D(Point2D a) : this(a.X)
        {
        }

        public Point1D(Point3D a) : this(a.X)
        {
        }


        public Point1D(Point1D a, Point1D b, float ratio)
        {
            X = Mix(a.X, b.X, ratio);
        }
    }
}
