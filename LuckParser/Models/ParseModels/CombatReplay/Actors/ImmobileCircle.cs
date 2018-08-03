using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class ImmobileCircle : CircleActor
    {
        private Point3D position;

        public ImmobileCircle(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, radius,lifespan,color)
        {
            this.position = position;
        }

        public override string getPosition(string id, CombatReplayMap map)
        {
            Tuple<int, int> coord = map.getMapCoord(position.X, position.Y);
            return "["+coord.Item1+","+coord.Item2+"]";
        }
    }
}
