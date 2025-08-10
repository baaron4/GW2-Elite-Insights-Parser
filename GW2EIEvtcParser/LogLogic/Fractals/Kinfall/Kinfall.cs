using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class Kinfall : FractalLogic
{
    public Kinfall(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.Kinfall;
        LogID |= LogIDs.FractalMasks.KinfallMask;
    }
}
