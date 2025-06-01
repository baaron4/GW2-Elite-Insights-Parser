using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class DstMissileLaunchMechanic : IDBasedMechanic<MissileLaunchEvent>
{

    private bool _withMinions { get; set; } = false;

    public DstMissileLaunchMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public DstMissileLaunchMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public DstMissileLaunchMechanic WithMinions()
    {
        _withMinions = true;
        return this;
    }
    public DstMissileLaunchMechanic UsingReflected()
    {
        UsingChecker((x, log) => x.MaybeReflected);
        return this;
    }

    public DstMissileLaunchMechanic UsingNotReflected()
    {
        UsingChecker((x, log) => !x.MaybeReflected);
        return this;
    }
    protected static AgentItem GetAgentItem(MissileLaunchEvent missileLaunchEvent)
    {
        return missileLaunchEvent.TargetedAgent;
    }

    protected AgentItem GetCreditedAgentItem(MissileLaunchEvent missileLaunchEvent)
    {
        AgentItem? agentItem = GetAgentItem(missileLaunchEvent);
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
                foreach (MissileLaunchEvent missileLaunchEvent in missileEvent.LaunchEvents)
                {
                    if (missileLaunchEvent.HasTargetAgent && TryGetActor(log, GetCreditedAgentItem(missileLaunchEvent), regroupedMobs, out var amp) && Keep(missileLaunchEvent, log))
                    {
                        InsertMechanic(log, mechanicLogs, missileLaunchEvent.Time, amp);
                    }
                }
            }
        }
    }

}
