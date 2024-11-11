namespace GW2EIEvtcParser.EIData;

// Serialized as part of BackgroundIconRenderingDescription. Take care when changing shape.
public class ParametricPoint1D
{
    public readonly float X;
    public readonly long  Time;


    public ParametricPoint1D(float x, long time)
    {
        X = x;
        Time = time;
    }

    public ParametricPoint1D(ParametricPoint1D a) : this(a.X, a.Time)
    {
    }


    public ParametricPoint1D(float a, float b, float ratio, long time)
    {
        X = Value.Lerp(a, b, ratio);
        Time = time;
    }
}
