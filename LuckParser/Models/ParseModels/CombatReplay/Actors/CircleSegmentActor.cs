using System;

namespace LuckParser.Models.ParseModels
{
    public class CircleSegmentActor : CircleActor
    {
        public int Direction { get; } //angle in degrees, growing clockwise and x-axis being 0
        public int OpeningAngle { get; } //in degrees

        // constructors
        public CircleSegmentActor(bool fill, int growing, int radius, int direction, int openingAngle, Tuple<int, int> lifespan, string color) : base(fill, growing, radius, lifespan, color)
        {
            Direction = direction;
            OpeningAngle = openingAngle;
        }

        public CircleSegmentActor(bool fill, int growing, int radius, int direction, int openingAngle, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, radius, lifespan, color, position)
        {
            Direction = direction;
            OpeningAngle = openingAngle;
        }

        //using starting point and end point (center of the circle and middle of the curved circle segment line)
        public CircleSegmentActor(bool fill, int growing, Point3D startPoint, Point3D endPoint, int openingAngle, Tuple<int, int> lifespan, string color) : base(fill, growing, (int)startPoint.DistanceToPoint(endPoint), lifespan, color)
        {
            Direction = (int)Math.Round(Math.Atan2(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }

        public CircleSegmentActor(bool fill, int growing, Point3D startPoint, Point3D endPoint, int openingAngle, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, (int)startPoint.DistanceToPoint(endPoint), lifespan, color, position)
        {
            Direction = (int)Math.Round(Math.Atan2(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }
    }
}
