
namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class StrongholdOfTheFaithful : RaidLogic
    {
        public StrongholdOfTheFaithful(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.EncounterSubCategory = SubFightCategory.StrongholdOfTheFaithful;
        }  
    }
}
