namespace GW2EIEvtcParser.EIData;

public static class Trigonometry
{
    internal static double RadianToDegree(double radian)
    {
        return radian * 180.0 / Math.PI;
    }

    internal static float RadianToDegreeF(double radian)
    {
        return (float)RadianToDegree(radian);
    }

    internal static double DegreeToRadian(double degree)
    {
        return degree * Math.PI / 180.0;
    }

    internal static float DegreeToRadianF(double degree)
    {
        return (float)DegreeToRadian(degree);
    }

    /// <summary>
    /// Given a <paramref name="degree"/>, calculates the <see cref="Math.Cos(double)"/> and <see cref="Math.Sin(double)"/> values.
    /// </summary>
    /// <param name="degree"></param>
    /// <returns>(<see cref="float"/>, <see cref="float"/>) tuple containing the resulting X and Y values.</returns>
    internal static (float, float) DegreeToRadiansTrigonometricF(double degree)
    {
        return ((float, float))DegreeToRadiansTrigonometric(degree);
    }

    /// <summary>
    /// Given a <paramref name="degree"/>, calculates the <see cref="Math.Cos(double)"/> and <see cref="Math.Sin(double)"/> values.
    /// </summary>
    /// <param name="degree"></param>
    /// <returns>(<see cref="double"/>, <see cref="double"/>) tuple containing the resulting X and Y values.</returns>
    internal static (double, double) DegreeToRadiansTrigonometric(double degree)
    {
        double x = Math.Cos(DegreeToRadian(degree));
        double y = Math.Sin(DegreeToRadian(degree));
        return (x, y);
    }
}
