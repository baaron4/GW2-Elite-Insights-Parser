
namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class BastionOfThePenitent : RaidLogic
    {
        public BastionOfThePenitent(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.EncounterSubCategory = SubFightCategory.BastionOfThePenitent;
        }  
    }
}
