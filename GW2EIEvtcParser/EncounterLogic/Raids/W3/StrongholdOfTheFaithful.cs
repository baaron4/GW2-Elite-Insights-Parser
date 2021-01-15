
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class StrongholdOfTheFaithful : RaidLogic
    {
        public StrongholdOfTheFaithful(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.StrongholdOfTheFaithful;
        }  
    }
}
