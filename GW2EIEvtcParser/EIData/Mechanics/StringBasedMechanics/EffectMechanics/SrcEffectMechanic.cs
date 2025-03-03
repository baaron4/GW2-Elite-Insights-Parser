using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;


internal abstract class SrcEffectMechanic : EffectMechanic
{

    protected override AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData)
    {
        if (effectEvt.Src.IsUnamedSpecies())
        {
            return agentData.GetNPCsByID(TrashID.Environment).FirstOrDefault()!;
        }
        return effectEvt.Src;
    }

    public SrcEffectMechanic(GUID effect, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([ effect ], inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public SrcEffectMechanic(ReadOnlySpan<GUID> effects, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effects, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
}
