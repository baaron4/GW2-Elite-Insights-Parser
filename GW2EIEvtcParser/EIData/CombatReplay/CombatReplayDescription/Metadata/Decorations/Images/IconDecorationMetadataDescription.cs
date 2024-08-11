using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.IconDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class IconDecorationMetadataDescription : GenericIconDecorationMetadataDescription
    {
        public float Opacity { get; }

        internal IconDecorationMetadataDescription(IconDecorationMetadata decoration) : base(decoration)
        {
            Type = "IconDecoration";
            Opacity = decoration.Opacity;
        }
    }

}
