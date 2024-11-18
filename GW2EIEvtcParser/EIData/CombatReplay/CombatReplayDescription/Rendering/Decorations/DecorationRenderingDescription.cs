using static GW2EIEvtcParser.EIData.Decoration;

namespace GW2EIEvtcParser.EIData;

public abstract class DecorationRenderingDescription : CombatReplayRenderingDescription
{
    public readonly string MetadataSignature;
    internal DecorationRenderingDescription(_DecorationRenderingData decoration, string metadataSignature)
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
