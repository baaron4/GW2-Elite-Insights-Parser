using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class GenericAttachedDecorationSerializable : GenericDecorationSerializable
    {
        public object ConnectedTo { get; }

        protected GenericAttachedDecorationSerializable(ParsedLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        }
    }
}
