using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class PieDecorationSerializable : CircleDecorationSerializable
    {
        public int Direction { get; set; }
        public int OpeningAngle { get; set; }

        public PieDecorationSerializable(ParsedLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            Direction = decoration.Direction;
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
