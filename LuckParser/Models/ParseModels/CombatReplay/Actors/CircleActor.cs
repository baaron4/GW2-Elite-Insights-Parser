using System;

namespace LuckParser.Models.ParseModels
{
    public class CircleActor : Actor
    {
        public int Radius { get; }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color) : base(fill, growing, lifespan, color)
        {
            Radius = radius;
            Type = PositionType.ID;
        }

        public CircleActor(bool fill, int growing, int radius, Tuple<int, int> lifespan, string color, Point3D position) : base(fill, growing, lifespan, color)
        {
            Radius = radius;
            Position = position;
            Type = PositionType.Array;
        }
        //
        protected class CircleSerializable<T> : Serializable<T>
        {
            public int Radius { get; set; }
        }

        public override string GetCombatReplayJSON(CombatReplayMap map, AbstractMasterPlayer master)
        {
            throw new NotImplementedException();
        }
    }
}
