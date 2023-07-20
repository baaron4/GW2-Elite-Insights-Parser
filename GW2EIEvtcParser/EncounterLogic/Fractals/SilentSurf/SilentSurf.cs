using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class SilentSurf : FractalLogic
    {
        public SilentSurf(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.SilentSurf;
            EncounterID |= EncounterIDs.FractalMasks.SilentSurfMask;
        }
    }
}
