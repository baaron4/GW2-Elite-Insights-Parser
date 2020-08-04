using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EncounterLogic
{
    public class UnknownFightLogic : FightLogic
    {
        public UnknownFightLogic(int triggerID) : base(triggerID)
        {
            Extension = "boss";
            Icon = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>();
        }

        protected override void ComputeFightTargets(AgentData agentData, List<CombatItem> combatItems)
        {
            int id = GetFightTargetsIDs().First();
            AgentItem agentItem = agentData.GetNPCsByID(id).FirstOrDefault();
            // Trigger ID is not NPC
            if (agentItem == null)
            {
                agentItem = agentData.GetGadgetsByID(id).FirstOrDefault();
                if (agentItem != null)
                {
                    Targets.Add(new NPC(agentItem));
                }
            }
            else
            {
                Targets.Add(new NPC(agentItem));
            }
        }
    }
}
