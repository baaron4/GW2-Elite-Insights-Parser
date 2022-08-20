namespace GW2EIEvtcParser.EIData
{
    public class RotatedRectangleDecorationCombatReplayDescription : RectangleDecorationCombatReplayDescription
    {
        public float Rotation { get; }
        public int RadialTranslation { get; }
        public int SpinAngle { get; }

        internal RotatedRectangleDecorationCombatReplayDescription(ParsedEvtcLog log, RotatedRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "RotatedRectangle";
            Rotation = decoration.Rotation;
            RadialTranslation = decoration.RadialTranslation;
            SpinAngle = decoration.SpinAngle;
        }

    }
}
