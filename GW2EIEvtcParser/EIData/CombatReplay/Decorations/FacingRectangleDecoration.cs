using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    internal class FacingRectangleDecoration : FacingDecoration
    {
        public int Width { get; }
        public int Height { get; }
        public string Color { get; }
        public int Translation { get; }
        public FacingRectangleDecoration((int start, int end) lifespan, AgentConnector connector, IReadOnlyList<ParametricPoint3D> facings, int width, int height, string color) : base(lifespan, connector, facings)
        {
            Width = width;
            Height = height;
            Color = color;
        }

        public FacingRectangleDecoration((int start, int end) lifespan, AgentConnector connector, IReadOnlyList<ParametricPoint3D> facings, int width, int height, int translation, string color) : this(lifespan, connector, facings, width, height, color)
        {
            Translation = translation;
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new FacingRectangleDecorationCombatReplayDescription(log, this, map);
        }
    }
}
