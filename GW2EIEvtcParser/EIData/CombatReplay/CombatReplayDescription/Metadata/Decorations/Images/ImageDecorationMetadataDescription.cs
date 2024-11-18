using static GW2EIEvtcParser.EIData.ImageDecoration;

namespace GW2EIEvtcParser.EIData;

public abstract class ImageDecorationMetadataDescription : AttachedDecorationMetadataDescription
{
    public readonly string Image;
    public readonly uint PixelSize;
    public readonly uint WorldSize;

    internal ImageDecorationMetadataDescription(ImageDecorationMetadata decoration) : base(decoration)
    {
        Image = decoration.Image;
        PixelSize = decoration.PixelSize;
        WorldSize = decoration.WorldSize;
        if (WorldSize == 0 && PixelSize == 0)
        {
            throw new InvalidDataException("Icon Decoration must have at least one size strictly positive");
        }
    }
}
