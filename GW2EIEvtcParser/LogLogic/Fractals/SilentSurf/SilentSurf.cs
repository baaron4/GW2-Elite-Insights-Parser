using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class SilentSurf : FractalLogic
{
    public SilentSurf(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.SilentSurf;
        LogID |= LogIDs.FractalMasks.SilentSurfMask;
    }
}
