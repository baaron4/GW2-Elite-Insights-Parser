using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemySrcEffectMechanic : SrcEffectMechanic
{

    public EnemySrcEffectMechanic(GUID effect, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this([ effect ], inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public EnemySrcEffectMechanic(ReadOnlySpan<GUID> effects, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effects, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
        IsEnemyMechanic = true;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        EnemyChecker(log, mechanicLogs, regroupedMobs);
    }
}
