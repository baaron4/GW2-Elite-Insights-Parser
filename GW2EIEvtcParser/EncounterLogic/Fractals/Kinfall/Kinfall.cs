using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;

namespace GW2EIEvtcParser.EncounterLogic;

internal abstract class Kinfall : FractalLogic
{
    public Kinfall(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = SubFightCategory.Kinfall;
        EncounterID |= EncounterIDs.FractalMasks.KinfallMask;
    }
}
