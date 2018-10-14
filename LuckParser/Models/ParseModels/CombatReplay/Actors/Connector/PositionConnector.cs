using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class PositionConnector : Connector
    {
        private Point3D _position;

        public PositionConnector(Point3D position)
        {
            _position = position;
        }

        public override object GetConnectedTo(CombatReplayMap map)
        {
            Tuple<int, int> mapPos = map.GetMapCoord(_position.X, _position.Y);
            return new int[2]
                       {
                        mapPos.Item1,
                        mapPos.Item2
                       }; ;
        }
    }
}
