using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class ActorOrientationDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {

        public IReadOnlyList<float> FacingData { get; }
        internal ActorOrientationDecorationCombatReplayDescription(ParsedEvtcLog log, ActorOrientationDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            FacingData = decoration.Angles;
            Type = "ActorOrientation";
            IsMechanicOrSkill = false;
        }

    }
}
