
namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class HallOfChains : RaidLogic
    {
        public HallOfChains(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.EncounterSubCategory = SubFightCategory.HallOfChains;
        }  
    }
}
