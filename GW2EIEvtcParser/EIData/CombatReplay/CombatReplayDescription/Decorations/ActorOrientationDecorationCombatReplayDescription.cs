using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class ActorOrientationDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {

        internal ActorOrientationDecorationCombatReplayDescription(ParsedEvtcLog log, ActorOrientationDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "ActorOrientation";
            IsMechanicOrSkill = false;
        }

    }
}
