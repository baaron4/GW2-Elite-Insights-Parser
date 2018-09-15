using System;

namespace LuckParser.Models.ParseModels
{
    public class RectangleActor : Actor
    {
        public int Height { get; }
        public int Width { get; }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color)
        {
            Height = height;
            Width = width;
            Type = PositionType.ID;
        }

        public RectangleActor(bool fill, int growing, int width, int height, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color)
        {
            Height = height;
            Width = width;
            Position = position;
            Type = PositionType.Array;
        }
        //

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            throw new NotImplementedException();
        }
    }
}
