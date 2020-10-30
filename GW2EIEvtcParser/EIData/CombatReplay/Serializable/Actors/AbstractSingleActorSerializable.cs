using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractSingleActorSerializable
    {
        public string Img { get; }
        public string Type { get; }
        public int ID { get; }
        public List<double> Positions { get; }

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
        }

    }
}
