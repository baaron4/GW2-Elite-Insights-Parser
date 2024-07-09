using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.RectangleDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class RectangleDecorationMetadataDescription : FormDecorationMetadataDescription
    {
        public uint Height { get; }
        public uint Width { get; }

        internal RectangleDecorationMetadataDescription(RectangleDecorationMetadata decoration) : base(decoration)
        {
            Type = "Rectangle";
            Width = decoration.Width;
            Height = decoration.Height;
        }
    }
}
