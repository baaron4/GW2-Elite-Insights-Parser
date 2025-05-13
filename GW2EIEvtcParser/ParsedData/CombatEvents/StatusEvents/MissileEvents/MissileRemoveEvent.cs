using System.Numerics;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class MissileRemoveEvent : TimeCombatEvent
{
    public readonly bool MaybeCollidedWithTerrain;
    public bool DidDamage => _damagedAgent != null && _damagingAgent != null;
    private readonly AgentItem? _damagedAgent = null;
    public AgentItem DamagedAgent => DidDamage ? _damagedAgent! : _unknownAgent;

    private readonly AgentItem? _damagingAgent = null;
    public AgentItem DamagingAgent => DidDamage ? _damagingAgent! : _unknownAgent;
    public MissileEvent Missile { get; internal set; }
    internal MissileRemoveEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        if (evtcItem.SrcAgent != 0)
        {
            _damagingAgent = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.Time);
        }
        if (evtcItem.DstAgent != 0)
        {
            _damagedAgent = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }
        MaybeCollidedWithTerrain = evtcItem.IsShields > 0;
    }
}
