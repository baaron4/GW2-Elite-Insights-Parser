namespace GW2EIEvtcParser.EIData
{
    public class ParametricPoint1D : Point1D
    {
        public long Time { get; }


        public ParametricPoint1D(float x, long time) : base(x)
        {
            Time = time;
        }

        public ParametricPoint1D(ParametricPoint1D a) : this(a.X, a.Time)
        {
        }

        public ParametricPoint1D(Point1D a, long time) : base(a)
        {
            Time = time;
        }

        public ParametricPoint1D(Point1D a, Point1D b, float ratio, long time) : base(a, b, ratio)
        {
            Time = time;
        }
    }
}
