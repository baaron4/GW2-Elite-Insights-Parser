using System.IO;

namespace GW2EIEvtcParser.EIData
{
    public class IconDecorationCombatReplayDescription : GenericIconDecorationCombatReplayDescription
    {
        public float Opacity { get; }

        internal IconDecorationCombatReplayDescription(ParsedEvtcLog log, IconDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "IconDecoration";
            Opacity = decoration.Opacity;
            if (WorldSize == 0 && PixelSize == 0)
            {
                throw new InvalidDataException("Icon Decoration must have at least one size strictly positive");
            }
        }
    }

}
