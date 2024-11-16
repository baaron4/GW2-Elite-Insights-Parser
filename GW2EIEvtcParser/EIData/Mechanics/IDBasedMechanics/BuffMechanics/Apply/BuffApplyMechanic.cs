using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class BuffApplyMechanic : IDBasedMechanic<BuffApplyEvent>
{

    public BuffApplyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public BuffApplyMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }



    protected abstract AgentItem GetAgentItem(BuffApplyEvent ba);
    protected abstract SingleActor? GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs);

    protected virtual void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffApplyEvent ba, SingleActor actor)
    {
        InsertMechanic(log, mechanicLogs, ba.Time, actor);
    }


    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (BuffEvent c in log.CombatData.GetBuffData(mechanicID))
            {
                if (c is BuffApplyEvent ba && Keep(ba, log))
                {
                    SingleActor? amp = GetActor(log, GetAgentItem(ba), regroupedMobs);
                    if (amp != null)
                    {
                        AddMechanic(log, mechanicLogs, ba, amp);
                    }
                }
            }
        }
    }
}
