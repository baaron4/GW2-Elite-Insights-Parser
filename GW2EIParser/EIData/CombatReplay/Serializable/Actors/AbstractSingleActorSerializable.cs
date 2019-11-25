using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class AbstractSingleActorSerializable
    {
        public string Img { get; }
        public string Type { get; }
        public int ID { get;}
        public List<double> Positions { get; }

        protected AbstractSingleActorSerializable(AbstractSingleActor actor, ParsedLog log, CombatReplayMap map, CombatReplay replay, string type)
        {
            Img = actor.Icon;
            ID = actor.GetCombatReplayID(log);
            Positions = new List<double>();
            foreach (Point3D pos in replay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                Positions.Add(x);
                Positions.Add(y);
            }
        }

    }
}
