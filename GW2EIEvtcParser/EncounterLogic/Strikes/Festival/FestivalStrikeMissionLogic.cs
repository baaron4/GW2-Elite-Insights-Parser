namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class FestivalStrikeMissionLogic : StrikeMissionLogic
    {

        protected FestivalStrikeMissionLogic(int triggerID) : base(triggerID)
        {
            EncounterID |= EncounterIDs.StrikeMasks.FestivalMask;
        }
    }
}
