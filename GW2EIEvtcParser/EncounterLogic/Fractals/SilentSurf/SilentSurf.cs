using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class SilentSurf : FractalLogic
{
    public SilentSurf(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.SilentSurf;
        LogID |= LogIDs.FractalMasks.SilentSurfMask;
    }
}
