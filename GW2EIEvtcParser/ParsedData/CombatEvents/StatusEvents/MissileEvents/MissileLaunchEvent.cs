using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParsedData.MissileEvent;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class MissileLaunchEvent : TimeCombatEvent
{
    /*
        ev->src_agent = (uintptr_t)src_ag;
        ev->dst_agent = (uintptr_t)dst_ag;
        int16_t* i16 = (int16_t*)&ev->value;
        i16[0] = float_to_int16_nonprecise(xyz_target[0], 10.0f);
        i16[1] = float_to_int16_nonprecise(xyz_target[1], 10.0f);
        i16[2] = float_to_int16_nonprecise(xyz_target[2], 10.0f);
        i16[3] = float_to_int16_nonprecise(xyz_current[0], 10.0f);
        i16[4] = float_to_int16_nonprecise(xyz_current[1], 10.0f);
        i16[5] = float_to_int16_nonprecise(xyz_current[2], 10.0f);
        ev->skillid = skillid;
        *(uint8_t*)&ev->iff = launch_type;
        *(uint32_t*)&ev->is_buffremove = launch_flags;
        ev->is_statechange = CBTS_MISSILELAUNCH;
        ev->is_src_flanking = is_first_launch; // 1 on initial launch, 0 on re-launch/reflect
        *(int16_t*)&ev->result = float_to_int16_nonprecise(radius, 1.0f);
        *(uint16_t*)&ev->is_shields = float_to_int16_nonprecise(speed, 1.0f);
        *(uint32_t*)&ev->pad61 = trackable_id;
    */
    public bool HasTargetAgent => _targetedAgent != null && !_targetedAgent.IsNonIdentifiedSpecies();
    private readonly AgentItem? _targetedAgent = null;
    public AgentItem TargetedAgent => _targetedAgent ?? _unknownAgent;

    public readonly Vector3 TargetPosition;
    public readonly Vector3 LaunchPosition;

    public readonly float Speed;
    public readonly float MotionRadius;

    public readonly bool IsFirstLaunch;
    public readonly byte LaunchType;
    public readonly uint LaunchFlags;
    public bool MaybeReflected => Missile.Src == TargetedAgent && !IsFirstLaunch;
    public MissileEvent Missile { get; internal set; }
    internal MissileLaunchEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        if (evtcItem.DstAgent != 0)
        {
            _targetedAgent = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }
        var positionsBytes = new ByteBuffer(stackalloc byte[6 * sizeof(short)]);
        // 1 
        positionsBytes.PushNative(evtcItem.Value);
        // 1
        positionsBytes.PushNative(evtcItem.BuffDmg);
        // 1
        positionsBytes.PushNative(evtcItem.OverstackValue);
        unsafe
        {
            fixed (byte* ptr = positionsBytes.Span)
            {
                var positionsShorts = (short*)ptr;
                TargetPosition = new(
                        positionsShorts[0] * MissilePositionConvertConstant,
                        positionsShorts[1] * MissilePositionConvertConstant,
                        positionsShorts[2] * MissilePositionConvertConstant
                    );
                LaunchPosition = new(
                        positionsShorts[3] * MissilePositionConvertConstant,
                        positionsShorts[4] * MissilePositionConvertConstant,
                        positionsShorts[5] * MissilePositionConvertConstant
                    );
            }
        }
        IsFirstLaunch = evtcItem.IsFlanking > 0;

        var speedBytes = new ByteBuffer(stackalloc byte[sizeof(short)]);
        speedBytes.PushNative(evtcItem.IsShields);
        speedBytes.PushNative(evtcItem.IsOffcycle);
        Speed = BitConverter.ToInt16(speedBytes) * MissileSpeedConvertConstant;

        var radiusBytes = new ByteBuffer(stackalloc byte[sizeof(short)]);
        radiusBytes.PushNative(evtcItem.Result);
        radiusBytes.PushNative(evtcItem.IsActivationByte);
        MotionRadius = BitConverter.ToInt16(radiusBytes);

        var flagsBytes = new ByteBuffer(stackalloc byte[sizeof(uint)]);
        // 0.25 
        flagsBytes.PushNative(evtcItem.IsBuffRemoveByte);
        // 0.25 
        flagsBytes.PushNative(evtcItem.IsNinety);
        // 0.25 
        flagsBytes.PushNative(evtcItem.IsFifty);
        // 0.25 
        flagsBytes.PushNative(evtcItem.IsMoving);
        LaunchFlags = BitConverter.ToUInt32(flagsBytes);
        LaunchType = evtcItem.IFFByte;
    }
    /// <summary>
    /// Assumes that the missile goes from LaunchPosition to TargetPosition
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public ParametricPoint3D GetFinalPosition(long start, long end)
    {
        var velocity = Speed;
        var direction = (TargetPosition - LaunchPosition);
        direction /= Math.Max(direction.Length(), 1e-6f);
        return new ParametricPoint3D(LaunchPosition + (velocity * direction) * (end - start), end);
    }
    /// <summary>
    /// Assumes that the missile goes from LaunchPosition to TargetPosition
    /// </summary>
    /// <param name="lifespan"></param>
    /// <returns></returns>
    public ParametricPoint3D GetFinalPosition((long start, long end) lifespan)
    {
        return GetFinalPosition(lifespan.start, lifespan.end);
    }
    /// <summary>
    /// Assumes that the missile goes from LaunchPosition to TargetPosition
    /// </summary>
    /// <param name="lifespan"></param>
    /// <returns></returns>
    public ParametricPoint3D GetFinalPosition(Segment lifespan)
    {
        return GetFinalPosition(lifespan.Start, lifespan.End);
    }

}
