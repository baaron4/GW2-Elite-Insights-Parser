using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Drizzlewood : IcebroodSagaStrike
{
    public Drizzlewood(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = LogCategories.SubLogCategory.Drizzlewood;
    }
}
