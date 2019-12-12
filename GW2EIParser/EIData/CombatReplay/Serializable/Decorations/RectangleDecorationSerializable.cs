using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class RectangleDecorationSerializable : FormDecorationSerializable
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleDecorationSerializable(ParsedLog log, RectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
