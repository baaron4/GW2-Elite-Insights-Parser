namespace GW2EIEvtcParser.EIData
{
    public class FacingRectangleDecorationSerializable : FacingDecorationSerializable
    {
        public int Width { get; }
        public int Height { get; }
        public string Color { get; }

        public FacingRectangleDecorationSerializable(ParsedEvtcLog log, FacingRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "FacingRectangle";
            Width = decoration.Width;
            Height = decoration.Height;
            Color = decoration.Color;
        }
    }
}
