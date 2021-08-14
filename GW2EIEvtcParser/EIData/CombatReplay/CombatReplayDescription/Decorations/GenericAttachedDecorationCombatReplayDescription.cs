namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericAttachedDecorationCombatReplayDescription : GenericDecorationCombatReplayDescription
    {
        public object ConnectedTo { get; }

        internal GenericAttachedDecorationCombatReplayDescription(ParsedEvtcLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        }
    }
}
