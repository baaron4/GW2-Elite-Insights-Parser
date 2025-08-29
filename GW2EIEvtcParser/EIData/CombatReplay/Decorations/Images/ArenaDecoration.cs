using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class ArenaDecoration : AttachedDecoration
{
    public class ArenaDecorationMetadata : AttachedDecorationMetadata
    {
        public readonly string Image;
        public readonly float Height;
        public readonly float Width;
        public ArenaDecorationMetadata(string icon, float width, float height) : base()
        {
            Image = icon;
            Height = height;
            Width = width;
        }

        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new ArenaDecorationMetadataDescription(this);
        }

        public override string GetSignature()
        {
            return "AI" + Image.GetHashCode().ToString() + Width + "-" + Height;
        }
    }
    public class ArenaDecorationRenderingData : AttachedDecorationRenderingData
    {
        public ArenaDecorationRenderingData((long, long) lifespan, PositionConnector connector) : base(lifespan, connector)
        {
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new ArenaDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }

        public override void UsingRotationConnector(RotationConnector? rotationConnectedTo)
        {
        }
        public override void UsingSkillMode(SkillModeDescriptor? skill)
        {
        }
    }
    private new ArenaDecorationMetadata DecorationMetadata => (ArenaDecorationMetadata)base.DecorationMetadata;

    protected ArenaDecoration(ArenaDecorationMetadata metadata, ArenaDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public ArenaDecoration((long, long) lifespan, string imageUrl, float width, float height, PositionConnector connector) : this(new ArenaDecorationMetadata(imageUrl, width, height), new ArenaDecorationRenderingData(lifespan, connector))
    {

    }
    public ArenaDecoration((long, long) lifespan, string imageUrl, CombatReplayMap crMap) : this(new ArenaDecorationMetadata(imageUrl, crMap.Width, crMap.Height), new ArenaDecorationRenderingData(lifespan, new PositionConnector(new Vector3(crMap.TopX, crMap.BottomY, 0))))
    {

    }
    public ArenaDecoration(string imageUrl, CombatReplayMap crMap) : this(new ArenaDecorationMetadata(imageUrl, crMap.Width, crMap.Height), new ArenaDecorationRenderingData((long.MinValue, long.MaxValue), new PositionConnector(new Vector3(crMap.TopX, crMap.BottomY, 0))))
    {

    }
}
