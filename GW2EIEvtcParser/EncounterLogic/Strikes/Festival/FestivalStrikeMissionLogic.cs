namespace GW2EIEvtcParser.LogLogic;

internal abstract class FestivalStrikeMissionLogic : StrikeMissionLogic
{

    protected FestivalStrikeMissionLogic(int triggerID) : base(triggerID)
    {
        LogID |= LogIDs.StrikeMasks.FestivalMask;
    }
}
