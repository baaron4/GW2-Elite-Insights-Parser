using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class ActorOrientationDecoration : GenericAttachedDecoration
    {

        public ActorOrientationDecoration((long start, long end) lifespan, AgentItem agent) : base(lifespan, new AgentConnector(agent))
        {
            RotationConnectedTo = new AgentFacingConnector(agent);
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new ActorOrientationDecorationCombatReplayDescription(log, this, map);
        }

        public override GenericAttachedDecoration UsingSkillMode(SkillModeDescriptor skill)
        {
            return this;
        }

        public override GenericAttachedDecoration UsingRotationConnector(RotationConnector rotationConnectedTo)
        {
            return this;
        }
    }
}
