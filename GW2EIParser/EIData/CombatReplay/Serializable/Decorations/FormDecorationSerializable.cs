using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public abstract class FormDecorationSerializable : GenericDecorationSerializable
    {
        public bool Fill { get; }
        public int Growing { get; }
        public string Color { get; }

        protected FormDecorationSerializable(ParsedLog log, FormDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Fill = decoration.Filled;
            Color = decoration.Color;
            Growing = decoration.Growing;
        }

    }

}
