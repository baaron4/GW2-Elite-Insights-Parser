using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class ActorOrientationDecorationCombatReplayDescription : FacingDecorationCombatReplayDescription
    {

        internal ActorOrientationDecorationCombatReplayDescription(ParsedEvtcLog log, FacingDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "ActorOrientation";
            IsMechanicOrSkill = false;
        }

    }
}
