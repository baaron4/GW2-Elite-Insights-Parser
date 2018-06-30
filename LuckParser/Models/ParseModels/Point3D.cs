namespace LuckParser.Models.ParseModels
{
    public class Point3D
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Point3D(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }
}