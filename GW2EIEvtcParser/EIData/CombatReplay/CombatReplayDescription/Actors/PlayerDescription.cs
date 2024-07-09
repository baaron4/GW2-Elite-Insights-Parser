namespace GW2EIEvtcParser.EIData
{
    public class PlayerDescription : AbstractSingleActorDescription
    {
        public int Group { get; }

        internal PlayerDescription(AbstractPlayer player, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(player, log, map, replay)
        {
            Group = player.Group;
            SetStatus(log, player);
            SetBreakbarStatus(log, player);
        }

    }
}
