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
                    return 0;
                }
                // if not herald or tempest in squad then can only be the trait
                return 1;
            }
            return -1;
        }

        private bool CouldBeImbuedMelodies(AbstractCastEvent item, long time, long extension, ParsedLog log)
        {
            if (extension == ImbuedMelodies && log.PlayerListBySpec.TryGetValue("Tempest", out List<Player> tempests))
            {
                var magAuraApplications = new HashSet<AgentItem>(log.CombatData.GetBoonData(5684).Where(x => x is BuffApplyEvent && x.Time - time < 50 && x.By != item.Caster).Select(x => x.By));
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
            if (essenceOfSpeedCheck != -1)
            {
                // unknown or self
                return essenceOfSpeedCheck == 0 ? GeneralHelper.UnknownAgent : dst;
            }
            if (DurationToIDs.TryGetValue(extension, out HashSet<long> idsToCheck))
            {
                List<AbstractCastEvent> cls = GetExtensionSkills(log, time, idsToCheck);
                if (cls.Count == 1)
                {
                    AbstractCastEvent item = cls.First();
                    // Imbued Melodies check
                    if (CouldBeImbuedMelodies(item, time, extension, log))
                    {
                        return GeneralHelper.UnknownAgent;
                    }
                    return item.Caster;
                }
            }
            return GeneralHelper.UnknownAgent;
        }

    }
}
