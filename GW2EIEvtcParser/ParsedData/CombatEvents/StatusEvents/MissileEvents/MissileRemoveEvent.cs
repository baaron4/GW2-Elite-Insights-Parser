using System.Numerics;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParsedData.MissileEvent;

namespace GW2EIEvtcParser.ParsedData;

public class MissileRemoveEvent : TimeCombatEvent
{
    /*
        ev->src_agent = (uintptr_t)src_ag;
        ev->value = friendly_fire; // ff sum 
        ev->skillid = skillid;
        ev->is_statechange = CBTS_MISSILEREMOVE;
        ev->is_src_flanking = did_hit; // hit an enemy
        *(uint32_t*)&ev->pad61 = trackable_id; 
    */
    public readonly bool DidHit;

    private readonly AgentItem? _damagingAgent = null;
    public AgentItem DamagingAgent => _damagingAgent ?? _unknownAgent;
    public MissileEvent Missile { get; internal set; }

    public readonly int FriendlyFireTotalDamage;
    public readonly Vector3 Position;
    internal MissileRemoveEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        if (evtcItem.SrcAgent != 0)
        {
            _damagingAgent = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        }
        DidHit = evtcItem.IsFlanking > 0;
        FriendlyFireTotalDamage = evtcItem.Value;

        var removePositionBytes = new ByteBuffer(stackalloc byte[4 * sizeof(short)]);
        // 1
        removePositionBytes.PushNative(evtcItem.BuffDmg);
        // 1 
        removePositionBytes.PushNative(evtcItem.OverstackValue);
        unsafe
        {
            fixed (byte* ptr = removePositionBytes.Span)
            {
                var removePositionShorts = (short*)ptr;
                Position = new(
                        removePositionShorts[0] * MissilePositionConvertConstant,
                        removePositionShorts[1] * MissilePositionConvertConstant,
                        removePositionShorts[2] * MissilePositionConvertConstant
                    );
            }
        }
    }
}
