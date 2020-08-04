namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericAttachedDecorationSerializable : GenericDecorationSerializable
    {
        public object ConnectedTo { get; }

        protected GenericAttachedDecorationSerializable(ParsedEvtcLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        }
    }
}
