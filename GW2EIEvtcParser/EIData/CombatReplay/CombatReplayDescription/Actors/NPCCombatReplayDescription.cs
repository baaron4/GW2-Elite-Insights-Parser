using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class NPCCombatReplayDescription : AbstractSingleActorCombatReplayDescription
    {
        public int MasterID { get; }

        internal NPCCombatReplayDescription(NPC npc, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay, log.FightData.Logic.TargetAgents.Contains(npc.AgentItem) ? "Target" : (log.FightData.Logic.NonPlayerFriendlyAgents.Contains(npc.AgentItem) || npc.AgentItem.GetFinalMaster().Type == ParsedData.AgentItem.AgentType.Player) ? "Friendly" : "Mob")
        {
            if (log.FriendlyAgents.Contains(npc.AgentItem))
            {
                SetStatus(log, npc);
            }
            if (npc.AgentItem.GetFinalMaster() != npc.AgentItem)
            {
                MasterID = npc.AgentItem.GetFinalMaster().UniqueID;
            }
        }
    }
}
