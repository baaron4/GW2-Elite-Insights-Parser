using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class SpawnMechanic : IDBasedMechanic<SingleActor>
{
    public SpawnMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([mechanicID], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public SpawnMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (AgentItem a in log.AgentData.GetStableSpeciesByID((int)mechanicID))
            {
                SingleActor? amp = MechanicHelper.FindEnemyActor(log, a, regroupedMobs);
                if (amp != null && Keep(amp, log))
                {
                    InsertMechanic(log, mechanicLogs, a.FirstAware, amp);
                }
            }
        }
    }
}
