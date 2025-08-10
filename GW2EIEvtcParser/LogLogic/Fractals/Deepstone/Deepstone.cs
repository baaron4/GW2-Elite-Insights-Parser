using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class Deepstone : FractalLogic
{
    public Deepstone(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.Deepstone;
        LogID = LogIDs.Unknown;
    }
}
