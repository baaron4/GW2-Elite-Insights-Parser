using System;

namespace LuckParser.Models.ParseModels
{
    public class ImmobileActor : Mobility
    {
        private readonly Point3D _position;

        public ImmobileActor(Point3D position)
        {
            _position = position;
        }

        public override string GetPosition(string id, CombatReplayMap map)
        {
            Tuple<int, int> coord = map.GetMapCoord(_position.X, _position.Y);
            return "[" + coord.Item1 + "," + coord.Item2 + "]";
        }
    }
}
