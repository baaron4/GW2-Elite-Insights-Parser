namespace GW2EIEvtcParser.EIData
{
    public class RectangleDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public int Height { get; }
        public int Width { get; }

        internal RectangleDecorationCombatReplayDescription(ParsedEvtcLog log, RectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
