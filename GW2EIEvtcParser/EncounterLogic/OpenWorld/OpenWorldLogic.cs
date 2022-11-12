using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class OpenWorldLogic : FightLogic
    {
        public OpenWorldLogic(int triggerID) : base(triggerID)
        {
            Mode = ParseMode.OpenWorld;
            EncounterCategoryInformation.Category = EncounterCategory.FightCategory.OpenWorld;
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.OpenWorld;
            EncounterID |= EncounterIDs.EncounterMasks.OpenWorldMask;
        }
    }
}
