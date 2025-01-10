using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class MovingPlatformDecoration : BackgroundDecoration
{
    internal class MovingPlatformDecorationMetadata : BackgroundDecorationMetadata
    {
        public readonly string Image;
        public readonly int Width;
        public readonly int Height;
        public MovingPlatformDecorationMetadata(string image, int width, int height)
        {
            Image = image;
            Width = width;
            Height = height;
        }

        public override string GetSignature()
        {
            return "MP" + Height + Image.GetHashCode().ToString() + Width;
        }
        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new MovingPlatformDecorationMetadataDescription(this);
        }
    }
    internal class MovingPlatformDecorationRenderingData((long, long) lifespan) : BackgroundDecorationRenderingData(lifespan)
    {
        public readonly List<(float x, float y, float z, float angle, float opacity, long time)> Positions = [];

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new MovingPlatformDecorationRenderingDescription(this, map, metadataSignature);
        }
    }
    private new MovingPlatformDecorationMetadata DecorationMetadata => (MovingPlatformDecorationMetadata)base.DecorationMetadata;
    private new MovingPlatformDecorationRenderingData DecorationRenderingData => (MovingPlatformDecorationRenderingData)base.DecorationRenderingData;
    //
    public string Image => DecorationMetadata.Image;
    public int Width => DecorationMetadata.Width;
    public int Height => DecorationMetadata.Height;

    public IReadOnlyList<(float x, float y, float z, float angle, float opacity, long time)> Positions => DecorationRenderingData.Positions;

    public MovingPlatformDecoration(string image, int width, int height, (long start, long end) lifespan) : base(new MovingPlatformDecorationMetadata(image, width, height), new MovingPlatformDecorationRenderingData(lifespan))
    {
    }

    public void AddPosition(float x, float y, float z, float angle, float opacity, long time)
    {
        DecorationRenderingData.Positions.Add((x ,y , z, angle, opacity, time));
    }
}
