using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class CaptainMaiTrinBoss : FractalLogic
{
    public CaptainMaiTrinBoss(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.CaptainMaiTrinBossFractal;
        LogID = LogIDs.Unknown;
    }
}
