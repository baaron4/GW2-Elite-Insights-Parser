
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class SalvationPass : RaidLogic
    {
        public SalvationPass(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.SalvationPass;
            EncounterID |= EncounterIDs.RaidWingMasks.SalvationPassMask;
        }
    }
}
