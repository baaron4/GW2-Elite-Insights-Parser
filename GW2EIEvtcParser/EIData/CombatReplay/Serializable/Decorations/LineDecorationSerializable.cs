namespace GW2EIEvtcParser.EIData
{
    public class LineDecorationSerializable : FormDecorationSerializable
    {
        public object ConnectedFrom { get; }

        public LineDecorationSerializable(ParsedEvtcLog log, LineDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Line";
            ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
        }
    }

}
