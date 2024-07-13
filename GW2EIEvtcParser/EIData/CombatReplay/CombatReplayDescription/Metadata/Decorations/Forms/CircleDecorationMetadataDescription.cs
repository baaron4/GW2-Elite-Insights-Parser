using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.CircleDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class CircleDecorationMetadataDescription : FormDecorationMetadataDescription
    {
        public uint Radius { get; }
        public uint MinRadius { get; }

        internal CircleDecorationMetadataDescription(CircleDecorationMetadata decoration) : base(decoration)
        {
            Type = "Circle";
            Radius = decoration.Radius;
            MinRadius = decoration.MinRadius;
        }
    }

}
