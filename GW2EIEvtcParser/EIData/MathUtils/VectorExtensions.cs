using System.Numerics;
using System.Runtime.CompilerServices;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

static class Value
{
    /// <summary> Linearly interpolates from a to b with a ratio from 0 to 1 </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float a, float b, float ratio)
        => a * (1.0f - ratio) + b * ratio;
}

static class Vector
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector2 XY(this Vector3 v)
        => *(Vector2*)&v;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct(this in Vector2 v, out float x, out float y)
    {
        x = v.X;
        y = v.Y;
    }

    /// <summary>
    /// Returns true if p is inside or on the edges the triangle defined by v0, v1 and v2
    /// Triangle can be clockwise or counter clock wise
    /// </summary>
    public static bool IsInTriangle(this in Vector2 p, Vector2 v0, Vector2 v1, Vector2 v2)
    {
        // barycentric coordinates
        // https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        // properly handles clockwise or counter clockwise
        var s = (v0.X - v2.X) * (p.Y - v2.Y) - (v0.Y - v2.Y) * (p.X - v2.X);
        var t = (v1.X - v0.X) * (p.Y - v0.Y) - (v1.Y - v0.Y) * (p.X - v0.X);

        if ((s < 0) != (t < 0) && s != 0 && t != 0)
        {
            return false;
        }

        var d = (v2.X - v1.X) * (p.Y - v1.Y) - (v2.Y - v1.Y) * (p.X - v1.X);
        return d == 0 || (d < 0) == (s + t <= 0);
    }

    /// <summary>
    /// Returns true if p in inside or on the edges of the triangle defined by vertices
    /// points must have exactly 3 values, returns false otherwise
    /// Triangle can be clockwise or counter clock wise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInTriangle(this in Vector2 p, ReadOnlySpan<Vector2> vertices)
        => vertices.Length == 3 && IsInTriangle(p, vertices[0], vertices[1], vertices[2]);
	/// <summary>
    /// Returns true if p in inside or on the edges of the triangle defined by vertices
    /// points must have exactly 3 values, returns false otherwise
    /// Triangle can be clockwise or counter clock wise
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInTriangle<L>(this in Vector2 p, L vertices) where L : IReadOnlyList<Vector2>
        => vertices.Count == 3 && IsInTriangle(p, vertices[0], vertices[1], vertices[2]);

    /// <returns> true if point p is within the inclusive rectangular bounds spanned by axis extremes v0 and v1. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInBoundingBox(this in Vector2 p, in Vector2 v0, in Vector2 v1)
        => Math.Min(v0.X, v1.X) <= p.X && p.X <= Math.Max(v0.X, v0.X)
           && Math.Min(v0.Y, v1.Y) <= p.Y && p.Y <= Math.Max(v0.Y, v0.Y);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct(this in Vector3 v, out float x, out float y, out float z)
    {
        x = v.X;
        y = v.Y;
        z = v.Z;
    }

    /// <returns> true if point p is within the inclusive rectangular bounds spanned by the x/y axis extremes of v0 and v1. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInBoundingBoxXY(this in Vector3 p, in Vector3 p0, in Vector3 p1)
        => IsInBoundingBox(p.XY(), p0.XY(), p1.XY());

    /// <returns> true if point p is within the inclusive cubic bounds spanned by axis extremes v0 and v1. </returns>
    public static bool IsInBoundingBox(this in Vector3 p, in Vector3 v0, in Vector3 v1)
        => Math.Min(v0.X, v1.X) <= p.X && p.X <= Math.Max(v0.X, v0.X)
           && Math.Min(v0.Y, v1.Y) <= p.Y && p.Y <= Math.Max(v0.Y, v0.Y)
           && Math.Min(v0.Z, v1.Z) <= p.Z && p.Z <= Math.Max(v0.Z, v0.Z);
    
    public static float ScalarProduct(in Vector2 pt1, in Vector2 pt2)
    {
        return pt1.X * pt2.X + pt1.Y * pt2.Y;
    }
    public static float ScalarProduct(in Vector3 pt1, in Vector3 pt2)
    {
        return pt1.X * pt2.X + pt1.Y * pt2.Y + pt1.Z * pt2.Z;
    }
    public static Vector2 ProjectPointOnLine(this in Vector2 toProject, in Vector2 pointOnLine, in Vector2 directionVector)
    {
        var normalizedDirectionVector = Vector2.Normalize(directionVector);
        var vectorToProject = toProject - pointOnLine;
        return ScalarProduct(vectorToProject, normalizedDirectionVector) * normalizedDirectionVector + pointOnLine;
    }

    public static Vector3 ProjectPointOnLine(this in Vector3 toProject, in Vector3 pointOnLine, in Vector3 directionVector)
    {
        var normalizedDirectionVector = Vector3.Normalize(directionVector);
        var vectorToProject = toProject - pointOnLine;
        return ScalarProduct(vectorToProject, normalizedDirectionVector) * normalizedDirectionVector + pointOnLine;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ProjectPointOn2DLine(this in Vector3 toProject, in Vector3 pointOnLine, in Vector3 directionVector) => new (toProject.XY().ProjectPointOnLine(pointOnLine.XY(), directionVector.XY()), pointOnLine.Z);
    public static Vector2 RotatePointAroundPoint(in Vector2 pivotPoint, in Vector2 rotationPoint, double angle)
    {
        double sin = Math.Sin(angle);
        double cos = Math.Cos(angle);
        double deltaX = rotationPoint.X - pivotPoint.X;
        double deltaY = rotationPoint.Y - pivotPoint.Y;
        double x = deltaX * cos - deltaY * sin + pivotPoint.X;
        double y = deltaX * sin + deltaY * cos + pivotPoint.Y;
        return new Vector2((float)x, (float)y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 RotatePointAroundPoint(in Vector3 pivotPoint, in Vector3 rotationPoint, double angle) => new (RotatePointAroundPoint(pivotPoint.XY(), rotationPoint.XY(), angle), 0);

    /// <summary> Calculates the average of a point-cloud. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Average(this IReadOnlyList<Vector3> points)
        => points.Aggregate(Vector3.Zero, static (sum, p) => sum + p) / points.Count;
    /// <summary> Calculates the average of a point-cloud. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Average(this in ReadOnlySpan<Vector3> points)
    {
        var sum = Vector3.Zero;
        foreach(var point in points)
        {
            sum += point;
        }
        return sum / points.Length;
    }

    public static float GetRoundedZRotationDeg(this in Vector3 facing)
    {
        return (float)Math.Round(RadianToDegree(Math.Atan2(facing.Y, facing.X)), CombatReplayDataDigit);
    }
}
