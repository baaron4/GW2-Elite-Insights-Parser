namespace LuckParser.Models.ParseModels
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

        private static long Mix(long a, long b, float c)
        {
            return (long)((1.0f - c) * a + c * b);
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
    }
}