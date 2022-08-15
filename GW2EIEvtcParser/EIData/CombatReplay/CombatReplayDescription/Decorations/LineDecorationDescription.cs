namespace GW2EIEvtcParser.EIData
{
    public class LineDecorationDescription : FormDecorationDescription
    {
        public object ConnectedFrom { get; }

        internal LineDecorationDescription(ParsedEvtcLog log, LineDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Line";
            ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
        }
    }

}
