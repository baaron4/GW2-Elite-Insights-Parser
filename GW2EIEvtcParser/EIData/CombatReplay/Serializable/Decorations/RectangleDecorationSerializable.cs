namespace GW2EIEvtcParser.EIData
{
    public class RectangleDecorationSerializable : FormDecorationSerializable
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleDecorationSerializable(ParsedEvtcLog log, RectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
