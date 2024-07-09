namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractRenderableCombatReplayDescription : AbstractCombatReplayDescription
    {
        public long Start { get; protected set; }
        public long End { get; protected set; }

        public bool IsMechanicOrSkill { get; protected set; }

        protected AbstractRenderableCombatReplayDescription()
        {
        }
    }
}
