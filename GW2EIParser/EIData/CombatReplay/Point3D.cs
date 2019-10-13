using System;

namespace GW2EIParser.EIData
{
    public class Point3D
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public long Time { get; }

        private static float Mix(float a, float b, float c)
        {
            return (1.0f - c) * a + c * b;
        }

        public float DistanceToPoint(Point3D endPoint)
        {
            float distance = (float)Math.Sqrt((endPoint.X - X) * (endPoint.X - X) + (endPoint.Y - Y) * (endPoint.Y - Y) + (endPoint.Z - Z) * (endPoint.Z - Z));
            return distance;
        }

        public Point3D(float x, float y, float z, long time)
        {
            X = x;
            Y = y;
            Z = z;
            Time = time;
        }

        public Point3D(Point3D a, Point3D b, float ratio, long time)
        {
            X = Mix(a.X, b.X, ratio);
            Y = Mix(a.Y, b.Y, ratio);
            Z = Mix(a.Z, b.Z, ratio);
            Time = time;
        }

        public static int GetRotationFromFacing(Point3D facing)
        {
            int rotation = (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI);
            return rotation;
        }
    }
}
