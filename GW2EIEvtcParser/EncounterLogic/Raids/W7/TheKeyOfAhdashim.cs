
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class TheKeyOfAhdashim : RaidLogic
    {
        public TheKeyOfAhdashim(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.TheKeyOfAhdashim;
        }  
    }
}
