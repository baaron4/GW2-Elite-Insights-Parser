using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class CircleActor : Actor
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color) : base(fill, growing, lifespan, color)
        {
            Radius = radius;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, lifespan, color, position)
        {
            Radius = radius;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(fill, growing, lifespan, color, prev, next, time)
        {
            Radius = radius;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, int minRadius) : base(fill, growing, lifespan, color)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Point3D position, int minRadius) : base(fill, growing, lifespan, color, position)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time, int minRadius) : base(fill, growing, lifespan, color, prev, next, time)
        {
            Radius = radius;
            MinRadius = minRadius;
        }

        //
        protected class CircleSerializable : Serializable
        {
            public int Radius { get; set; }
            public int MinRadius { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            CircleSerializable aux = new CircleSerializable
            {
                Type = "Circle",
                Radius = Radius,
                MinRadius = MinRadius,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.Item1,
                End = Lifespan.Item2
            };
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
