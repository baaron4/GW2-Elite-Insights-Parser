using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class PlayerSerializable : AbstractSingleActorSerializable
    {
        public int Group { get; }

        internal PlayerSerializable(Player player, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(player, log, map, replay, "Player")
        {
            Group = player.Group;
        }

    }
}
