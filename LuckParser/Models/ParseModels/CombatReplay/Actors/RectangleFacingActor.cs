using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class RectangleFacingActor : FacingActor
    {
        private readonly int _width;
        private readonly int _height;
        private readonly string _color;
        public RectangleFacingActor((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings, int width, int height, string color) : base(lifespan, connector, facings)
        {
            _width = width;
            _height = height;
            _color = color;
        }

        //
        protected class RectangleFacingSerializable : FacingSerializable
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string Color { get; set; }
        }

        public override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map)
        {
            FacingSerializable aux = new RectangleFacingSerializable
            {
                Type = "RectangleFacing",
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map),
                FacingData = new object[Data.Count],
                Width = _width,
                Height = _height,
                Color = _color
            };
            int i = 0;
            foreach ((double angle, long time) in Data)
            {
                aux.FacingData[i++] = new object[2]
                {
                    angle,
                    time
                };
            }
            return aux;
        }
    }
}
