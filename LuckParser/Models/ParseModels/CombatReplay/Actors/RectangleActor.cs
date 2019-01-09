using Newtonsoft.Json;
using System;

namespace LuckParser.Models.ParseModels
{
    public class RectangleActor : FormActor
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleActor(bool fill, int growing, int width, int height, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            Height = height;
            Width = width;
        }
        //


        protected class RectangleSerializable : FormSerializable
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map)
        {
            RectangleSerializable aux = new RectangleSerializable
            {
                Type = "Rectangle",
                Width = Width,
                Height = Height,
                Fill = Filled,
                Color = Color,
                Growing = Growing,
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map)
            };
            return JsonConvert.SerializeObject(aux);
        }
    }
}
