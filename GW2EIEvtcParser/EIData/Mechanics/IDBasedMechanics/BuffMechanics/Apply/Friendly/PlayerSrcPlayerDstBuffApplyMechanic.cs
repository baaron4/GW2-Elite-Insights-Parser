using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcPlayerDstBuffApplyMechanic : PlayerSrcBuffApplyMechanic
{

    public PlayerSrcPlayerDstBuffApplyMechanic(long mechanicID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicID, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    public PlayerSrcPlayerDstBuffApplyMechanic(long[] mechanicIDs, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description, MechanicSeverity severity, int internalCoolDown = 0) : base(mechanicIDs, id, plotlySetting, description, severity, internalCoolDown)
    {
    }

    protected override void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffApplyEvent ba, SingleActor actor)
    {
        SingleActor? dst = MechanicHelper.FindPlayerActor(log, ba.To);
        if (dst != null)
        {
            InsertMechanic(log, mechanicLogs, ba.Time, actor, ba.AppliedDuration);
            InsertMechanic(log, mechanicLogs, ba.Time, dst, ba.AppliedDuration);
        }
    }
}
