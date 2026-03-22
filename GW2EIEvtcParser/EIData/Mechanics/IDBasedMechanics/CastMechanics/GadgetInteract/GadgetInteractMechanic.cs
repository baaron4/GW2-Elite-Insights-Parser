using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class GadgetInteractMechanic : CastMechanic
{
    public GadgetInteractMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public GadgetInteractMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (long mechanicID in MechanicIDs)
        {
            foreach (GadgetInteractEvent c in log.CombatData.GetGadgetInteractCastDataByGadgetSpeciesID(mechanicID))
            {
                if (TryGetActor(log, c.Caster, regroupedMobs, out var amp) && Keep(c, log))
                {
                    InsertMechanic(log, mechanicLogs, GetTime(c), amp, c.ActualDuration);
                }
            }
        }

    }

}
