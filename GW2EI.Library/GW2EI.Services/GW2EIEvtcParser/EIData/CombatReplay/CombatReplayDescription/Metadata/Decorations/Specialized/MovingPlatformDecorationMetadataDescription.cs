using static GW2EIEvtcParser.EIData.MovingPlatformDecoration;
using static GW2EIEvtcParser.ParsedEvtcLog;

namespace GW2EIEvtcParser.EIData;

public class MovingPlatformDecorationMetadataDescription : BackgroundDecorationMetadataDescription
{
    public readonly string Image;
    public readonly int Height;
    public readonly int Width;

    internal MovingPlatformDecorationMetadataDescription(MovingPlatformDecorationMetadata decoration, StringToUlongHandler stringToUlongHandler) : base(decoration, stringToUlongHandler)
    {
        Type = Types.MovingPlatform;
        Image = decoration.Image;
        Width = decoration.Width;
        Height = decoration.Height;
    }
}
