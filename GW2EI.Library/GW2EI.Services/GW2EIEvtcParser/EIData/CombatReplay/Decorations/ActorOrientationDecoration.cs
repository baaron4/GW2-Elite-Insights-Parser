using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

internal class ActorOrientationDecoration : AttachedDecoration
{
    public class ActorOrientationDecorationMetadata : AttachedDecorationMetadata
    {

        public override string GetSignature()
        {
            return "AO";
        }
        internal override DecorationMetadataDescription GetCombatReplayMetadataDescription(StringToUlongHandler stringToUlongHandler)
        {
            return new ActorOrientationDecorationMetadataDescription(this, stringToUlongHandler);
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

        internal override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature, StringToUlongHandler stringToUlongHandler)
        {
            return new ActorOrientationDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature, stringToUlongHandler);
        }
    }

    public ActorOrientationDecoration((long start, long end) lifespan, AgentItem agent) : base(new ActorOrientationDecorationMetadata(), new ActorOrientationDecorationRenderingData(lifespan, agent))
    {
    }

    //
}
