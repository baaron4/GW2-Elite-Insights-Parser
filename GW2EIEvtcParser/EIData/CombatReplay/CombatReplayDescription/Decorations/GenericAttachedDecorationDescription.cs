namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericAttachedDecorationDescription : GenericDecorationDescription
    {
        public object ConnectedTo { get; }

        internal GenericAttachedDecorationDescription(ParsedEvtcLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        }
    }
}
