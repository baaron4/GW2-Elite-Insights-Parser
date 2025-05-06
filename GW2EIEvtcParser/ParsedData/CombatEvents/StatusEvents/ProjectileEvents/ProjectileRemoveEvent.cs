using System.Numerics;
using static GW2EIEvtcParser.EIData.Trigonometry;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.ParsedData;

public class ProjectileRemoveEvent : TimeCombatEvent
{
    public readonly bool MaybeCollidedWithTerrain;
    public bool MaybeHitAnAgent => _guessedHitAgent != null;
    private readonly AgentItem? _guessedHitAgent = null;
    public AgentItem GuessedHitAgent => MaybeHitAnAgent ? _guessedHitAgent! : _unknownAgent;
    public ProjectileEvent Projectile { get; internal set; }
    internal ProjectileRemoveEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem.Time)
    {
        if (evtcItem.DstAgent != 0)
        {
            _guessedHitAgent = agentData.GetAgent(evtcItem.DstAgent, evtcItem.Time);
        }
        MaybeCollidedWithTerrain = evtcItem.IsShields > 0;
    }
}
