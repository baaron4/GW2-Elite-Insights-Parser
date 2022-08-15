namespace GW2EIEvtcParser.EIData
{
    public class DoughnutDecorationDescription : FormDecorationDescription
    {
        public int InnerRadius { get; }
        public int OuterRadius { get; }

        internal DoughnutDecorationDescription(ParsedEvtcLog log, DoughnutDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }

    }
}
