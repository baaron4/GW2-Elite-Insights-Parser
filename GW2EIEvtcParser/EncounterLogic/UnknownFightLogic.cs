using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class UnknownFightLogic : FightLogic
    {
        public UnknownFightLogic(int triggerID) : base(triggerID)
        {
            Extension = "boss";
            Icon = EncounterIconGeneric;
            EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
            EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>();
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                AgentItem target = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault() ?? agentData.GetGadgetsByID(GenericTriggerID).FirstOrDefault();
                return GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, target);
            }
            return GetGenericFightOffset(fightData);
        }

        internal override void ComputeFightTargets(AgentData agentData, List<CombatItem> combatItems, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            int id = GenericTriggerID;
            AgentItem agentItem = agentData.GetNPCsByID(id).FirstOrDefault();
            // Trigger ID is not NPC
            if (agentItem == null)
            {
                agentItem = agentData.GetGadgetsByID(id).FirstOrDefault();
                if (agentItem != null)
                {
                    _targets.Add(new NPC(agentItem));
                }
            }
            else
            {
                _targets.Add(new NPC(agentItem));
            }
            //
            FinalizeComputeFightTargets();
        }
    }
}
