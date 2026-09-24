using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class SolitaryThrone : FractalLogic
{
    public SolitaryThrone(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.SolitaryThrone;
        LogID |= LogIDs.FractalMasks.SolitaryThroneMask;
    }
}
