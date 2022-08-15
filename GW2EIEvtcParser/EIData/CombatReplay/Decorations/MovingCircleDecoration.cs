using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class MovingCircleDecoration : CircleDecoration
    {
        public IReadOnlyList<Point3D> Points { get; }

        // constructors

        public MovingCircleDecoration(bool fill, int radius, (int start, int end) lifespan, string color, Connector connector) : base(fill, 0, radius, lifespan, color, connector)
        {
        }
        //

        public override GenericDecorationDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new MovingCircleDecorationCombatReplayDescription(log, this, map);
        }
    }
}
