namespace GW2EIEvtcParser.EIData
{
    public class FacingDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public int[] FacingData { get; }

        internal FacingDecorationCombatReplayDescription(ParsedEvtcLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Facing";
            FacingData = decoration.Angles.ToArray();
        }

    }
}
