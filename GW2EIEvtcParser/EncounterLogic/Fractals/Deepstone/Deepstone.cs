using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class Deepstone : FractalLogic
{
    public Deepstone(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = SubFightCategory.Deepstone;
        EncounterID = EncounterIDs.Unknown;
    }
}
