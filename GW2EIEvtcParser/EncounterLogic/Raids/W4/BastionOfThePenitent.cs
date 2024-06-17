
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class BastionOfThePenitent : RaidLogic
    {
        public BastionOfThePenitent(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.BastionOfThePenitent;
            EncounterID |= EncounterIDs.RaidWingMasks.BastionOfThePenitentMask;
        }
    }
}
