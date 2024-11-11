using System.Numerics;

namespace GW2EIEvtcParser.EIData;

public class ParametricPoint2D(float x, float y, long time)
{
    public readonly float X = x, Y = y;
    public readonly long Time = time;


    public ParametricPoint2D(ParametricPoint2D a) : this(a.X, a.Y, a.Time)
    {
    }

    public ParametricPoint2D(in Vector2 a, long time) : this(a.X, a.Y, time)
    {
    }

    public ParametricPoint2D(in Vector2 a, in Vector2 b, float ratio, long time) : this(Vector2.Lerp(a, b, ratio), time)
    {
    }
}
