using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractSingleActorSerializable
    {
        public string Img { get; }
        public string Type { get; }
        public int ID { get; }
        public List<double> Positions { get; }
        public List<long> Dead { get; private set; }
        public List<long> Down { get; private set; }
        public List<long> Dc { get; private set; }
        public long Start { get; }
        public long End { get; }

        internal AbstractSingleActorSerializable(AbstractSingleActor actor, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay, string type)
        {
            Start = replay.TimeOffsets.start;
            End = replay.TimeOffsets.end;
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
        }
        protected void SetStatus(ParsedEvtcLog log, AbstractSingleActor a)
        {
            Dead = new List<long>();
            Down = new List<long>();
            Dc = new List<long>();
            (IReadOnlyList<(long start, long end)> deads, IReadOnlyList<(long start, long end)> downs, IReadOnlyList<(long start, long end)> dcs) = a.GetStatus(log);

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
