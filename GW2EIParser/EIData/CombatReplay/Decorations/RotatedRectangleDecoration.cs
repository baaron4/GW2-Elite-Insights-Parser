using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class RotatedRectangleDecoration : RectangleDecoration
    {
        public int Rotation { get; } // initial rotation angle
        public int RadialTranslation { get; } // translation of the triangle center in the direction of the current rotation
        public int SpinAngle { get; } // rotation the rectangle is supposed to go through over the course of its lifespan, 0 for no rotation

        // Rectangles with fixed rotation and no translation
        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, int rotation, (int start, int end) lifespan, string color, Connector connector)
            : this(fill, growing, width, height, rotation, 0, 0, lifespan, color, connector) { }


        // Rectangles with a fixed rotation and translation

        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, int rotation, int translation, (int start, int end) lifespan, string color, Connector connector)
            : this(fill, growing, width, height, rotation, translation, 0, lifespan, color, connector) { }

        // Rectangles rotating over time

        public RotatedRectangleDecoration(bool fill, int growing, int width, int height, int rotation, int translation, int spinAngle, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, width, height, lifespan, color, connector)
        {
            Rotation = rotation;
            RadialTranslation = translation;
            SpinAngle = spinAngle;
        }

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new RotatedRectangleDecorationSerializable(log, this, map);
        }
    }
}
