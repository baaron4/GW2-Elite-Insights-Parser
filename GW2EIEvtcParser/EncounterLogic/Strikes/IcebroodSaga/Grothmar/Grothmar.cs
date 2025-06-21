using System.Drawing;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EIData.Trigonometry;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Grothmar : IcebroodSagaStrike
{
    public Grothmar(int triggerID) : base(triggerID)
    {
        EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Grothmar;
    }
}
