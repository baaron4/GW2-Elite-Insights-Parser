using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public abstract class Decoration
{
    public abstract class _DecorationMetadata
    {

        public abstract string GetSignature();

        public abstract DecorationMetadataDescription GetCombatReplayMetadataDescription();

    }

    public abstract class _DecorationRenderingData
    {
        public readonly (long start, long end) Lifespan;

        protected _DecorationRenderingData((long start, long end) lifespan)
        {
            Lifespan = (lifespan.start, lifespan.end);
        }
        public abstract DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature);
    }

    internal readonly _DecorationMetadata DecorationMetadata;
    internal readonly _DecorationRenderingData DecorationRenderingData;

    public (long start, long end) Lifespan => DecorationRenderingData.Lifespan;
    protected Decoration(_DecorationMetadata metaData, _DecorationRenderingData renderingData)
    {
        DecorationMetadata = metaData;
        DecorationRenderingData = renderingData;
    }
}
