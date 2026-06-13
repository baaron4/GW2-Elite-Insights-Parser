using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class Kinfall : FractalLogic
{
    public Kinfall(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.Kinfall;
        LogID |= LogIDs.FractalMasks.KinfallMask;
    }
}
