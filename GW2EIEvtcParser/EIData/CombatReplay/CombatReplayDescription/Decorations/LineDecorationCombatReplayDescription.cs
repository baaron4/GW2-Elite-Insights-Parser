namespace GW2EIEvtcParser.EIData
{
    public class LineDecorationCombatReplayDescription : FormDecorationCombatReplayDescription
    {
        public object ConnectedFrom { get; }

        internal LineDecorationCombatReplayDescription(ParsedEvtcLog log, LineDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Line";
            ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
        }
    }

}
