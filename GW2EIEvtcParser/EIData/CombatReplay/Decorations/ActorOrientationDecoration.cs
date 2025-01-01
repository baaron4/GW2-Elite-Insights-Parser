using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class ActorOrientationDecoration : AttachedDecoration
{
    public class ActorOrientationDecorationMetadata : AttachedDecorationMetadata
    {

        public override string GetSignature()
        {
            return "AO";
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new ActorOrientationDecorationMetadataDescription(this);
        }
    }
    public class ActorOrientationDecorationRenderingData : AttachedDecorationRenderingData
    {
        public ActorOrientationDecorationRenderingData((long, long) lifespan, AgentItem agent) : base(lifespan, new AgentConnector(agent))
        {
            RotationConnectedTo = new AgentFacingConnector(agent);
        }

        public override void UsingRotationConnector(RotationConnector? rotationConnectedTo)
        {
        }
        public override void UsingSkillMode(SkillModeDescriptor? skill)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new ActorOrientationDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }

    public ActorOrientationDecoration((long start, long end) lifespan, AgentItem agent) : base(new ActorOrientationDecorationMetadata(), new ActorOrientationDecorationRenderingData(lifespan, agent))
    {
    }

    //
}
