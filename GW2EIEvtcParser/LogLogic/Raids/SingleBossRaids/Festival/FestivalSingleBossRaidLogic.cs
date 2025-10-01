namespace GW2EIEvtcParser.LogLogic;

internal abstract class FestivalSingleBossRaidLogic : SingleBossRaidLogic
{

    protected FestivalSingleBossRaidLogic(int triggerID) : base(triggerID)
    {
        LogID |= LogIDs.SingleBossRaidMasks.FestivalMask;
    }
}
