namespace GW2EIEvtcParser.EIData
{
    public class DoughnutDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public int InnerRadius { get; }
        public int OuterRadius { get; }

        internal DoughnutDecorationCombatReplayDescription(ParsedEvtcLog log, DoughnutDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }

    }
}
