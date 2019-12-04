using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class GenericAttachedDecorationSerializable : GenericDecorationSerializable
    {
        public object ConnectedTo { get; }

        protected GenericAttachedDecorationSerializable(ParsedLog log, GenericAttachedDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
        }
    }
}
