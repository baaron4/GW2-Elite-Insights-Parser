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
    internal abstract class StoryInstance : FightLogic
    {
        public StoryInstance(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.Category = EncounterCategory.FightCategory.Story;
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.Story;
            EncounterID |= EncounterIDs.EncounterMasks.StoryInstanceMask;
        }
    }
}
