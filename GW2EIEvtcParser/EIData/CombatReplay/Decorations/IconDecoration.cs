namespace GW2EIEvtcParser.EIData
{
    internal class IconDecoration : GenericAttachedDecoration
    {
        public string Image { get; }
        public int Size { get; }
        public float Opacity { get; }
        public int OwnerID { get; }

        public IconDecoration(string icon, int size, float opacity, int ownerID, (int start, int end) lifespan, Connector connector) : base(lifespan, connector)
        {
            Image = icon;
            Size = size;
            Opacity = opacity;
            OwnerID = ownerID;
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new IconDecorationCombatReplayDescription(log, this, map);
        }
    }
}
