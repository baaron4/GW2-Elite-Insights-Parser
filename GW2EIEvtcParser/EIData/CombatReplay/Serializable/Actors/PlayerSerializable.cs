using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class PlayerSerializable : AbstractSingleActorSerializable
    {
        public int Group { get; }
        public List<long> Dead { get; }
        public List<long> Down { get; }
        public List<long> Dc { get; }

        public PlayerSerializable(Player player, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(player, log, map, replay, "Player")
        {
            Group = player.Group;
            Dead = new List<long>();
            Down = new List<long>();
            Dc = new List<long>();
            (List<(long start, long end)> deads, List<(long start, long end)> downs, List<(long start, long end)> dcs) = player.GetStatus(log);

            foreach ((long start, long end) in deads)
            {
                Dead.Add(start);
                Dead.Add(end);
            }
            foreach ((long start, long end) in downs)
            {
                Down.Add(start);
                Down.Add(end);
            }
            foreach ((long start, long end) in dcs)
            {
                Dc.Add(start);
                Dc.Add(end);
            }
        }

    }
}
