using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecoration
    {

        internal abstract class GenericDecorationMetadata
        {

            public abstract string GetSignature();

            /// <summary>
            /// Temporary method to keep code outside of the solution intact.
            /// Will remain as a debugging tool down the line
            /// </summary>
            /// <param name="renderingData"></param>
            /// <returns></returns>
            internal abstract GenericDecoration GetDecorationFromVariable(GenericDecorationRenderingData renderingData);

        }
        internal abstract class GenericDecorationRenderingData
        {
            public (int start, int end) Lifespan { get; }

            protected GenericDecorationRenderingData((long start, long end) lifespan)
            {
                Lifespan = ((int)lifespan.start, (int)lifespan.end);
            }
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

        public abstract GenericDecorationRenderableDescription GetRenderableDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs);

    }
}
