namespace LuckParser.Models.ParseModels
{
    public class Point3D
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public long time { get; }

        public Point3D(float X, float Y, float Z, long time)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.time = time;
        }
    }
}