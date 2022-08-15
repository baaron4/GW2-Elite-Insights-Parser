namespace GW2EIEvtcParser.EIData
{
    public class RectangleDecorationDescription : FormDecorationDescription
    {
        public int Height { get; }
        public int Width { get; }

        internal RectangleDecorationDescription(ParsedEvtcLog log, RectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
