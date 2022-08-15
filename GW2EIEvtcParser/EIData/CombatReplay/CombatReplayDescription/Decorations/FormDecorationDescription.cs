namespace GW2EIEvtcParser.EIData
{
    public abstract class FormDecorationDescription : GenericAttachedDecorationDescription
    {
        public bool Fill { get; }
        public int Growing { get; }
        public string Color { get; }

        internal FormDecorationDescription(ParsedEvtcLog log, FormDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Fill = decoration.Filled;
            Color = decoration.Color;
            Growing = decoration.Growing;
        }

    }

}
