using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class CircleDecorationSerializable : FormDecorationSerializable
    {
        public int Radius { get; }
        public int MinRadius { get; }

        public CircleDecorationSerializable(ParsedLog log, CircleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }

}
