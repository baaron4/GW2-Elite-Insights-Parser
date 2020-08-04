namespace GW2EIEvtcParser.EIData
{
    public abstract class FormDecorationSerializable : GenericAttachedDecorationSerializable
    {
        public bool Fill { get; }
        public int Growing { get; }
        public string Color { get; }

        protected FormDecorationSerializable(ParsedEvtcLog log, FormDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Fill = decoration.Filled;
            Color = decoration.Color;
            Growing = decoration.Growing;
        }

    }

}
