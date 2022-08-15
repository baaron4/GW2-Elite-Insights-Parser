using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class FacingDecorationDescription : GenericAttachedDecorationDescription
    {
        public IReadOnlyList<float> FacingData { get; }

        internal FacingDecorationDescription(ParsedEvtcLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Facing";
            FacingData = decoration.Angles;
        }

    }
}
