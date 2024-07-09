using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.FormDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class FormDecorationMetadataDescription : GenericAttachedDecorationMetadataDescription
    {
        public string Color { get; }

        internal FormDecorationMetadataDescription(FormDecorationMetadata decoration) : base(decoration)
        {
            Color = decoration.Color;
        }

    }

}
