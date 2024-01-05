namespace GW2EIEvtcParser.EIData
{
    public class PieDecorationCombatReplayDescription : CircleDecorationCombatReplayDescription
    {
        public float OpeningAngle { get; set; }

        internal PieDecorationCombatReplayDescription(ParsedEvtcLog log, PieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Pie";
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
