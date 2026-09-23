namespace GW2EIEvtcParser.EIData;

// Serialized as part of BackgroundIconRenderingDescription. Take care when changing shape.
public class ParametricPoint1D(float x, long time)
{
    public readonly float X = x;
    public readonly long Time = time;

    public ParametricPoint1D(ParametricPoint1D a) : this(a.X, a.Time)
    {
    }


    public ParametricPoint1D(float a, float b, float ratio, long time) : this(Value.Lerp(a, b, ratio), time)
    {
    }
}
