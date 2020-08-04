namespace GW2EIEvtcParser.EIData
{
    public class FacingDecorationSerializable : GenericAttachedDecorationSerializable
    {
        public int[] FacingData { get; }

        public FacingDecorationSerializable(ParsedEvtcLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Facing";
            FacingData = decoration.Angles.ToArray();
        }

    }
}
