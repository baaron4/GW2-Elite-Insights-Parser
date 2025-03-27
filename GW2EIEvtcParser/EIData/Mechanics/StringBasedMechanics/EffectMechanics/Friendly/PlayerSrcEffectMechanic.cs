using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcEffectMechanic : SrcEffectMechanic
{

    public PlayerSrcEffectMechanic(GUID effect, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([ effect ], plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public PlayerSrcEffectMechanic(ReadOnlySpan<GUID> effects, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effects, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        PlayerChecker(log, mechanicLogs);
    }
}
