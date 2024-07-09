using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class ActorOrientationDecoration : GenericAttachedDecoration
    {
        internal class ActorOrientationDecorationMetadata : GenericAttachedDecorationMetadata
        {
            public override string GetSignature()
            {
                return "AO";
            }

            internal override GenericDecoration GetDecorationFromVariable(GenericDecorationRenderingData renderingData)
            {
                if (renderingData is ActorOrientationDecorationRenderingData  expectedRenderingData)
                {
                    return new ActorOrientationDecoration(this,  expectedRenderingData);
                }
                throw new InvalidOperationException("Expected VariableActorOrientationDecoration");
            }
        }
        internal class ActorOrientationDecorationRenderingData : GenericAttachedDecorationRenderingData
        {
            public ActorOrientationDecorationRenderingData((long, long) lifespan, AgentItem agent) : base(lifespan, new AgentConnector(agent))
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

        internal ActorOrientationDecoration(ActorOrientationDecorationMetadata metadata, ActorOrientationDecorationRenderingData renderingData) : base (metadata, renderingData)
        {
        }

        public ActorOrientationDecoration((long start, long end) lifespan, AgentItem agent) : base(new ActorOrientationDecorationMetadata(), new ActorOrientationDecorationRenderingData(lifespan, agent))
        {
        }

        //

        public override GenericDecorationRenderableDescription GetRenderableDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new ActorOrientationDecorationRenderableDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
