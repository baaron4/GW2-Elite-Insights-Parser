using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemyDstEffectMechanic : DstEffectMechanic
{

    public EnemyDstEffectMechanic(GUID effect, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : this([ effect ], id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemyDstEffectMechanic(ReadOnlySpan<GUID> effects, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown) : base(effects, id, plotlySetting, description, severity, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        EnemyChecker(log, mechanicLogs, regroupedMobs);
    }
}
