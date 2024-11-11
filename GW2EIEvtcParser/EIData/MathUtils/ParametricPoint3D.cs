using System.Numerics;
using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.EIData;

//TODO i assume this is serialized, add note 
public class ParametricPoint3D(float x, float y, float z, long time)
{
    public readonly float X = x, Y = y, Z = z;
    public readonly long Time = time;

    public ParametricPoint3D(ParametricPoint3D a) : this(a.X, a.Y, a.Z, a.Time) //TODO(Rennorb) 
    {
    }

    public ParametricPoint3D(in Vector3 a, long time) : this(a.X, a.Y, a.Z, time)
    {
    }

    public ParametricPoint3D(in Vector3 a, in Vector3 b, float ratio, long time) : this(Vector3.Lerp(a, b, ratio), time)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 ExtractVector() => new(X, Y, Z); //TODO(Rennorb) use a vector field and use a custom serilizer
}
