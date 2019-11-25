using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class FacingRectangleDecorationSerializable : FacingDecorationSerializable
    {
        public int Width { get; }
        public int Height { get; }
        public string Color { get; }

        public FacingRectangleDecorationSerializable(ParsedLog log, FacingRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "FacingRectangle";
            Width = decoration.Width;
            Height = decoration.Height;
            Color = decoration.Color;
        }
    }
}
