using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;


internal abstract class DstEffectMechanic : EffectMechanic
{

    protected override AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData)
    {
        if (!effectEvt.IsAroundDst || effectEvt.Dst.IsUnamedSpecies())
        {
            return agentData.GetStableSpeciesByID(TargetID.Environment).First();
        }
        return effectEvt.Dst;
    }

    public DstEffectMechanic(GUID effectGUID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : this([ effectGUID ], plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }

    public DstEffectMechanic(ReadOnlySpan<GUID> effects, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, MechanicSeverity severity, int internalCoolDown) : base(effects, plotlySetting, shortName, description, fullName, severity, internalCoolDown)
    {
    }
}
