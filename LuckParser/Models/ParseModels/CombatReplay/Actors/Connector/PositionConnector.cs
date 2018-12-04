using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class PositionConnector : Connector
    {
        protected Point3D Position;

        public PositionConnector()
        {

        }

        public PositionConnector(Point3D position)
        {
            Position = position;
        }

        public override object GetConnectedTo(CombatReplayMap map)
        {
            Tuple<double, double> mapPos = map.GetMapCoord(Position.X, Position.Y);
            return new double[2]
                       {
                        mapPos.Item1,
                        mapPos.Item2
                       };
        }
    }
}
