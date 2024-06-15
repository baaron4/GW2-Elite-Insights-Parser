using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class ActorOrientationDecoration : GenericAttachedDecoration
    {
        internal class ConstantActorOrientationDecoration : ConstantGenericAttachedDecoration
        {
            public override string GetSignature()
            {
                return "AO";
            }
        }
        internal class VariableActorOrientationDecorationn : VariableGenericAttachedDecoration
        {
            public VariableActorOrientationDecorationn((long, long) lifespan, AgentItem agent) : base(lifespan, new AgentConnector(agent))
            {
                RotationConnectedTo = new AgentFacingConnector(agent);
            }

            public override void UsingRotationConnector(RotationConnector rotationConnectedTo)
            {
            }
            public override void UsingSkillMode(SkillModeDescriptor skill)
            {
            }
        }

        public ActorOrientationDecoration((long start, long end) lifespan, AgentItem agent) : base()
        {
            ConstantDecoration = new ConstantActorOrientationDecoration();
            VariableDecoration = new VariableActorOrientationDecorationn(lifespan, agent);
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new ActorOrientationDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
