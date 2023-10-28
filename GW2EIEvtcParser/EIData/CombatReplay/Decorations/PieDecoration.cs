using System;

namespace GW2EIEvtcParser.EIData
{
    internal class PieDecoration : CircleDecoration
    {
        public float OpeningAngle { get; } //in degrees


        //using arcs rotation argument as Input (cone in facing direction). Y direction is reversed due to different axis definitions for arc and javascript

        public PieDecoration(int radius, float openingAngle, (long start, long end) lifespan, string color, Connector connector) : base(radius, lifespan, color, connector)
        {
            OpeningAngle = openingAngle;
        }

        public override FormDecoration Copy()
        {
            return (PieDecoration)new PieDecoration(Radius, OpeningAngle, Lifespan, Color, ConnectedTo).UsingFilled(Filled).UsingGrowingEnd(GrowingEnd, GrowingReverse).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new PieDecorationCombatReplayDescription(log, this, map);
        }
    }
}
