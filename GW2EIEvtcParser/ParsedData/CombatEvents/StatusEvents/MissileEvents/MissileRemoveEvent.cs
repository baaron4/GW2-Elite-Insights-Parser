using System.Numerics;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.ParserHelper;

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
    internal MissileRemoveEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        if (evtcItem.SrcAgent != 0)
        {
            _damagingAgent = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        }
        DidHit = evtcItem.IsFlanking > 0;
        FriendlyFireTotalDamage = evtcItem.Value;
    }
}
