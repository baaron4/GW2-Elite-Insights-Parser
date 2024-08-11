using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.PieDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal class PieDecorationMetadataDescription : CircleDecorationMetadataDescription
    {
        public float OpeningAngle { get; set; }

        internal PieDecorationMetadataDescription(PieDecorationMetadata decoration) : base(decoration)
        {
            Type = "Pie";
            OpeningAngle = decoration.OpeningAngle;
        }

    }
}
