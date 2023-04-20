using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

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
        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetGenericFightOffset(fightData);
        }
    }
}
