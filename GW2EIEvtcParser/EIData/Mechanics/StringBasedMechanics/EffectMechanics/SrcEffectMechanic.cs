using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;


internal abstract class SrcEffectMechanic : EffectMechanic
{

    protected override AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData)
    {
        if (effectEvt.Src.IsUnamedSpecies())
        {
            return agentData.GetStableSpeciesByID(TargetID.Environment).First();
        }
        return effectEvt.Src;
    }

    public SrcEffectMechanic(GUID effect, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : this([ effect ], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public SrcEffectMechanic(ReadOnlySpan<GUID> effects, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(effects, id, plotlySetting, description, severity, internalCoolDown)
    {
    }
}
