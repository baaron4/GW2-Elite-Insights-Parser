using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class RotatedRectangleDecorationSerializable : RectangleDecorationSerializable
    {
        public int Rotation { get; }
        public int RadialTranslation { get; }
        public int SpinAngle { get; }

        public RotatedRectangleDecorationSerializable(ParsedLog log, RotatedRectangleDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "RotatedRectangle";
            Rotation = decoration.Rotation;
            RadialTranslation = decoration.RadialTranslation;
            SpinAngle = decoration.SpinAngle;
        }

    }
}
