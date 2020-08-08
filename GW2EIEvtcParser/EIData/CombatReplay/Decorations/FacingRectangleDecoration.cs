using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class FacingRectangleDecoration : FacingDecoration
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

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new FacingRectangleDecorationSerializable(log, this, map);
        }
    }
}
