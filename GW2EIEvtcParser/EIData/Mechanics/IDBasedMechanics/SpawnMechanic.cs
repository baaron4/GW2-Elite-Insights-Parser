using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class SpawnMechanic : IDBasedMechanic<SingleActor>
{
    public SpawnMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public SpawnMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (AgentItem a in log.AgentData.GetNPCsByID((int)mechanicID))
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
