namespace GW2EIEvtcParser.EIData;

public class PlayerCombatReplayDescription : SingleActorCombatReplayDescription
{
    public readonly int Group;

    internal PlayerCombatReplayDescription(PlayerActor player, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay) : base(player, log, map, replay)
    {
        Group = player.Group;
    }

}
