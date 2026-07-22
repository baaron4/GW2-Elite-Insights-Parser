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

    public DstEffectMechanic(GUID effectGUID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([ effectGUID ], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public DstEffectMechanic(ReadOnlySpan<GUID> effects, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(effects, id, plotlySetting, description, severity, internalCoolDown)
    {
    }
}
