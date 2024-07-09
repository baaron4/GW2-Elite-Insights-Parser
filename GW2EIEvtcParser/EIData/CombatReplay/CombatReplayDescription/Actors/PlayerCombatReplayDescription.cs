namespace GW2EIEvtcParser.EIData
{
    internal class PlayerCombatReplayDescription : AbstractSingleActorCombatReplayDescription
    {
        public int Group { get; }

        internal PlayerCombatReplayDescription(AbstractPlayer player, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(player, log, map, replay)
        {
            Group = player.Group;
            SetStatus(log, player);
            SetBreakbarStatus(log, player);
        }

    }
}
