using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class TextOverheadDecoration : TextDecoration
{
    public class TextOverheadDecorationMetadata : TextDecorationMetadata
    {


        public TextOverheadDecorationMetadata(string color, string? backgroundColor = null) : base(color, backgroundColor)
        {
        }

        public override string GetSignature()
        {
            return "TxtO" + Color + BackgroundColor ?? "";
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new TextOverheadDecorationMetadataDescription(this);
        }
    }
    public class TextOverheadDecorationRenderingData : TextDecorationRenderingData
    {
        public TextOverheadDecorationRenderingData((long, long) lifespan, string text, uint fontSize, GeographicalConnector connector) : base(lifespan, text, fontSize, connector)
        {
        }
        public override void UsingSkillMode(SkillModeDescriptor? skill)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new TextOverheadDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    public TextOverheadDecoration((long, long) lifespan, string text, uint fontSize, string color, AgentConnector connector) : base(new TextOverheadDecorationMetadata(color), new TextOverheadDecorationRenderingData(lifespan, text, fontSize, connector))
    {
    }

    public TextOverheadDecoration(Segment lifespan, string text, uint fontSize, string color, AgentConnector connector) : this((lifespan.Start, lifespan.End), text, fontSize, color, connector)
    {
    }
    //
}
