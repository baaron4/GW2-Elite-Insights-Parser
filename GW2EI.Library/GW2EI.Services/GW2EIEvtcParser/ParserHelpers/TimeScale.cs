namespace GW2EIEvtcParser.ParserHelpers;

public static class TimeScale
{
    public enum Behavior
    {
        Floor,
        RoundToEven,
        RoundAwayFromZero,
        Ceil,
    }

    public const int Tick = 40;

    public static int ToTickCount(int duration)
    {
        return (duration + Tick - 1) / Tick;
    }

    public static int ToTickTime(int duration)
    {
        return ToTickCount(duration) * Tick;
    }

    public static int ToScaledTickTime(int duration, double timeScale, Behavior behavior)
    {
        return ToTickTime(Scale(duration, timeScale, behavior));
    }

    public static int Scale(int duration, double timeScale, Behavior behavior)
    {
        return behavior switch
        {
            Behavior.Floor => ScaleFloored(duration, timeScale),
            Behavior.RoundAwayFromZero => ScaleRoundedAwayFromZero(duration, timeScale),
            Behavior.RoundToEven => ScaleRoundedToEven(duration, timeScale),
            Behavior.Ceil => ScaleCeiled(duration, timeScale),
            _ => throw new NotImplementedException(),
        };
    }

    public static int ScaleRoundedAwayFromZero(int duration, double timeScale)
    {
        return (int)Math.Round(duration / timeScale, MidpointRounding.AwayFromZero);
    }

    public static int ScaleRoundedToEven(int duration, double timeScale)
    {
        return (int)Math.Round(duration / timeScale, MidpointRounding.ToEven);
    }

    public static int ScaleFloored(int duration, double timeScale)
    {
        return (int)(duration / timeScale);
    }

    public static int ScaleCeiled(int duration, double timeScale)
    {
        return (int)Math.Ceiling(duration / timeScale);
    }

    public delegate int Scaling(int duration);

    public static int ApplyScaling(int duration, Scaling? scaling)
    {
        return scaling?.Invoke(duration) ?? duration;
    }

    public static Scaling GetTickScaling(double timeScale = 1.0, Behavior behavior = Behavior.RoundToEven)
    {
        return duration => ToScaledTickTime(duration, timeScale, behavior);
    }
}
