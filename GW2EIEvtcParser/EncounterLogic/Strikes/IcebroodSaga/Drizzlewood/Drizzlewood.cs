using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Drizzlewood : IcebroodSagaStrike
{
    public Drizzlewood(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Drizzlewood;
    }
}
