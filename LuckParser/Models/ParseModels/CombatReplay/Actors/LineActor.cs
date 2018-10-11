using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class LineActor : Actor
    {
        public Object Target { get; }
        public int Width { get; }

        // using startpoint and endpoint
        public LineActor(int growing, int width, Point3D endPoint, Tuple<int, int> lifespan, string color) : base(false, growing, lifespan, color)
        {
            Target = endPoint;
            Width = width;
        }

        public LineActor(int growing, int width, Point3D endPoint, Tuple<int, int> lifespan, string color, Point3D position) : base(false, growing, lifespan, color, position)
        {
            Target = endPoint;
            Width = width;
        }

        public LineActor(int growing, int width, Point3D endPoint, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(false, growing, lifespan, color, prev, next, time)
        {
            Target = endPoint;
            Width = width;
        }

        public LineActor(int growing, int width, int targetID, Tuple<int, int> lifespan, string color) : base(false, growing, lifespan, color)
        {
            Target = targetID;
            Width = width;
        }

        public LineActor(int growing, int width, int targetID, Tuple<int, int> lifespan, string color, Point3D position) : base(false, growing, lifespan, color, position)
        {
            Target = targetID;
            Width = width;
        }

        public LineActor(int growing, int width, int targetID, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(false, growing, lifespan, color, prev, next, time)
        {
            Target = targetID;
            Width = width;
        }

        // using startpoint, direction as arcs rotation argument and length
        public LineActor(int growing, int width, Point3D startPoint, Point3D rotation, int length, Tuple<int, int> lifespan, string color) : base(false, growing, lifespan, color)
        {
            Target = new Point3D(startPoint.X + rotation.X * length, startPoint.Y + rotation.Y * length, startPoint.Z, startPoint.Time);
            Width = width;
        }

        public LineActor(int growing, int width, Point3D rotation, int length, Tuple<int, int> lifespan, string color, Point3D position) : base(false, growing, lifespan, color, position)
        {
            Target = new Point3D(position.X + rotation.X * length, position.Y + rotation.Y * length, position.Z, position.Time);
            Width = width;
        }

        public LineActor(int growing, int width, Point3D startPoint, Point3D rotation, int length, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(false, growing, lifespan, color, prev, next, time)
        {
            Target = new Point3D(startPoint.X + rotation.X * length, startPoint.Y + rotation.Y * length, startPoint.Z, startPoint.Time);
            Width = width;
        }

        // using startpoint, direction as angle value (in degrees) and length
        public LineActor(int growing, int width, Point3D startPoint, int direction, int length, Tuple<int, int> lifespan, string color) : base(false, growing, lifespan, color)
        {
            Target = new Point3D(startPoint.X + (int)Math.Cos(direction / 180 * Math.PI) * length, startPoint.Y + (int)Math.Sin(direction / 180 * Math.PI) * length, startPoint.Z, startPoint.Time);
            Width = width;
        }

        public LineActor(int growing, int width, int direction, int length, Tuple<int, int> lifespan, string color, Point3D position) : base(false, growing, lifespan, color, position)
        {
            Target = new Point3D(position.X + (int)Math.Cos(direction / 180 * Math.PI) * length, position.Y + (int)Math.Sin(direction / 180 * Math.PI) * length, position.Z, position.Time);
            Width = width;
        }

        public LineActor(int growing, int width, Point3D startPoint, int direction, int length, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(false, growing, lifespan, color, prev, next, time)
        {
            Target = new Point3D(startPoint.X + (int)Math.Cos(direction / 180 * Math.PI) * length, startPoint.Y + (int)Math.Sin(direction / 180 * Math.PI) * length, startPoint.Z, startPoint.Time);
            Width = width;
        }

        //
        private class LineSerializable : Serializable
        {
            public Object Target { get; set; }
            public int Width { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            LineSerializable aux = new LineSerializable
            {
                Type = "Line",
                Width = Width,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.Item1,
                End = Lifespan.Item2
            };
            if (Target is Point3D)
            {
                Point3D TargetPoint = (Point3D)Target;
                Tuple<int, int> mapPos = map.GetMapCoord(TargetPoint.X, TargetPoint.Y);
                aux.Target = new int[2]
                {
                        mapPos.Item1,
                        mapPos.Item2
                };
            }
            else
            {
                aux.Target = (int)Target;
            }
            if (Position != null)
            {
                Tuple<int, int> mapPos = map.GetMapCoord(Position.X, Position.Y);
                aux.ConnectedTo = new int[2]
                {
                        mapPos.Item1,
                        mapPos.Item2
                };
                return JsonConvert.SerializeObject(aux);
            }
            else
            {
                aux.ConnectedTo = master.GetCombatReplayID();
                return JsonConvert.SerializeObject(aux);
            }
        }
    }
}
