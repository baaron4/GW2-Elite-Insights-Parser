using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffSourceFinder
    {
        private List<AbstractCastEvent> _extensionSkills = null;
        private readonly HashSet<long> _boonIds = null;
        protected HashSet<long> ExtensionIDS { get; set; } = new HashSet<long>();
        protected Dictionary<long, HashSet<long>> DurationToIDs { get; set; } = new Dictionary<long, HashSet<long>>();
        // non trackable times
        protected long EssenceOfSpeed { get; set; }
        protected long ImbuedMelodies { get; set; }

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
                    _extensionSkills.AddRange(p.GetIntersectingCastEvents(log, 0, log.FightData.FightEnd).Where(x => ExtensionIDS.Contains(x.SkillId) && x.Status != AbstractCastEvent.AnimationStatus.Interrupted));
                }
            }
            return _extensionSkills.Where(x => idsToKeep.Contains(x.SkillId) && x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant).ToList();
        }
        // Spec specific checks
        protected virtual int CouldBeEssenceOfSpeed(AgentItem dst, long extension, long buffID, ParsedEvtcLog log)
        {
            if (extension == EssenceOfSpeed && dst.Prof == "Soulbeast")
            {
                if (log.FriendliesListBySpec.ContainsKey("Herald") || log.FriendliesListBySpec.ContainsKey("Tempest"))
                {
                    // uncertain, needs to check more
                    return 0;
                }
                // if not herald or tempest in squad then can only be the trait
                return 1;
            }
            return -1;
        }

        protected virtual bool CouldBeImbuedMelodies(AgentItem agent, long time, long extension, ParsedEvtcLog log)
        {
            if (extension == ImbuedMelodies && log.FriendliesListBySpec.TryGetValue("Tempest", out List<AbstractSingleActor> tempests))
            {
                var magAuraApplications = new HashSet<AgentItem>(log.CombatData.GetBuffData(5684).Where(x => x is BuffApplyEvent && Math.Abs(x.Time - time) < ParserHelper.ServerDelayConstant && x.CreditedBy != agent).Select(x => x.CreditedBy));
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

        protected virtual HashSet<long> GetIDs(ParsedEvtcLog log, long buffID, long extension)
        {
            if (DurationToIDs.TryGetValue(extension, out HashSet<long> idsToCheck))
            {
                return idsToCheck;
            }
            return new HashSet<long>();
        }

        // Main method
        public AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedEvtcLog log, long buffID)
        {
            if (!_boonIds.Contains(buffID))
            {
                return dst;
            }
            int essenceOfSpeedCheck = CouldBeEssenceOfSpeed(dst, extension, buffID, log);
            // can only be the soulbeast
            if (essenceOfSpeedCheck == 1)
            {
                return dst;
            }
            HashSet<long> idsToCheck = GetIDs(log, buffID, extension);
            if (idsToCheck.Any())
            {
                List<AbstractCastEvent> cls = GetExtensionSkills(log, time, idsToCheck);
                // If only one cast item
                if (cls.Count == 1)
                {
                    AbstractCastEvent item = cls.First();
                    // If uncertainty due to essence of speed or imbued melodies, return unknown
                    if (essenceOfSpeedCheck == 0 || CouldBeImbuedMelodies(item.Caster, time, extension, log))
                    {
                        return ParserHelper._unknownAgent;
                    }
                    // otherwise the src is the caster
                    return item.Caster;
                }
                // If no cast item and uncertainty due to essence of speed
                else if (!cls.Any() && essenceOfSpeedCheck == 0)
                {
                    // If uncertainty due to imbued melodies, return unknown
                    if (CouldBeImbuedMelodies(dst, time, extension, log))
                    {
                        return ParserHelper._unknownAgent;
                    }
                    // otherwise return the soulbeast
                    return dst;
                }
            }
            return ParserHelper._unknownAgent;
        }

    }
}
