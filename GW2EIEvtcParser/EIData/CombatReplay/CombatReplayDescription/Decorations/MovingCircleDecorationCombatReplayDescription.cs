using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class MovingCircleDecorationCombatReplayDescription : CircleDecorationDescription
    {
        public IReadOnlyList<float> PositionsWithTimes { get; }

        internal MovingCircleDecorationCombatReplayDescription(ParsedEvtcLog log, MovingCircleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "MovingCircle";
            var list = new List<float>();
            PositionsWithTimes = list;
            foreach (Point3D point in decoration.Points)
            {
                (float x, float y) = map.GetMapCoord(point.X, point.Y);
                list.Add(x);
                list.Add(y);
                list.Add(point.Time);
            }
        }

    }
}
