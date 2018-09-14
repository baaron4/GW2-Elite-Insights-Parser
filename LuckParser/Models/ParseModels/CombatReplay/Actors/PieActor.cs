using System;

namespace LuckParser.Models.ParseModels
{
    public class PieActor : CircleActor
    {
        public int Direction { get; } //angle in degrees, growing clockwise and x-axis being 0
        public int OpeningAngle { get; } //in degrees

        // constructors


        //using arcs rotation argument as Input (cone in facing direction). Y direction is reversed due to different axis definitions for arc and javascript
        public PieActor(bool fill, int growing, int radius, Point3D rotation, int openingAngle, Tuple<int, int> lifespan, string color) : base(fill, growing, radius, lifespan, color)
        {
            Direction = (int)Math.Round(Math.Atan2(-rotation.Y, rotation.X) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }

        public PieActor(bool fill, int growing, int radius, Point3D rotation, int openingAngle, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, radius, lifespan, color, position)
        {
            Direction = (int)Math.Round(Math.Atan2(-rotation.Y, rotation.X) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }


        //using simple direction/opening angle definition 
        public PieActor(bool fill, int growing, int radius, int direction, int openingAngle, Tuple<int, int> lifespan, string color) : base(fill, growing, radius, lifespan, color)
        {
            Direction = direction;
            OpeningAngle = openingAngle;
        }

        public PieActor(bool fill, int growing, int radius, int direction, int openingAngle, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, radius, lifespan, color, position)
        {
            Direction = direction;
            OpeningAngle = openingAngle;
        }

        //using starting point and end point (center of the circle and middle of the curved circle segment line)
        public PieActor(bool fill, int growing, Point3D startPoint, Point3D endPoint, int openingAngle, Tuple<int, int> lifespan, string color) : base(fill, growing, (int)startPoint.DistanceToPoint(endPoint), lifespan, color)
        {
            Direction = (int)Math.Round(Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }

        public PieActor(bool fill, int growing, Point3D startPoint, Point3D endPoint, int openingAngle, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, (int)startPoint.DistanceToPoint(endPoint), lifespan, color, position)
        {
            Direction = (int)Math.Round(Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X) * 180 / Math.PI);
            OpeningAngle = openingAngle;
        }
    }
}
