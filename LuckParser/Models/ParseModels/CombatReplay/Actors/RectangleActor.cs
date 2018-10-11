using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class RectangleActor : Actor
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color) : base(fill, growing, lifespan, color)
        {
            Height = height;
            Width = width;
        }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, lifespan, color, position)
        {
            Height = height;
            Width = width;
        }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color, Point3D prev, Point3D next, int time) : base(fill, growing, lifespan, color, prev, next, time)
        {
            Height = height;
            Width = width;
        }
        //


        private class RectangleSerializable : Serializable
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            RectangleSerializable aux = new RectangleSerializable
            {
                Type = "Rectangle",
                Width = Width,
                Height = Height,
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
