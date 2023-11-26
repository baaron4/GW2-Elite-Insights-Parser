using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal abstract class BuffSourceFinder
    {
        private List<AbstractCastEvent> _extensionSkills = null;
        private readonly HashSet<long> _boonIds = null;
        protected HashSet<long> ExtensionIDS { get; set; } = new HashSet<long>();
        protected Dictionary<long, HashSet<long>> DurationToIDs { get; set; } = new Dictionary<long, HashSet<long>>();
        // non trackable times
        protected long EssenceOfSpeed { get; set; } = int.MinValue;
        protected long ImbuedMelodies { get; set; } = int.MinValue;
        protected long ImperialImpactExtension { get; set; } = int.MinValue;

        protected BuffSourceFinder(HashSet<long> boonIds)
        {
            _boonIds = boonIds;
        }

        private List<AbstractCastEvent> GetExtensionSkills(ParsedEvtcLog log, long time, HashSet<long> idsToKeep)
        {
            if (_extensionSkills == null)
            {
                _extensionSkills = new List<AbstractCastEvent>();
                foreach (Player p in log.PlayerList)
                {
                    _extensionSkills.AddRange(p.GetIntersectingCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => ExtensionIDS.Contains(x.SkillId) && x.Status != AbstractCastEvent.AnimationStatus.Interrupted));
                }
            }
            return _extensionSkills.Where(x => idsToKeep.Contains(x.SkillId) && x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant).ToList();
        }
        // Spec specific checks

        protected virtual int CouldBeEssenceOfSpeed(AgentItem dst, long buffID, long time, long extension, ParsedEvtcLog log)
        {
            if (dst.Spec == ParserHelper.Spec.Soulbeast && Math.Abs(extension - EssenceOfSpeed) <= ParserHelper.BuffSimulatorStackActiveDelayConstant)
            {
                if (GetIDs(log, buffID, extension).Any())
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
            if (log.FriendliesListBySpec.TryGetValue(ParserHelper.Spec.Tempest, out List<AbstractSingleActor> tempests) && Math.Abs(extension - ImbuedMelodies) <= ParserHelper.BuffSimulatorStackActiveDelayConstant)
            {
                var magAuraApplications = new HashSet<AgentItem>(log.CombatData.GetBuffData(SkillIDs.MagneticAura).Where(x => x is BuffApplyEvent && Math.Abs(x.Time - time) < ParserHelper.ServerDelayConstant && x.CreditedBy != agent).Select(x => x.CreditedBy));
                foreach (AbstractSingleActor tempest in tempests)
                {
                    if (magAuraApplications.Contains(tempest.AgentItem))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual List<AgentItem> CouldBeImperialImpact(long buffID, long time, long extension, ParsedEvtcLog log)
        {
            return new List<AgentItem>();
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

        private AgentItem NoCastSrcFinder(AgentItem dst, long time, long extension, ParsedEvtcLog log, long buffID, int essenceOfSpeedCheck, IReadOnlyList<AgentItem> imperialImpactCheck)
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
            if (essenceOfSpeedCheck == -1 && imperialImpactCheck.Count == 1)
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
                    BuffApplyEvent seedApply = log.CombatData.GetBuffDataByInstanceID(buffID, buffInstance).OfType<BuffApplyEvent>().LastOrDefault(x => x.BuffInstance == buffInstance && x.Time <= time);
                    if (seedApply != null)
                    {
                        return seedApply.By;
                    }
                }
                return ParserHelper._unknownAgent;
            }
            List<AgentItem> imperialImpactCheck = CouldBeImperialImpact(buffID, time, extension, log);
            // Multiple imperial impact at the same time
            if (imperialImpactCheck.Count > 1)
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
            if (idsToCheck.Any())
            {
                List<AbstractCastEvent> cls = GetExtensionSkills(log, time, idsToCheck);
                // If multiple casters, return unknown
                if (cls.Count > 1)
                {
                    return ParserHelper._unknownAgent;
                }
                else if (cls.Count == 1)
                {
                    AbstractCastEvent item = cls.First();
                    // If uncertainty due to essence of speed, imbued melodies or imperial impact, return unknown
                    if (essenceOfSpeedCheck == 0 || CouldBeImbuedMelodies(item.Caster, buffID, time, extension, log) || imperialImpactCheck.Any())
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
}
