using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public abstract class Decoration
{
    internal abstract class _DecorationMetadata
    {

        public abstract string GetSignature();

        public abstract DecorationMetadataDescription GetCombatReplayMetadataDescription();

    }

    internal abstract class _DecorationRenderingData
    {
        public readonly (int start, int end) Lifespan;

        protected _DecorationRenderingData((long start, long end) lifespan)
        {
            Lifespan = ((int)lifespan.start, (int)lifespan.end);
        }
        public abstract DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature);
    }

    internal readonly _DecorationMetadata DecorationMetadata;
    internal readonly _DecorationRenderingData DecorationRenderingData;

    public (int start, int end) Lifespan => DecorationRenderingData.Lifespan;
    internal Decoration(_DecorationMetadata metaData, _DecorationRenderingData renderingData)
    {
        DecorationMetadata = metaData;
        DecorationRenderingData = renderingData;
    }
}
