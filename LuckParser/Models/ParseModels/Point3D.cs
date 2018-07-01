using System;

namespace LuckParser.Models.ParseModels
{
    public class Point3D
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public long time { get; }

        private static float Mix(float a, float b, float c)
        {
            return (1.0f - c) * a + c * b;
        }

        private static long Mix(long a, long b, float c)
        {
            return (long)((1.0f - c) * a + c * b);
        }

        public Point3D(float X, float Y, float Z, long time)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.time = time;
        }

        public Point3D(Point3D a, Point3D b, float ratio, long time)
        {
            X = Mix(a.X, b.X, ratio);
            Y = Mix(a.Y, b.Y, ratio);
            Z = Mix(a.Z, b.Z, ratio);
            this.time = time;
        }
    }
}