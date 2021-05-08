using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractSingleActorSerializable
    {
        public string Img { get; }
        public string Type { get; }
        public int ID { get; }
        public List<double> Positions { get; }
        public List<long> Dead { get; }
        public List<long> Down { get; }
        public List<long> Dc { get; }

        internal AbstractSingleActorSerializable(AbstractSingleActor actor, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay, string type)
        {
            Img = actor.GetIcon();
            ID = actor.UniqueID;
            Positions = new List<double>();
            Type = type;
            foreach (Point3D pos in replay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                Positions.Add(x);
                Positions.Add(y);
            }
            if (actor.AgentItem.IsPlayer)
            {
                Dead = new List<long>();
                Down = new List<long>();
                Dc = new List<long>();
                (IReadOnlyList<(long start, long end)> deads, IReadOnlyList<(long start, long end)> downs, IReadOnlyList<(long start, long end)> dcs) = actor.GetStatus(log);

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
}
