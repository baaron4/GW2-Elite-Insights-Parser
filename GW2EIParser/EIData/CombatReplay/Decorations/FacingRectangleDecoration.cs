using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class FacingRectangleDecoration : FacingDecoration
    {
        private readonly int _width;
        private readonly int _height;
        private readonly string _color;
        public FacingRectangleDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings, int width, int height, string color) : base(lifespan, connector, facings)
        {
            _width = width;
            _height = height;
            _color = color;
        }

        //
        protected class FacingRectangleSerializable : FacingSerializable
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public string Color { get; set; }
        }

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            FacingSerializable aux = new FacingRectangleSerializable
            {
                Type = "FacingRectangle",
                Start = Lifespan.start,
                End = Lifespan.end,
                ConnectedTo = ConnectedTo.GetConnectedTo(map, log),
                FacingData = new int[Data.Count],
                Width = _width,
                Height = _height,
                Color = _color
            };
            int i = 0;
            foreach (int angle in Data)
            {
                aux.FacingData[i++] = angle;
            }
            return aux;
        }
    }
}
