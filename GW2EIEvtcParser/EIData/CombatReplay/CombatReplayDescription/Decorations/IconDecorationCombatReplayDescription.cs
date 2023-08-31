namespace GW2EIEvtcParser.EIData
{
    public class IconDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public string Image { get; }
        public int PixelSize { get; }
        public int WorldSize { get; }
        public float Opacity { get; }

        internal IconDecorationCombatReplayDescription(ParsedEvtcLog log, IconDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "IconDecoration";
            Image = decoration.Image;
            PixelSize = decoration.PixelSize;
            WorldSize = decoration.WorldSize;
            Opacity = decoration.Opacity;
        }
    }

}
