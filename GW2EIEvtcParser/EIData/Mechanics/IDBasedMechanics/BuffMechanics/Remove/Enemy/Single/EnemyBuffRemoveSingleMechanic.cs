using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class EnemyBuffRemoveSingleMechanic : EnemyBuffRemoveMechanic<AbstractBuffRemoveEvent>
{

    public EnemyBuffRemoveSingleMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, 0)
    {
    }

    public EnemyBuffRemoveSingleMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, 0)
    {
    }
    protected override SingleActor? GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs)
    {
        return MechanicHelper.FindEnemyActor(log, agentItem, regroupedMobs);
    }
    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (BuffEvent c in log.CombatData.GetBuffData(mechanicID))
            {
                if (c is AbstractBuffRemoveEvent abre && Keep(abre, log))
                {
                    SingleActor? amp = GetActor(log, GetAgentItem(abre), regroupedMobs);
                    if (amp != null)
                    {
                        if (abre is BuffRemoveAllEvent brae)
                        {
                            for (int i = 0; i < brae.RemovedStacks; i++)
                            {
                                AddMechanic(log, mechanicLogs, brae, amp);
                            }
                        }
                        else if (abre is BuffRemoveSingleEvent brse)
                        {
                            AddMechanic(log, mechanicLogs, brse, amp);
                        }
                    }
                }
            }
        }
    }

}
