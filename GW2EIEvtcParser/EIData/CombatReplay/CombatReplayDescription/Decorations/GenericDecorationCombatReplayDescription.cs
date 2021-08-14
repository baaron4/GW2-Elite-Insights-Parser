namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecorationCombatReplayDescription
    {
        public string Type { get; protected set; }
        public long Start { get; }
        public long End { get; }

        protected GenericDecorationCombatReplayDescription(GenericDecoration decoration)
        {
            Start = decoration.Lifespan.start;
            End = decoration.Lifespan.end;
        }
    }
}
