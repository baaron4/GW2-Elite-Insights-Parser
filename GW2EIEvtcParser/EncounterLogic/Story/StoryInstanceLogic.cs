using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
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
        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetGenericFightOffset(fightData);
        }
    }
}
