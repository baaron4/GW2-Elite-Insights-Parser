using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecoration
    {

        internal abstract class GenericDecorationMetadata
        {

            public abstract string GetSignature();

            public abstract GenericDecorationMetadataDescription GetCombatReplayMetadataDescription();

        }
        internal abstract class GenericDecorationRenderingData
        {
            public (int start, int end) Lifespan { get; }

            protected GenericDecorationRenderingData((long start, long end) lifespan)
            {
                Lifespan = ((int)lifespan.start, (int)lifespan.end);
            }
            public abstract GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature);
        }

        internal GenericDecorationMetadata DecorationMetadata { get; }
        internal GenericDecorationRenderingData DecorationRenderingData { get; }

        public (int start, int end) Lifespan => DecorationRenderingData.Lifespan;
        internal GenericDecoration(GenericDecorationMetadata metaData, GenericDecorationRenderingData renderingData)
        {
            DecorationMetadata = metaData;
            DecorationRenderingData = renderingData;
        }
        protected GenericDecoration()
        {
        }
        //

    }
}
