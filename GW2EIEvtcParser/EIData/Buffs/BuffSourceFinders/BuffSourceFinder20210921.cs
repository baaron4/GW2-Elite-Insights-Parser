using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders;

internal class BuffSourceFinder20210921 : BuffSourceFinder20210511
{
    private List<CastEvent>? _vindicatorDodges = null;
    public BuffSourceFinder20210921(HashSet<long> boonIDs) : base(boonIDs)
    {
        ImperialImpactExtension = 2000;
    }

    protected override IEnumerable<AgentItem> GetImperialImpactAgents(long buffID, long time, long extension, ParsedEvtcLog log)
    {
        if (extension > ImperialImpactExtension + ParserHelper.BuffSimulatorStackActiveDelayConstant)
        {
            return [];
        }

        if (_vindicatorDodges == null)
        {
            //TODO(Rennorb) @perf: find average complexity
            _vindicatorDodges = new List<CastEvent>(log.PlayerList.Count(p => p.Spec == ParserHelper.Spec.Vindicator) * 50);
            foreach (Player p in log.PlayerList)
            {
                if (p.Spec == ParserHelper.Spec.Vindicator)
                {
                    _vindicatorDodges.AddRange(p.GetIntersectingCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillID == SkillIDs.ImperialImpactDodge));
                }
            }
            _vindicatorDodges.SortByTime();
        }
        
        var buffDescription = log.CombatData.GetBuffInfoEvent(buffID);
        if (buffDescription != null && buffDescription.DurationCap == 0)
        {
            if (Math.Abs(extension - ImperialImpactExtension) > ParserHelper.BuffSimulatorStackActiveDelayConstant)
            {
                return [];
            }
        }
        var candidates = _vindicatorDodges.Where(x => x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant);
        return candidates.Select(x => x.Caster);
    }

}
