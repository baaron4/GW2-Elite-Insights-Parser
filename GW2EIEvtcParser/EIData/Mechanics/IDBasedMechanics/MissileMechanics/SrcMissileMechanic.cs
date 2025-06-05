using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class SrcMissileMechanic : IDBasedMechanic<MissileEvent>
{

    private bool _withMinions { get; set; } = false;

    public SrcMissileMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public SrcMissileMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public SrcMissileMechanic WithMinions()
    {
        _withMinions = true;
        return this;
    }

    public SrcMissileMechanic UsingReflected()
    {
        UsingChecker((x, log) => x.MaybeReflected);
        return this;
    }

    public SrcMissileMechanic UsingNotReflected()
    {
        UsingChecker((x, log) => !x.MaybeReflected);
        return this;
    }

    protected static AgentItem GetAgentItem(MissileEvent missileEvent)
    {
        return missileEvent.Src;
    }

    protected AgentItem GetCreditedAgentItem(MissileEvent missileEvent)
    {
        AgentItem agentItem = GetAgentItem(missileEvent);
        if (_withMinions)
        {
            agentItem = agentItem.GetFinalMaster();
        }
        return agentItem!;
    }
    protected abstract bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor);

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long missileSkillID in MechanicIDs)
        {
            foreach (MissileEvent missileEvent in log.CombatData.GetMissileEventsBySkillID(missileSkillID))
            {
                if (TryGetActor(log, GetCreditedAgentItem(missileEvent), regroupedMobs, out var amp) && Keep(missileEvent, log))
                {
                    InsertMechanic(log, mechanicLogs, missileEvent.Time, amp);
                }
            }
        }
    }

}
