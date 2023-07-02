namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecorationCombatReplayDescription : AbstractCombatReplayDescription
    {

        protected GenericDecorationCombatReplayDescription(GenericDecoration decoration)
        {
            Start = decoration.Lifespan.start;
            End = decoration.Lifespan.end;
        }
    }
}
