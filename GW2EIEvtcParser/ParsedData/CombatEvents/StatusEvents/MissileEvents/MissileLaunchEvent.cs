using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Trigonometry;
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
        ev->is_statechange = CBTS_MISSILELAUNCH;
        ev->is_src_flanking = is_first_launch; // 1 on initial launch, 0 on re-launch/reflect
        *(uint16_t*)&ev->is_shields = float_to_int16_nonprecise(speed, 1.0f);
        *(uint32_t*)&ev->pad61 = trackable_id;
    */
    public bool LaunchedTowardsAgent => _targetedAgent != null && !_targetedAgent.IsNonIdentifiedSpecies();
    private readonly AgentItem? _targetedAgent = null;
    public AgentItem TargetedAgent => _targetedAgent ?? _unknownAgent;

    public readonly Vector3 TargetPosition;
    public readonly Vector3 LaunchPosition;

    public readonly ushort Speed;

    public readonly bool IsFirstLaunch;
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
                        positionsShorts[0] * 10,
                        positionsShorts[1] * 10,
                        positionsShorts[2] * 10
                    );
                LaunchPosition = new(
                        positionsShorts[3] * 10,
                        positionsShorts[4] * 10,
                        positionsShorts[5] * 10
                    );
            }
        }
        IsFirstLaunch = evtcItem.IsFlanking > 0;
        var speedBytes = new ByteBuffer(stackalloc byte[sizeof(short)]);
        speedBytes.PushNative(evtcItem.IsShields);
        speedBytes.PushNative(evtcItem.IsOffcycle);
        Speed = BitConverter.ToUInt16(speedBytes);
    }

}
