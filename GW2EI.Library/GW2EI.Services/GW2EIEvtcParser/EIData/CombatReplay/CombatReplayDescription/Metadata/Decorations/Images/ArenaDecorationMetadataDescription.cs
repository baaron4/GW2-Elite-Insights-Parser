using static GW2EIEvtcParser.EIData.ArenaDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class ArenaDecorationMetadataDescription : AttachedDecorationMetadataDescription
{

    public readonly string Image;
    public readonly float Height;
    public readonly float Width;
    internal ArenaDecorationMetadataDescription(ArenaDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Image = decoration.Image;
        Height = decoration.Height;
        Width = decoration.Width;
        Type = Types.Arena;
    }
}
