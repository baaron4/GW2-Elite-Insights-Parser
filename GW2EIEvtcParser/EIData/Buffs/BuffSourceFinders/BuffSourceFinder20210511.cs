using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders;

internal class BuffSourceFinder20210511 : BuffSourceFinder20191001
{
    public BuffSourceFinder20210511(HashSet<long> boonIDs) : base(boonIDs)
    {
    }

    // Spec specific checks
    protected override Certainty CouldBeEssenceOfSpeed(AgentItem dst, long buffID, long time, long extension, ParsedEvtcLog log)
    {
        var buffDescription = log.CombatData.GetBuffInfoEvent(buffID);
        if (buffDescription != null && buffDescription.DurationCap == 0)
        {
            return base.CouldBeEssenceOfSpeed(dst, buffID, time, extension, log);
        }
        if (IsSoulbeast(dst, time) && extension <= EssenceOfSpeed + ParserHelper.BuffSimulatorStackActiveDelayConstant)
        {
            if (GetIDs(log, buffID, extension).Count != 0)
            {
                return Certainty.Uncertain;
            }
            if (extension <= ImbuedMelodies + ParserHelper.BuffSimulatorStackActiveDelayConstant && log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Tempest))
            {
                return Certainty.Uncertain;
            }
            if (extension <= ImperialImpactExtension + ParserHelper.BuffSimulatorStackActiveDelayConstant && log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Vindicator))
            {
                return Certainty.Uncertain;
            }
            return Certainty.Certain;
        }
        return Certainty.NotApplicable;
    }

    protected override HashSet<long> GetIDs(ParsedEvtcLog log, long buffID, long extension)
    {
        var buffDescription = log.CombatData.GetBuffInfoEvent(buffID);
        if (buffDescription != null && buffDescription.DurationCap == 0)
        {
            return base.GetIDs(log, buffID, extension);
        }
        var res = new HashSet<long>();
        foreach (KeyValuePair<long, HashSet<long>> pair in DurationToIDs)
        {
            if (extension <= pair.Key + ParserHelper.BuffSimulatorStackActiveDelayConstant)
            {
                res.UnionWith(pair.Value);
            }
        }
        return res;
    }
}
