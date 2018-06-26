namespace LuckParser.Models.ParseModels
{
    public class Point3D
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public Point3D(long X, long Y, long Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;

        }
    }
}