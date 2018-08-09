using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class ImmobileActor : Mobility
    {
        private Point3D position;

        public ImmobileActor(Point3D position) : base()
        {
            this.position = position;
        }

        public override string getPosition(string id, CombatReplayMap map)
        {
            Tuple<int, int> coord = map.getMapCoord(position.X, position.Y);
            return "[" + coord.Item1 + "," + coord.Item2 + "]";
        }
    }
}
