using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.EIData
{
    public abstract class BoonSourceFinder
    {
        private List<AbstractCastEvent> _extensionSkills = null;
        private readonly HashSet<long> _boonIds = null;
        protected HashSet<long> ExtensionIDS { get; set; } = new HashSet<long>();
        protected Dictionary<long, HashSet<long>> DurationToIDs { get; set; } = new Dictionary<long, HashSet<long>>();
        // non trackable times
        protected long EssenceOfSpeed { get; set; }
        protected long ImbuedMelodies { get; set; }

        protected BoonSourceFinder(HashSet<long> boonIds)
        {
            _boonIds = boonIds;
        }

        private List<AbstractCastEvent> GetExtensionSkills(ParsedLog log, long time, HashSet<long> idsToKeep)
        {
            if (_extensionSkills == null)
            {
                _extensionSkills = new List<AbstractCastEvent>();
                foreach (Player p in log.PlayerList)
                {
                    _extensionSkills.AddRange(p.GetCastLogsActDur(log, 0, log.FightData.FightDuration).Where(x => ExtensionIDS.Contains(x.SkillId) && !x.Interrupted));
                }
            }
            return _extensionSkills.Where(x => idsToKeep.Contains(x.SkillId) && x.Time <= time && time <= x.Time + x.ActualDuration + 10).ToList();
        }
        // Spec specific checks
        private int CouldBeEssenceOfSpeed(AgentItem dst, long extension, ParsedLog log)
        {
            if (extension == EssenceOfSpeed && dst.Prof == "Soulbeast")
            {
                if (log.PlayerListBySpec.ContainsKey("Herald") || log.PlayerListBySpec.ContainsKey("Tempest"))
                {
                    // uncertain, needs to check more
                    return 0;
                }
                // if not herald or tempest in squad then can only be the trait
                return 1;
            }
            return -1;
        }

        private bool CouldBeImbuedMelodies(AgentItem agent, long time, long extension, ParsedLog log)
        {
            if (extension == ImbuedMelodies && log.PlayerListBySpec.TryGetValue("Tempest", out List<Player> tempests))
            {
                var magAuraApplications = new HashSet<AgentItem>(log.CombatData.GetBoonData(5684).Where(x => x is BuffApplyEvent && Math.Abs(x.Time - time) < 10 && x.By != agent).Select(x => x.By));
                foreach (Player tempest in tempests)
                {
                    if (magAuraApplications.Contains(tempest.AgentItem))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        // Main method
        public AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedLog log, long buffID)
        {
            if (!_boonIds.Contains(buffID))
            {
                return dst;
            }
            int essenceOfSpeedCheck = CouldBeEssenceOfSpeed(dst, extension, log);
            // can only be the soulbeast
            if (essenceOfSpeedCheck == 1)
            {
                return dst;
            }
            if (DurationToIDs.TryGetValue(extension, out HashSet<long> idsToCheck))
            {
                List<AbstractCastEvent> cls = GetExtensionSkills(log, time, idsToCheck);
                // If only one cast item
                if (cls.Count == 1)
                {
                    AbstractCastEvent item = cls.First();
                    // If uncertainty due to essence of speed or imbued melodies, return unknown
                    if (essenceOfSpeedCheck == 0 || CouldBeImbuedMelodies(item.Caster, time, extension, log))
                    {
                        return GeneralHelper.UnknownAgent;
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
                        return GeneralHelper.UnknownAgent;
                    }
                    // otherwise return the soulbeast
                    return dst;
                }
            }
            return GeneralHelper.UnknownAgent;
        }

    }
}
