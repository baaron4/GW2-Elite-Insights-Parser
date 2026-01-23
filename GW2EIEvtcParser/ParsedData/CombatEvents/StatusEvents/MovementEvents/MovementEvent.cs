using System.Numerics;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public abstract class MovementEvent : StatusEvent
{
    private readonly ulong _dstAgent;
    private readonly int _value;

    internal MovementEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        _dstAgent = evtcItem.DstAgent;
        _value = evtcItem.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (ulong, int) PackMovementData(float x, float y, float z)
    {
        var xBytes = BitConverter.GetBytes(x);
        var yBytes = BitConverter.GetBytes(y);
        byte[] xyBytes = [.. xBytes, .. yBytes];
        ulong packXY = BitConverter.ToUInt64(xyBytes);
        return new(packXY, BitConverter.SingleToInt32Bits(z));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector3 UnpackMovementData(ulong packedXY, int intZ)
    {
        return new(*(float*)&packedXY, *((float*)&packedXY + 1), *(float*)&intZ);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParametricPoint3D GetParametricPoint3D()
    {
        var p = UnpackMovementData(_dstAgent, _value); //TODO(Rennorb) @cleanup: use union for event data to not have to do this kind of stuff
        return new ParametricPoint3D(p, Time);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetPoint3D()
    {
        return UnpackMovementData(_dstAgent, _value); //TODO(Rennorb) @cleanup: use union for event data to not have to do this kind of stuff
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Vector2 GetPointXY()
    {
        var packedXY = _dstAgent;
        return new(*(float*)&packedXY, *((float*)&packedXY + 1)); //TODO(Rennorb) @cleanup: use union for event data to not have to do this kind of stuff
    }

    /// <summary>
    /// Uses <see cref="UnpackMovementData(ulong, int)"/> to get X, Y, Z coordinates.<br></br>
    /// Converts the coordinate points to a <see cref="Point3D"/> to access the class methods.
    /// </summary>
    /// <param name="evt">CombatItem</param>
    /// <returns><see cref="Point3D"/> containing coordinates obtained from <paramref name="evt"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Vector3 GetPoint3D(CombatItem evt)
    {
        return UnpackMovementData(evt.DstAgent, evt.Value); //TODO(Rennorb) @cleanup: use union for event data to not have to do this kind of stuff
    }

    /// <summary>
    /// Uses <see cref="UnpackMovementData(ulong, int)"/> to get X, Y, Z coordinates.<br></br>
    /// Converts the coordinate points to a <see cref="Point3D"/> to access the class methods.
    /// </summary>
    /// <param name="evt">CombatItem</param>
    /// <returns><see cref="Point3D"/> containing coordinates obtained from <paramref name="evt"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static unsafe Vector2 GetPointXY(CombatItem evt)
    {
        var packedXY = evt.DstAgent;
        return new(*(float*)&packedXY, *((float*)&packedXY + 1)); //TODO(Rennorb) @cleanup: use union for event data to not have to do this kind of stuff
    }

    internal abstract void AddPoint3D(CombatReplay replay);
}
