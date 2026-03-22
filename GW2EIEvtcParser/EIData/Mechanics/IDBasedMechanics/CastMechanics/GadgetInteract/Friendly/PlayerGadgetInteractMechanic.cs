using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal abstract class PlayerGadgetInteractMechanic : GadgetInteractMechanic
{
    public PlayerGadgetInteractMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([mechanicID], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public PlayerGadgetInteractMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }
    protected override bool TryGetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, SingleActor> regroupedMobs, [NotNullWhen(true)] out SingleActor? actor)
    {
        actor = MechanicHelper.FindPlayerActor(log, agentItem);
        return actor != null;
    }
}
