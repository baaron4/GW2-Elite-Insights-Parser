
namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class SpiritVale : RaidLogic
    {
        public SpiritVale(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.EncounterSubCategory = SubFightCategory.SpiritVale;
        }  
    }
}
