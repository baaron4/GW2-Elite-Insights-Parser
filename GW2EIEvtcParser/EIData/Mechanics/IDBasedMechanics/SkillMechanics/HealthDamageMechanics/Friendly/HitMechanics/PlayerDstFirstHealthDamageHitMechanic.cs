using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerDstFirstHealthDamageHitMechanic : PlayerDstHealthDamageHitMechanic
{
    protected override bool Keep(HealthDamageEvent c, ParsedEvtcLog log)
    {
        return !c.From.IsUnknown && base.Keep(c, log) && GetFirstHit(c.From, log) == c;
    }

    private readonly Dictionary<AgentItem, HealthDamageEvent?> _firstHits = [];

    public PlayerDstFirstHealthDamageHitMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public PlayerDstFirstHealthDamageHitMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    private HealthDamageEvent? GetFirstHit(AgentItem src, ParsedEvtcLog log)
    {
        if (!_firstHits.TryGetValue(src, out var healthEvt))
        {
            HealthDamageEvent? res = log.CombatData.GetDamageData(src).Where(x => MechanicIDs.Contains(x.SkillID) && x.To.Type == AgentItem.AgentType.Player && base.Keep(x, log)).FirstOrDefault();
            _firstHits[src] = res;
            return res;
        }
        return healthEvt;
    }
}
