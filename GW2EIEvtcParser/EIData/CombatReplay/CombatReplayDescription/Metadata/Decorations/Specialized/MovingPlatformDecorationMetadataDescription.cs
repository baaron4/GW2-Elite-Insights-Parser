using static GW2EIEvtcParser.EIData.MovingPlatformDecoration;

namespace GW2EIEvtcParser.EIData;

public class MovingPlatformDecorationMetadataDescription : BackgroundDecorationMetadataDescription
{
    public readonly string Image;
    public readonly int Height;
    public readonly int Width;

    internal MovingPlatformDecorationMetadataDescription(MovingPlatformDecorationMetadata decoration) : base(decoration)
    {
        Type = Types.MovingPlatform;
        Image = decoration.Image;
        Width = decoration.Width;
        Height = decoration.Height;
    }
}
