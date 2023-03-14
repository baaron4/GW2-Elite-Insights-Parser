namespace GW2EIEvtcParser.EIData
{
    public class FacingPieDecorationCombatReplayDescription : FacingDecorationCombatReplayDescription
    {
        public float OpeningAngle { get; } //in degrees
        public int Radius { get; }
        public string Color { get; }

        internal FacingPieDecorationCombatReplayDescription(ParsedEvtcLog log, FacingPieDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "FacingPie";
            Radius = decoration.Radius;
            OpeningAngle = decoration.OpeningAngle;
            Color = decoration.Color;
        }
    }
}
