using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.GenericIconDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericIconDecorationMetadataDescription : GenericAttachedDecorationMetadataDescription
    {
        public string Image { get; }
        public uint PixelSize { get; }
        public uint WorldSize { get; }

        internal GenericIconDecorationMetadataDescription(GenericIconDecorationMetadata decoration) : base(decoration)
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

}
