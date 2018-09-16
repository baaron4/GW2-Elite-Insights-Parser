using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class CircleActor : Actor
    {
        public int Radius { get; }

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
        //
        protected class CircleSerializable<T> : Serializable<T>
        {
            public int Radius { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            if (Type == PositionType.Array)
            {
                CircleSerializable<int[]> aux = new CircleSerializable<int[]>
                {
                    Type = "Circle",
                    Radius = Radius,
                    Fill = Filled,
                    Color = Color,
                    Growing = Growing,
                    Start = Lifespan.Item1,
                    End = Lifespan.Item2,
                    Position = new int[2]
                };
                Tuple<int, int> mapPos = map.GetMapCoord(Position.X, Position.Y);
                aux.Position[0] = mapPos.Item1;
                aux.Position[1] = mapPos.Item2;
                return JsonConvert.SerializeObject(aux);
            } else
            {

                CircleSerializable<int> aux = new CircleSerializable<int>()
                {
                    Type = "Circle",
                    Radius = Radius,
                    Fill = Filled,
                    Color = Color,
                    Growing = Growing,
                    Start = Lifespan.Item1,
                    End = Lifespan.Item2,
                    Position = master.GetCombatReplayID()
                };
                return JsonConvert.SerializeObject(aux);
            }
        }
    }
}
