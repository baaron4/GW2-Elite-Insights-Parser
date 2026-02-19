using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;


internal class PlayerSrcPlayerDstBuffApplyMechanic : PlayerSrcBuffApplyMechanic
{

    public PlayerSrcPlayerDstBuffApplyMechanic(long mechanicID, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, plotlySetting, shortName, description, fullName, internalCoolDown)
    {
    }

    public PlayerSrcPlayerDstBuffApplyMechanic(long[] mechanicIDs, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, plotlySetting, shortName, description, fullName, internalCoolDown)
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
