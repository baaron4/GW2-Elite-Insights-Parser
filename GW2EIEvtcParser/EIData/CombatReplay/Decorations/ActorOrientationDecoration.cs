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
            public override GenericDecorationMetadataDescription GetCombatReplayMetadataDescription()
            {
                return new ActorOrientationDecorationMetadataDescription(this);
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

            public override GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
            {
                return new ActorOrientationDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
            }
        }

        internal ActorOrientationDecoration(ActorOrientationDecorationMetadata metadata, ActorOrientationDecorationRenderingData renderingData) : base (metadata, renderingData)
        {
        }

        public ActorOrientationDecoration((long start, long end) lifespan, AgentItem agent) : base(new ActorOrientationDecorationMetadata(), new ActorOrientationDecorationRenderingData(lifespan, agent))
        {
        }

        //
    }
}
