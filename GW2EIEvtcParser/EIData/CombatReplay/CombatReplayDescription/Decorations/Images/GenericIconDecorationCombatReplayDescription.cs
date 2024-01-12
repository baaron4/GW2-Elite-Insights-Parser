using System.IO;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericIconDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public string Image { get; }
        public uint PixelSize { get; }
        public uint WorldSize { get; }

        internal GenericIconDecorationCombatReplayDescription(ParsedEvtcLog log, GenericIconDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
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
