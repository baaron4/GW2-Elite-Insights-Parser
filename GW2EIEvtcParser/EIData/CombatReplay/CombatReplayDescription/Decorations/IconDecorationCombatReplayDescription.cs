namespace GW2EIEvtcParser.EIData
{
    public class IconDecorationCombatReplayDescription : GenericAttachedDecorationCombatReplayDescription
    {
        public string Image { get; }
        public int Size { get; }
        public float Opacity { get; }
        public int Owner { get; }

        internal IconDecorationCombatReplayDescription(ParsedEvtcLog log, IconDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "IconDecoration";
            Image = decoration.Image;
            Size = decoration.Size;
            Opacity = decoration.Opacity;
            Owner = decoration.OwnerID;
        }
    }

}
