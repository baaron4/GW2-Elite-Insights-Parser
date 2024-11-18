using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class CastMechanic : IDBasedMechanic<CastEvent>
{

    protected abstract long GetTime(CastEvent evt);

    protected abstract SingleActor? GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs);

    public CastMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public CastMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (CastEvent c in log.CombatData.GetAnimatedCastData(mechanicID))
            {
                if (Keep(c, log))
                {
                    SingleActor? amp = GetActor(log, c.Caster, regroupedMobs);
                    if (amp != null)
                    {
                        InsertMechanic(log, mechanicLogs, GetTime(c), amp);
                    }
                }
            }
        }

    }

}
