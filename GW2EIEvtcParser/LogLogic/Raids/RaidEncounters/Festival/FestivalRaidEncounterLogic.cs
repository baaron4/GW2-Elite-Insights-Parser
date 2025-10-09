namespace GW2EIEvtcParser.LogLogic;

internal abstract class FestivalRaidEncounterLogic : RaidEncounterLogic
{

    protected FestivalRaidEncounterLogic(int triggerID) : base(triggerID)
    {
        LogID |= LogIDs.RaidEncounterMasks.FestivalMask;
    }
}
