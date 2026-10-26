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
    public const double Quickness = 1.0 / 0.67;
    public const double Slow = 1.0 / 1.5;

    private static int ToTickCount(int duration)
    {
        return (duration + Tick - 1) / Tick;
    }

    private static int ToTickTime(int duration)
    {
        return ToTickCount(duration) * Tick;
    }

    private static int ToScaledTickTime(int duration, double timeScale, Behavior behavior)
    {
        return ToTickTime(Scale(duration, timeScale, behavior));
    }

    private static int Scale(int duration, double timeScale, Behavior behavior)
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

    private static int ScaleRoundedAwayFromZero(int duration, double timeScale)
    {
        return (int)Math.Round(duration / timeScale, MidpointRounding.AwayFromZero);
    }

    private static int ScaleRoundedToEven(int duration, double timeScale)
    {
        return (int)Math.Round(duration / timeScale, MidpointRounding.ToEven);
    }

    private static int ScaleFloored(int duration, double timeScale)
    {
        return (int)(duration / timeScale);
    }

    private static int ScaleCeiled(int duration, double timeScale)
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

    public static double NormalizeCommonScales(double timeScale, double epsilon = 0.06)
    {
        if (Math.Abs(timeScale - 1.0) < epsilon)
        {
            return 1.0;
        }
        else if (Math.Abs(timeScale - Quickness) < epsilon)
        {
            return Quickness;
        }
        else if (Math.Abs(timeScale - Slow) < epsilon)
        {
            return Slow;
        }
        return timeScale;
    }
}
