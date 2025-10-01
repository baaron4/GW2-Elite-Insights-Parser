
using static GW2EIEvtcParser.LogLogic.LogCategories;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class Bjora : IcebroodSagaRaidEncounter
{
    public Bjora(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = SubLogCategory.Bjora;
    }
}
