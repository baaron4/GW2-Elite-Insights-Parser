using static GW2EIEvtcParser.EIData.Decoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public abstract class DecorationRenderingDescription : CombatReplayRenderingDescription
{
    public readonly ulong MetadataSignature;
    internal DecorationRenderingDescription(_DecorationRenderingData decoration, string metadataSignature, StringToUlongHandler stringToUlongHandler)
    {
        Start = decoration.Lifespan.start;
        End = decoration.Lifespan.end;
        MetadataSignature = stringToUlongHandler.GetSignatureIndex(metadataSignature);
        if (End <= Start)
        {
            // such things should be filtered way before coming here
            throw new InvalidOperationException("Decorations can not have a negative or zero lifespan");
        }
    }
}
