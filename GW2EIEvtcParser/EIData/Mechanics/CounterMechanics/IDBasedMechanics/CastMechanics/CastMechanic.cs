using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class CastMechanic : IDBasedMechanic<CastEvent>
{

    protected abstract long GetTime(CastEvent evt);

    protected abstract bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor);

    public CastMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public CastMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (CastEvent c in log.CombatData.GetAnimatedCastData(mechanicID))
            {
                if (TryGetActor(log, c.Caster, regroupedMobs, out var amp) && Keep(c, log))
                {
                    InsertMechanic(log, mechanicLogs, GetTime(c), amp);
                }
            }
        }

    }

}
