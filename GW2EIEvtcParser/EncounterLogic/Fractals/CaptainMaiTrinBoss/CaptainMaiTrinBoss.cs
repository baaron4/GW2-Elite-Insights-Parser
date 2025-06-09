using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class CaptainMaiTrinBoss : FractalLogic
{
    public CaptainMaiTrinBoss(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = SubFightCategory.CaptainMaiTrinBossFractal;
        EncounterID |= 0;
    }
}
