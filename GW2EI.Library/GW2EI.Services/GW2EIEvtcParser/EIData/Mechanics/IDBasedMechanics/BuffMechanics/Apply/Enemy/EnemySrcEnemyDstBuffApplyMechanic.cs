using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class EnemySrcEnemyDstBuffApplyMechanic : EnemySrcBuffApplyMechanic
{

    public EnemySrcEnemyDstBuffApplyMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public EnemySrcEnemyDstBuffApplyMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    protected override void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffApplyEvent ba, SingleActor actor, Dictionary<int, SingleActor> regroupedMobs)
    {
        SingleActor? dst = MechanicHelper.FindEnemyActor(log, ba.To, regroupedMobs);
        if (dst != null)
        {
            InsertMechanic(log, mechanicLogs, ba.Time, actor, ba.AppliedDuration);
            InsertMechanic(log, mechanicLogs, ba.Time, dst, ba.AppliedDuration);
        }
    }
}
