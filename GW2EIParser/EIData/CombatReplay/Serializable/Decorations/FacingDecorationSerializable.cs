using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class FacingDecorationSerializable : GenericDecorationSerializable
    {
        public int[] FacingData { get; }

        public FacingDecorationSerializable(ParsedLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Facing";
            FacingData = decoration.Angles.ToArray();
        }

    }
}
