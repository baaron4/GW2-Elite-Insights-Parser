namespace GW2EIEvtcParser.EIData;

public class PlayerCombatReplayDescription : AbstractSingleActorCombatReplayDescription
{
    public readonly int Group;

    internal PlayerCombatReplayDescription(AbstractPlayer player, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(player, log, map, replay)
    {
        Group = player.Group;
        SetStatus(log, player);
        SetBreakbarStatus(log, player);
    }

}
