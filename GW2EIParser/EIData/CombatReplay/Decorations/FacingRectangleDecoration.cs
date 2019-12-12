using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class FacingRectangleDecoration : FacingDecoration
    {
        public int Width { get; }
        public int Height { get; }
        public string Color { get; }
        public FacingRectangleDecoration((int start, int end) lifespan, AgentConnector connector, List<Point3D> facings, int width, int height, string color) : base(lifespan, connector, facings)
        {
            Width = width;
            Height = height;
            Color = color;
        }

        //

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new FacingRectangleDecorationSerializable(log, this, map);
        }
    }
}
