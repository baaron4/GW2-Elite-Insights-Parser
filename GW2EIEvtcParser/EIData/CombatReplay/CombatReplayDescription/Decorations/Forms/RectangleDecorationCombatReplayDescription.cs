namespace GW2EIEvtcParser.EIData
{
    public class RectangleDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public uint Height { get; }
        public uint Width { get; }

        internal RectangleDecorationCombatReplayDescription(ParsedEvtcLog log, RectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
