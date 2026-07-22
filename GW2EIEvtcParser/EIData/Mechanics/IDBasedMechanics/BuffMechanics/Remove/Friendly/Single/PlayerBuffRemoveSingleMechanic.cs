using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class PlayerBuffRemoveSingleMechanic : PlayerBuffRemoveMechanic<AbstractBuffRemoveEvent>
{
    public PlayerBuffRemoveSingleMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicID, id, plotlySetting, description, severity, 0)
    {
    }

    public PlayerBuffRemoveSingleMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity) : base(mechanicIDs, id, plotlySetting, description, severity, 0)
    {
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
