using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstFirstHealthDamageHitMechanic : PlayerDstHealthDamageHitMechanic
{
    protected override bool Keep(HealthDamageEvent c, ParsedEvtcLog log)
    {
        return !c.From.IsUnknown && base.Keep(c, log) && GetFirstHit(c.From, log) == c;
    }

    private readonly Dictionary<AgentItem, HealthDamageEvent?> _firstHits = [];

    public PlayerDstFirstHealthDamageHitMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerDstFirstHealthDamageHitMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    private HealthDamageEvent? GetFirstHit(AgentItem src, ParsedEvtcLog log)
    {
        if (!_firstHits.TryGetValue(src, out var healthEvt))
        {
            HealthDamageEvent? res = log.CombatData.GetDamageData(src).FirstOrDefault(x => MechanicIDs.Contains(x.SkillID) && x.To.Type == AgentItem.AgentType.Player && base.Keep(x, log));
            _firstHits[src] = res;
            return res;
        }
        return healthEvt;
    }
}
