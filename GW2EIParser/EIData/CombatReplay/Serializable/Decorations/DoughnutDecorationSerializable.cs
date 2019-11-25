using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class DoughnutDecorationSerializable : FormDecorationSerializable
    {
        public int InnerRadius { get; }
        public int OuterRadius { get; }

        public DoughnutDecorationSerializable(ParsedLog log, DoughnutDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }

    }
}
