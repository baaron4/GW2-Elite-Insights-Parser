using System.Drawing;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EIData.Trigonometry;

namespace GW2EIEvtcParser.LogLogic;

internal class Grothmar : IcebroodSagaStrike
{
    public Grothmar(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = LogCategories.SubLogCategory.Grothmar;
    }
}
