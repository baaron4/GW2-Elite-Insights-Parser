using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class LonelyTower : FractalLogic
{
    public LonelyTower(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.LonelyTower;
        LogID |= LogIDs.FractalMasks.LonelyTowerMask;
    }
}
