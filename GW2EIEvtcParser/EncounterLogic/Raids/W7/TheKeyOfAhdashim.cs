
namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class TheKeyOfAhdashim : RaidLogic
    {
        public TheKeyOfAhdashim(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.EncounterSubCategory = SubFightCategory.TheKeyOfAhdashim;
        }  
    }
}
