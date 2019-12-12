using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class LineDecorationSerializable : FormDecorationSerializable
    {
        public object ConnectedFrom { get; }

        public LineDecorationSerializable(ParsedLog log, LineDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "Line";
            ConnectedFrom = decoration.ConnectedFrom.GetConnectedTo(map, log);
        }
    }

}
