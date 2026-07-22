using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class EnemyBuffRemoveSingleMechanic : EnemyBuffRemoveMechanic<AbstractBuffRemoveEvent>
{

    public EnemyBuffRemoveSingleMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicID, id, plotlySetting, description, severity,0)
    {
    }

    public EnemyBuffRemoveSingleMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicIDs, id, plotlySetting, description, severity, 0)
    {
    }
    protected override bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor)
    {
        actor = MechanicHelper.FindEnemyActor(log, agentItem, regroupedMobs);
        return actor != null;
    }
    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (BuffEvent c in log.CombatData.GetBuffData(mechanicID))
            {
                if (c is AbstractBuffRemoveEvent abre && TryGetActor(log, GetAgentItem(abre), regroupedMobs, out var amp) && Keep(abre, log))
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
