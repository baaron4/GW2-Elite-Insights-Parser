namespace GW2EIEvtcParser.EIData
{
    public class DoughnutDecorationSerializable : FormDecorationSerializable
    {
        public int InnerRadius { get; }
        public int OuterRadius { get; }

        internal DoughnutDecorationSerializable(ParsedEvtcLog log, DoughnutDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Doughnut";
            OuterRadius = decoration.OuterRadius;
            InnerRadius = decoration.InnerRadius;
        }

    }
}
