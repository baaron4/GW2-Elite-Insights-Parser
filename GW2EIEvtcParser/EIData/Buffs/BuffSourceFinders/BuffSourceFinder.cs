using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders;

internal abstract class BuffSourceFinder
{
    private List<CastEvent>? _extensionSkills = null;
    private readonly HashSet<long> _boonIds;
    protected HashSet<long> ExtensionIDS = [];
    protected Dictionary<long, HashSet<long>> DurationToIDs = [];
    // non trackable times
    protected long EssenceOfSpeed = int.MinValue;
    protected long ImbuedMelodies = int.MinValue;
    protected long ImperialImpactExtension = int.MinValue;

    protected BuffSourceFinder(HashSet<long> boonIds)
    {
        _boonIds = boonIds;
    }

    private IEnumerable<CastEvent> GetExtensionSkills(ParsedEvtcLog log, long time, HashSet<long> idsToKeep)
    {
        if (_extensionSkills == null)
        {
            _extensionSkills = [];
            foreach (Player p in log.PlayerList)
            {
                _extensionSkills.AddRange(p.GetIntersectingCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => ExtensionIDS.Contains(x.SkillId) && !x.IsInterrupted));
            }
        }
        return _extensionSkills.Where(x => idsToKeep.Contains(x.SkillId) && x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant);
    }
    // Spec specific checks

    protected virtual int CouldBeEssenceOfSpeed(AgentItem dst, long buffID, long time, long extension, ParsedEvtcLog log)
    {
        if (dst.Spec == ParserHelper.Spec.Soulbeast && Math.Abs(extension - EssenceOfSpeed) <= ParserHelper.BuffSimulatorStackActiveDelayConstant)
        {
            if (GetIDs(log, buffID, extension).Count != 0)
            {
                // uncertain, needs to check more
                return 0;
            }
            if (ImbuedMelodies == EssenceOfSpeed && log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Tempest))
            {
                // uncertain, needs to check more
                return 0;
            }
            if (EssenceOfSpeed == ImperialImpactExtension && log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Vindicator))
            {
                // uncertain, needs to check more
                return 0;
            }
            return 1;
        }
        return -1;
    }

    protected virtual bool CouldBeImbuedMelodies(AgentItem agent, long buffID, long time, long extension, ParsedEvtcLog log)
    {
        if (log.FriendliesListBySpec.TryGetValue(ParserHelper.Spec.Tempest, out var tempests) && Math.Abs(extension - ImbuedMelodies) <= ParserHelper.BuffSimulatorStackActiveDelayConstant)
        {
            var magAuraApplications = new HashSet<AgentItem>(log.CombatData.GetBuffData(SkillIDs.MagneticAura).Where(x => x is BuffApplyEvent && Math.Abs(x.Time - time) < ParserHelper.ServerDelayConstant && x.CreditedBy != agent).Select(x => x.CreditedBy));
            foreach (SingleActor tempest in tempests)
            {
                if (magAuraApplications.Contains(tempest.AgentItem))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected virtual IEnumerable<AgentItem> CouldBeImperialImpact(long buffID, long time, long extension, ParsedEvtcLog log)
    {
        return [];
    }


    protected virtual HashSet<long> GetIDs(ParsedEvtcLog log, long buffID, long extension)
    {
        var res = new HashSet<long>();
        foreach (KeyValuePair<long, HashSet<long>> pair in DurationToIDs)
        {
            if (Math.Abs(pair.Key - extension) <= ParserHelper.BuffSimulatorStackActiveDelayConstant)
            {
                return pair.Value;
            }
        }
        return res;
    }

    private AgentItem NoCastSrcFinder(AgentItem dst, long time, long extension, ParsedEvtcLog log, long buffID, int essenceOfSpeedCheck, IEnumerable<AgentItem> imperialImpactCheck)
    {
        // If uncertainty due to imbued melodies, return unknown
        if (CouldBeImbuedMelodies(dst, buffID, time, extension, log))
        {
            return ParserHelper._unknownAgent;
        }
        // uncertainty due to essence of speed but not due to imperial impact
        if (essenceOfSpeedCheck == 0 && !imperialImpactCheck.Any())
        {
            // the soulbeast
            return dst;
        }
        // uncertainty due to imperial impact but not due to essence of speed
        if (essenceOfSpeedCheck == -1 && imperialImpactCheck.Count() == 1)
        {
            // the vindicator
            return imperialImpactCheck.First();
        }
        return ParserHelper._unknownAgent;
    }


    // Main method
    public AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedEvtcLog log, long buffID, uint buffInstance)
    {
        if (!_boonIds.Contains(buffID))
        {
            if (buffInstance > 0)
            {
                BuffApplyEvent? seedApply = log.CombatData.GetBuffDataByInstanceID(buffID, buffInstance).OfType<BuffApplyEvent>().LastOrDefault(x => x.BuffInstance == buffInstance && x.Time <= time);
                if (seedApply != null)
                {
                    return seedApply.By;
                }
            }
            return ParserHelper._unknownAgent;
        }
        var imperialImpactCheck = CouldBeImperialImpact(buffID, time, extension, log);
        var impImpactCount = imperialImpactCheck.Count();
        // Multiple imperial impact at the same time
        if (impImpactCount > 1)
        {
            return ParserHelper._unknownAgent;
        }
        int essenceOfSpeedCheck = CouldBeEssenceOfSpeed(dst, buffID, time, extension, log);
        // can only be the soulbeast
        if (essenceOfSpeedCheck == 1)
        {
            return dst;
        }
        HashSet<long> idsToCheck = GetIDs(log, buffID, extension);
        if (idsToCheck.Count != 0)
        {
            var cls = GetExtensionSkills(log, time, idsToCheck);
            var clsCount = cls.Count();
            // If multiple casters, return unknown
            if (clsCount > 1)
            {
                return ParserHelper._unknownAgent;
            }
            else if (clsCount == 1)
            {
                CastEvent item = cls.First();
                // If uncertainty due to essence of speed, imbued melodies or imperial impact, return unknown
                if (essenceOfSpeedCheck == 0 || CouldBeImbuedMelodies(item.Caster, buffID, time, extension, log) || impImpactCount != 0)
                {
                    return ParserHelper._unknownAgent;
                }
                // otherwise the src is the caster
                return item.Caster;
            }
            // If no cast item 
            else
            {
                return NoCastSrcFinder(dst, time, extension, log, buffID, essenceOfSpeedCheck, imperialImpactCheck);
            }
        }
        // No known cast skill ID for given extension, same as no cast being present
        return NoCastSrcFinder(dst, time, extension, log, buffID, essenceOfSpeedCheck, imperialImpactCheck);
    }

}
