using System;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class FacingPieDecoration : FacingDecoration
    {
        public float OpeningAngle { get; } //in degrees
        public int Radius { get; }
        public string Color { get; }

        public FacingPieDecoration((int start, int end) lifespan, AgentConnector connector, IReadOnlyList<ParametricPoint3D> facings, int radius, float openingAngle, string color) : base(lifespan, connector, facings)
        {
            OpeningAngle = openingAngle;
            Radius = radius;
            Color = color;
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new FacingPieDecorationCombatReplayDescription(log, this, map);
        }
    }
}
