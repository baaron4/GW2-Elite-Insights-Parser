using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;


internal abstract class SrcEffectMechanic : EffectMechanic
{

    protected override AgentItem GetAgentItemInternal(EffectEvent effectEvt, AgentData agentData)
    {
        if (effectEvt.Src.IsUnamedSpecies())
        {
            return agentData.GetNPCsByID(TargetID.Environment).First();
        }
        return effectEvt.Src;
    }

    public SrcEffectMechanic(GUID effect, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([ effect ], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public SrcEffectMechanic(ReadOnlySpan<GUID> effects, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effects, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
}
