using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class NPCSerializable : AbstractSingleActorSerializable
    {

        internal NPCSerializable(NPC npc, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay, log.FightData.Logic.TargetAgents.Contains(npc.AgentItem) ? "Target" : log.FriendlyAgents.Contains(npc.AgentItem) ? "Friendly" : "Mob")
        {

            if (log.FriendlyAgents.Contains(npc.AgentItem))
            {
                SetStatus(log, npc);
            }
        }
    }
}
