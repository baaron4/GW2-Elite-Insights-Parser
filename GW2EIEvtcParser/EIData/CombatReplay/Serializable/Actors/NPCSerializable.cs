using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class NPCSerializable : AbstractSingleActorSerializable
    {
        public long Start { get; }
        public long End { get; }

        internal NPCSerializable(NPC npc, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay, log.FightData.Logic.TargetAgents.Contains(npc.AgentItem) ? "Target" : log.FriendlyAgents.Contains(npc.AgentItem) ? "Friendly" : "Mob")
        {
            Start = replay.TimeOffsets.start;
            End = replay.TimeOffsets.end;

            if (log.FriendlyAgents.Contains(npc.AgentItem))
            {
                SetStatus(log, npc);
            }
        }
    }
}
