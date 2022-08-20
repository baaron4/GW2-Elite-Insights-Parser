namespace GW2EIEvtcParser.EIData
{
    internal class InterpolatedPositionConnector : PositionConnector
    {
        public InterpolatedPositionConnector(ParametricPoint3D prev, ParametricPoint3D next, int time) : base(prev)
        {
            if (prev != null && next != null)
            {
                long denom = next.Time - prev.Time;
                if (denom == 0)
                {
                    Position = prev;
                }
                else
                {
                    float ratio = (float)(time - prev.Time) / denom;
                    Position = new Point3D(prev, next, ratio);
                }
            }
            else
            {
                Position = prev ?? next;
            }
        }
    }
}
