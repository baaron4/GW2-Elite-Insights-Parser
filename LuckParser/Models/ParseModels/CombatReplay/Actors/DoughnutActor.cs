using System;

namespace LuckParser.Models.ParseModels
{
    public class DoughnutActor : Actor
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color) : base(true, growing, lifespan, color)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
            Type = PositionType.ID;
        }

        public DoughnutActor(int growing, int innerRadius, int outerRadius, Tuple<int, int> lifespan, string color, Point3D position) : base(true, growing, lifespan, color)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
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
