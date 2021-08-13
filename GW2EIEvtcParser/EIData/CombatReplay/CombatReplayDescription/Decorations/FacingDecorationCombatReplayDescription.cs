using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class FacingDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public IReadOnlyList<float> FacingData { get; }

        internal FacingDecorationCombatReplayDescription(ParsedEvtcLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Facing";
            FacingData = decoration.Angles;
        }

    }
}
