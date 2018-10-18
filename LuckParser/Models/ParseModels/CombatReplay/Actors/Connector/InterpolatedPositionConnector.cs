using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class InterpolatedPositionConnector : Connector
    {
        private Point3D _position;
        public InterpolatedPositionConnector(Point3D prev, Point3D next, int time)
        {
            if (prev != null && next != null)
            {
                long denom = next.Time - prev.Time;
                if (denom == 0)
                {
                    _position = prev;
                }
                else
                {
                    float ratio = (float)(time - prev.Time) / denom;
                    _position = new Point3D(prev, next, ratio, time);
                }
            }
            else
            {
                _position = prev ?? next;
            }
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
