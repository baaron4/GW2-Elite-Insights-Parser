
namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class MythwrightGambit : RaidLogic
    {
        public MythwrightGambit(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.EncounterSubCategory = SubFightCategory.MythwrightGambit;
        }  
    }
}
