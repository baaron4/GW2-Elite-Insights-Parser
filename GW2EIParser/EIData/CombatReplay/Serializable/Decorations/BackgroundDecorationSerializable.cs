using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class BackgroundDecorationSerializable : GenericDecorationSerializable
    {
        protected BackgroundDecorationSerializable(ParsedLog log, BackgroundDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {

        }

    }

}
