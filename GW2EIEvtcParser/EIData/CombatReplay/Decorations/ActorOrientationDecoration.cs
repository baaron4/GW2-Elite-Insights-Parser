using System;
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

            internal override GenericDecoration GetDecorationFromVariable(VariableGenericDecoration variable)
            {
                if (variable is VariableActorOrientationDecoration expectedVariable)
                {
                    return new ActorOrientationDecoration(this, expectedVariable);
                }
                throw new InvalidOperationException("Expected VariableActorOrientationDecoration");
            }
        }
        internal class VariableActorOrientationDecoration : VariableGenericAttachedDecoration
        {
            public VariableActorOrientationDecoration((long, long) lifespan, AgentItem agent) : base(lifespan, new AgentConnector(agent))
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

        internal ActorOrientationDecoration(ConstantActorOrientationDecoration constant, VariableActorOrientationDecoration variable)
        {
            ConstantDecoration = constant;
            VariableDecoration = variable;
        }

        public ActorOrientationDecoration((long start, long end) lifespan, AgentItem agent) : base()
        {
            ConstantDecoration = new ConstantActorOrientationDecoration();
            VariableDecoration = new VariableActorOrientationDecoration(lifespan, agent);
        }

        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new ActorOrientationDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
