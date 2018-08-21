using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class ImmobileActor : Mobility
    {
        private Point3D _position;

        public ImmobileActor(Point3D position) : base()
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
