using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSourceFinder
    {
        private List<CastLog> _extensionSkills = null;
        protected HashSet<long> ExtensionIDS = new HashSet<long>();
        protected Dictionary<long, HashSet<long>> DurationToIDs = new Dictionary<long, HashSet<long>>();
        // non trackable times
        protected long EssenceOfSpeed;
        protected long ImbuedMelodies;

        private List<CastLog> GetExtensionSkills(ParsedLog log, long time, HashSet<long> idsToKeep)
        {
            if (_extensionSkills == null)
            {
                _extensionSkills = new List<CastLog>();
                foreach (Player p in log.PlayerList)
                {
                    _extensionSkills.AddRange(p.GetCastLogs(log, log.FightData.ToFightSpace(p.FirstAware), log.FightData.ToFightSpace(p.LastAware)).Where(x => ExtensionIDS.Contains(x.SkillId) && x.EndActivation.NoInterruptEndCasting()));
                }
            }
            return _extensionSkills.Where(x => idsToKeep.Contains(x.SkillId) && x.Time <= time && time <= x.Time + x.ActualDuration + 10).ToList();
        }
        // Spec specific checks
        private int CheckSoulbeast(AbstractActor a, long extension, ParsedLog log)
        {
            if (extension == EssenceOfSpeed && a.Prof == "Soulbeast")
            {
                if (log.PlayerListBySpec.ContainsKey("Herald") || log.PlayerListBySpec.ContainsKey("Tempest"))
                {
                    return 0;
                }
                // if not herald or tempest in squad then can only be the trait
                return a.InstID;
            }
            return -1;
        }

        private int CheckTempest(CastLog item, long time, long extension, ParsedLog log)
        {
            if (extension == ImbuedMelodies && log.PlayerListBySpec.TryGetValue("Tempest", out List<Player> tempests))
            {
                HashSet<ushort> magAuraApplications = new HashSet<ushort>(log.CombatData.GetBoonData(5684).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.None && Math.Abs(x.Time - log.FightData.ToLogSpace(time)) < 50 && x.SrcInstid != item.SrcInstId).Select(x => x.SrcInstid));
                foreach (Player tempest in tempests)
                {
                    if (magAuraApplications.Contains(tempest.InstID))
                    {
                        return 0;
                    }
                }
            }
            return -1;
        }
        // Main method
        public ushort TryFindSrc(AbstractActor a, long time, long extension, ParsedLog log)
        {
            int sbCheck = CheckSoulbeast(a, extension, log);
            if (sbCheck != -1)
            {
                return (ushort)sbCheck;
            }
            HashSet<long> idsToCheck = new HashSet<long>();
            if (DurationToIDs.TryGetValue(extension, out idsToCheck))
            {
                List<CastLog> cls = GetExtensionSkills(log, time, idsToCheck);
                if (cls.Count == 1)
                {
                    CastLog item = cls.First();
                    // Imbued Melodies check
                    int tempestCheck = CheckTempest(item, time, extension, log);
                    if (tempestCheck != -1)
                    {
                        return (ushort)tempestCheck;
                    }
                    return item.SrcInstId;
                }
            }
            return 0;
        }

    }
}
