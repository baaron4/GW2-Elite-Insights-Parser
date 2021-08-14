using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class NPCCombatReplayDescription : AbstractSingleActorCombatReplayDescription
    {

        internal NPCCombatReplayDescription(NPC npc, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay, log.FightData.Logic.TargetAgents.Contains(npc.AgentItem) ? "Target" : log.FriendlyAgents.Contains(npc.AgentItem) ? "Friendly" : "Mob")
        {

            if (log.FriendlyAgents.Contains(npc.AgentItem))
            {
                SetStatus(log, npc);
            }
        }
    }
}
