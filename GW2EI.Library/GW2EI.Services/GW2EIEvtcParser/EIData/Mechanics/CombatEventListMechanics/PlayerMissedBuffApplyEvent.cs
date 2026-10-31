using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
internal class PlayerMissedBuffApplyEvent : CheckedMechanic<BuffApplyEvent>
{
    private readonly Dictionary<SingleActor, List<BuffApplyEvent>> _noBuffAppliesPerGivers = [];

    private readonly long BuffID;

    private List<BuffApplyEvent> GetBuffApplyEventsForExpectedProvider(ParsedEvtcLog log, SingleActor player)
    {
        if (!_noBuffAppliesPerGivers.TryGetValue(player, out var noBuffApplies))
        {
            noBuffApplies = [];
            // Find buff applies to other players from the same subroup + current player
            var sameGroupPlayers = log.PlayerList.Where(x => x.Group == player.Group && x.InAwareTimes(player)).ToList();
            List<BuffApplyEvent> buffApplies = [];
            foreach (var sameGroupPlayer in sameGroupPlayers)
            {
                var buffAppliesToSameGroupPlayer = log.CombatData.GetBuffApplyDataByIDByDst(BuffID, sameGroupPlayer.AgentItem).OfType<BuffApplyEvent>().Where(x => !x.Initial);
                buffApplies.AddRange(buffAppliesToSameGroupPlayer);
            }
            buffApplies.SortByTime();
            // Group by Src Single Actor first, and then by application sequences.
            var groupedBySrc = buffApplies.Select(buffApply => (log.FindActor(buffApply.By.FindEnglobedAgentItem(buffApply.Time)), buffApply)).GroupBy(x => x.Item1).ToDictionary(x => x.Key, x => x.ToList().Select(y => y.buffApply).ToList());
            var groupedBySrcByApplicationSequence = new Dictionary<SingleActor, Dictionary<long, List<BuffApplyEvent>>>();
            foreach (var groupBySrc in groupedBySrc)
            {
                var applierActor = groupBySrc.Key;
                // Ignore sources from another subgroup
                if (applierActor.Group != player.Group)
                {
                    continue;
                }
                var groupedApplies = new Dictionary<long, List<BuffApplyEvent>>();
                var epsilonPacked = CombatData.RegroupCombatEventsByTime(groupBySrc.Value, ParserHelper.BuffSimulatorDelayConstant);
                // Ignore sources only giving the buff to themselves
                if (epsilonPacked.All(x => x.Count <= 1))
                {
                    continue;
                }
                groupedBySrcByApplicationSequence[groupBySrc.Key] = groupedApplies;
                foreach (var list in epsilonPacked)
                {
                    groupedApplies[list.First().Time] = list;
                }
            }
            var livelyLuteCasts = BuffID == SkillIDs.Quickness ? log.CombatData.GetAnimatedCastData(SkillIDs.LivelyLute).GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList()) : [];
            // Prepare the dictionary for every players from the same subgroup
            foreach (var sameGroupPlayer in sameGroupPlayers)
            {
                var noBuffAppliesPerPlayer = new List<BuffApplyEvent>();
                if (sameGroupPlayer == player)
                {
                    noBuffApplies = noBuffAppliesPerPlayer;
                }
                foreach (var groupBySrcByApplicationSequence in groupedBySrcByApplicationSequence)
                {
                    foreach (var buffAppliesSequence in groupBySrcByApplicationSequence.Value)
                    {
                        // Check if player is present as dst in the sequence
                        if (!buffAppliesSequence.Value.Any(x => x.To.Is(sameGroupPlayer.AgentItem)))
                        {
                            var firstBuffApplyFromSequence = buffAppliesSequence.Value.First();
                            var applier = firstBuffApplyFromSequence.By;
                            // Security window for lively lute
                            if (livelyLuteCasts.TryGetValue(applier, out var casts) && casts.Any(x => x.Time < firstBuffApplyFromSequence.Time && x.Time + 1100 > firstBuffApplyFromSequence.Time))
                            {
                                continue;
                            }
                            // Verify that the player is not dead nor dc
                            if (!sameGroupPlayer.IsDead(log, firstBuffApplyFromSequence.Time) && !sameGroupPlayer.IsDC(log, firstBuffApplyFromSequence.Time))
                            {
                                noBuffAppliesPerPlayer.Add(firstBuffApplyFromSequence);
                            }
                        }
                    }
                }
                noBuffApplies.SortByTime();
                _noBuffAppliesPerGivers[sameGroupPlayer] = noBuffAppliesPerPlayer;
            }
            // Sanity add, just in case
            _noBuffAppliesPerGivers[player] = noBuffApplies;
        }
        return noBuffApplies;
    }

    public PlayerMissedBuffApplyEvent(long buffID, int id, MechanicPlotlySetting plotlySetting, MechanicDescription description) : base(id, plotlySetting, description, MechanicSeverity.Sev4, 50)
    {
        if (buffID != SkillIDs.Quickness && buffID != SkillIDs.Alacrity)
        {
            throw new InvalidDataException("PlayerMissedBuffApplyEvent only supports alacrity and quickness");
        }
        BuffID = buffID;
        IsEnemyMechanic = false;
    }

    internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, SingleActor> regroupedMobs)
    {
        foreach (Player p in log.PlayerList)
        {
            foreach (BuffApplyEvent c in GetBuffApplyEventsForExpectedProvider(log, p))
            {
                if (Keep(c, log))
                {
                    InsertMechanic(log, mechanicLogs, c.Time, p);
                }
            }
        }
    }
}
