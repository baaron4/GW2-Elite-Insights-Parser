namespace GW2EIEvtcParser.EIData
{
    public class ParametricPoint2D : Point2D
    {
        public long Time { get; }


        public ParametricPoint2D(float x, float y, long time) : base(x, y)
        {
            Time = time;
        }

        public ParametricPoint2D(ParametricPoint2D a) : this(a.X, a.Y, a.Time)
        {
        }

        public ParametricPoint2D(Point2D a, long time) : base(a)
        {
            Time = time;
        }

        public ParametricPoint2D(Point2D a, Point2D b, float ratio, long time) : base(a, b, ratio)
        {
            Time = time;
        }
    }
}
