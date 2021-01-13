
namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class SalvationPass : RaidLogic
    {
        public SalvationPass(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.EncounterSubCategory = SubFightCategory.SalvationPass;
        }  
    }
}
