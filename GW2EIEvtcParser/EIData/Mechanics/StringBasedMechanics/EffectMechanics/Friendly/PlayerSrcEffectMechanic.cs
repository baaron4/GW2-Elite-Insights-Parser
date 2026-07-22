using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcEffectMechanic : SrcEffectMechanic
{

    public PlayerSrcEffectMechanic(GUID effect, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([ effect ], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerSrcEffectMechanic(ReadOnlySpan<GUID> effects, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(effects, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        PlayerChecker(log, mechanicLogs);
    }
}
