using System;
using static GW2EIEvtcParser.EIData.GenericDecoration;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecorationRenderingDescription : AbstractCombatReplayRenderingDescription
    {
        public string MetadataSignature { get; }
        internal GenericDecorationRenderingDescription(GenericDecorationRenderingData decoration, string metadataSignature)
        {
            Start = decoration.Lifespan.start;
            End = decoration.Lifespan.end;
            MetadataSignature = metadataSignature;
            if (End <= Start)
            {
                // such things should be filtered way before coming here
                throw new InvalidOperationException("Decorations can not have a negative or zero lifespan");
            }
        }
    }
}
