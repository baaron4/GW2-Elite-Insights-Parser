
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class MythwrightGambit : RaidLogic
    {
        public MythwrightGambit(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.MythwrightGambit;
        }  
    }
}
