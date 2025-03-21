using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class SkillMechanic : IDBasedMechanic<HealthDamageEvent>
{

    protected bool Minions { get; private set; } = false;

    public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public SkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public SkillMechanic WithMinions(bool withMinions)
    {
        Minions = withMinions;
        return this;
    }

    protected abstract AgentItem GetAgentItem(HealthDamageEvent ahde);

    protected AgentItem GetCreditedAgentItem(HealthDamageEvent ahde)
    {
        AgentItem agentItem = GetAgentItem(ahde);
        if (Minions && agentItem != null)
        {
            agentItem = agentItem.GetFinalMaster();
        }
        return agentItem!;
    }

    protected abstract SingleActor? GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs);

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long skillID in MechanicIDs)
        {
            foreach (HealthDamageEvent ahde in log.CombatData.GetDamageData(skillID))
            {
                SingleActor? amp = GetActor(log, GetCreditedAgentItem(ahde), regroupedMobs);
                if (amp != null && Keep(ahde, log))
                {
                    InsertMechanic(log, mechanicLogs, ahde.Time, amp);
                }
            }
        }
    }

}
