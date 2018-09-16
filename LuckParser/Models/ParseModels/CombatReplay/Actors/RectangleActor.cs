using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class RectangleActor : Actor
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color)
        {
            Height = height;
            Width = width;
        }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color, position)
        {
            Height = height;
            Width = width;
        }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(true, growing, lifespan, color, prev, next, time)
        {
            Height = height;
            Width = width;
        }
        //


        private class RectangleSerializable<T> : Serializable<T>
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            if (Type == PositionType.Array)
            {
                RectangleSerializable<int[]> aux = new RectangleSerializable<int[]>
                {
                    Type = "Rectangle",
                    Width = Width,
                    Height = Height,
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
            }
            else
            {

                RectangleSerializable<int> aux = new RectangleSerializable<int>()
                {
                    Type = "Rectangle",
                    Width = Width,
                    Height = Height,
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
