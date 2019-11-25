using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class NPCSerializable : AbstractSingleActorSerializable
    {
        public long Start { get; }
        public long End { get; }

        public NPCSerializable(NPC npc, ParsedLog log, CombatReplayMap map, CombatReplay replay) : base(npc, log, map, replay, log.FightData.Logic.Targets.Contains(npc) ? "Target" : "NPC")
        {
            Start = replay.TimeOffsets.start;
            End = replay.TimeOffsets.end;
        }
    }
}
