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

        public static float GetRotationFromFacing(Point2D facing)
        {
            return (float)Math.Round(ParserHelper.RadianToDegree(Math.Atan2(facing.Y, facing.X)), ParserHelper.CombatReplayDataDigit);
        }
    }
}
