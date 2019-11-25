using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class DoughnutDecoration : FormDecoration
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutDecoration(bool fill, int growing, int innerRadius, int outerRadius, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        //

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            return new DoughnutDecorationSerializable(log, this, map);
        }

    }
}
