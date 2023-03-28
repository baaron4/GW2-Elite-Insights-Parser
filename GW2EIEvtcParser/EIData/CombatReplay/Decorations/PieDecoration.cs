using System;

namespace GW2EIEvtcParser.EIData
{
    internal class PieDecoration : CircleDecoration
    {
        public float Direction { get; } //angle in degrees, growing clockwise and x-axis being 0
        public float OpeningAngle { get; } //in degrees

        // constructors


        //using arcs rotation argument as Input (cone in facing direction). Y direction is reversed due to different axis definitions for arc and javascript

        public PieDecoration(bool fill, int growing, int radius, Point3D rotation, float openingAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, radius, lifespan, color, connector)
        {
            Direction = Point3D.GetRotationFromFacing(rotation);
            OpeningAngle = openingAngle;
        }


        //using simple direction/opening angle definition 

        public PieDecoration(bool fill, int growing, int radius, float direction, float openingAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, radius, lifespan, color, connector)
        {
            Direction = (float)Math.Round(direction, ParserHelper.CombatReplayDataDigit);
            OpeningAngle = openingAngle;
        }

        //using starting point and end point (center of the circle and middle of the curved circle segment line)

        public PieDecoration(bool fill, int growing, Point3D startPoint, Point3D endPoint, float openingAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, (int)startPoint.DistanceToPoint(endPoint), lifespan, color, connector)
        {
            Direction = Point3D.GetRotationFromFacing(endPoint - startPoint);
            OpeningAngle = openingAngle;
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new PieDecorationCombatReplayDescription(log, this, map);
        }
    }
}
