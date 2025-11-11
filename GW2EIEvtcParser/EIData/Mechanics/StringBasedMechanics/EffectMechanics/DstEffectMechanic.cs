using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;


internal abstract class DstEffectMechanic : EffectMechanic
{

    protected override AgentItem GetAgentItemInternal(EffectEvent effectEvt, AgentData agentData)
    {
        if (!effectEvt.IsAroundDst || effectEvt.Dst.IsUnamedSpecies())
        {
            return agentData.GetNPCsByID(TargetID.Environment).First();
        }
        return effectEvt.Dst;
    }

    public DstEffectMechanic(GUID effectGUID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([ effectGUID ], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public DstEffectMechanic(ReadOnlySpan<GUID> effects, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effects, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
}
