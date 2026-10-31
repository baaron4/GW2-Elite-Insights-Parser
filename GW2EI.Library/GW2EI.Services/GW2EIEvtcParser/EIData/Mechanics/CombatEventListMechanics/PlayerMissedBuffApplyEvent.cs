using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
internal class PlayerMissedBuffApplyEvent : CheckedMechanic<BuffApplyEvent>
{
    private readonly Dictionary<SingleActor, List<BuffApplyEvent>> _noBuffAppliesPerGivers = [];

    private readonly long BuffID;

    private List<BuffApplyEvent> GetBuffApplyEventsForExpectedProvider(ParsedEvtcLog log, AgentItem playerAgent)
    {
        var actor = log.FindActor(playerAgent);
        if (!_noBuffAppliesPerGivers.TryGetValue(actor, out var noBuffApplies))
        {
            var sameGroupPlayers = log.PlayerList.Where(x => x.Group == actor.Group && x.InAwareTimes(actor)).ToList();
            List<BuffApplyEvent> buffApplies = [];
            foreach (var sameGroupPlayer in sameGroupPlayers)
            {
                var buffAppliesToSameGroupPlayer = log.CombatData.GetBuffApplyDataByIDByDst(BuffID, sameGroupPlayer.AgentItem).OfType<BuffApplyEvent>().Where(x => !x.Initial);
                buffApplies.AddRange(buffAppliesToSameGroupPlayer);
            }
            buffApplies.SortByTime();
            // Group by Src first, and then by application sequences. Ignore sources only giving the buff to themselves or sources from another subgroup
            var groupedBySrc = buffApplies.Select(buffApply => (log.FindActor(buffApply.By.FindEnglobedAgentItem(buffApply.Time)), buffApply)).GroupBy(x => x.Item1).ToDictionary(x => x.Key, x => x.ToList().Select(y => y.buffApply).ToList());
            var groupedBySrcByApplicationSequence = new Dictionary<SingleActor, Dictionary<long, List<BuffApplyEvent>>>();
            foreach (var groupBySrc in groupedBySrc)
            {
                var applierActor = groupBySrc.Key;
                if (applierActor.Group != actor.Group)
                {
                    continue;
                }
                var groupedApplies = new Dictionary<long, List<BuffApplyEvent>>();
                var epsilonPacked = CombatData.RegroupCombatEventsByTime(groupBySrc.Value, ParserHelper.BuffSimulatorDelayConstant);
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
            noBuffApplies = [];
            var livelyLuteCasts = BuffID == SkillIDs.Quickness ? log.CombatData.GetAnimatedCastData(SkillIDs.LivelyLute).GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList()) : [];
            foreach (var player in sameGroupPlayers)
            {
                var noBuffAppliesPerPlayer = new List<BuffApplyEvent>();
                if (player == actor)
                {
                    noBuffApplies = noBuffAppliesPerPlayer;
                }
                foreach (var groupBySrcByApplicationSequence in groupedBySrcByApplicationSequence)
                {
                    foreach (var buffAppliesSequence in groupBySrcByApplicationSequence.Value)
                    {
                        // Check if player is present as dst in the sequence
                        if (!buffAppliesSequence.Value.Any(x => x.To.Is(player.AgentItem)))
                        {
                            var firstBuffApplyFromSequence = buffAppliesSequence.Value.First();
                            var applier = firstBuffApplyFromSequence.By;
                            // Security window for lively lute
                            if (livelyLuteCasts.TryGetValue(applier, out var casts) && casts.Any(x => x.Time < firstBuffApplyFromSequence.Time && x.Time + 1100 > firstBuffApplyFromSequence.Time))
                            {
                                continue;
                            }
                            // Verify that the player is not dead nor dc
                            if (!player.IsDead(log, firstBuffApplyFromSequence.Time) && !player.IsDC(log, firstBuffApplyFromSequence.Time))
                            {
                                noBuffAppliesPerPlayer.Add(firstBuffApplyFromSequence);
                            }
                        }
                    }
                }
                noBuffApplies.SortByTime();
                _noBuffAppliesPerGivers[player] = noBuffAppliesPerPlayer;
            }
            _noBuffAppliesPerGivers[actor] = noBuffApplies;
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
            foreach (BuffApplyEvent c in GetBuffApplyEventsForExpectedProvider(log, p.AgentItem))
            {
                if (Keep(c, log))
                {
                    InsertMechanic(log, mechanicLogs, c.Time, p);
                }
            }
        }
    }
}
