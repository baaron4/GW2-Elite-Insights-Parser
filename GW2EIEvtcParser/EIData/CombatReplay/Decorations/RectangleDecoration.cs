using System;

namespace GW2EIEvtcParser.EIData
{
    internal class RectangleDecoration : FormDecoration
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleDecoration(int width, int height, (long start, long end) lifespan, string color, Connector connector) : base( lifespan, color, connector)
        {
            Height = height;
            Width = width;
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new RectangleDecorationCombatReplayDescription(log, this, map);
        }
        public override FormDecoration Copy()
        {
            return (FormDecoration)new RectangleDecoration(Width, Height, Lifespan, Color, ConnectedTo).UsingFilled(Filled).UsingGrowing(Math.Abs(GrowingEnd), GrowingEnd < 0).UsingRotationConnector(RotationConnectedTo).UsingSkillMode(SkillMode);
        }
    }
}
