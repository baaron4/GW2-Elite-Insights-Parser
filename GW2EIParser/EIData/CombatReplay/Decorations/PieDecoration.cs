using System;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class PieDecoration : CircleDecoration
    {
        public int Direction { get; } //angle in degrees, growing clockwise and x-axis being 0
        public int OpeningAngle { get; } //in degrees

        // constructors


        //using arcs rotation argument as Input (cone in facing direction). Y direction is reversed due to different axis definitions for arc and javascript

        public PieDecoration(bool fill, int growing, int radius, Point3D rotation, int openingAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, radius, lifespan, color, connector)
        {
            Direction = (int)Math.Round(Math.Atan2(rotation.Y, rotation.X) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }


        //using simple direction/opening angle definition 

        public PieDecoration(bool fill, int growing, int radius, int direction, int openingAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, radius, lifespan, color, connector)
        {
            Direction = direction;
            OpeningAngle = openingAngle;
        }

        //using starting point and end point (center of the circle and middle of the curved circle segment line)

        public PieDecoration(bool fill, int growing, Point3D startPoint, Point3D endPoint, int openingAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, (int)startPoint.DistanceToPoint(endPoint), lifespan, color, connector)
        {
            Direction = (int)Math.Round(Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }

        //

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new PieDecorationSerializable(log, this, map);
        }
    }
}
