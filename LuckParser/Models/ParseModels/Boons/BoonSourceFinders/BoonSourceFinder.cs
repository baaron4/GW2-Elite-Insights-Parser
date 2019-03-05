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

        public virtual ushort TryFindSrc(AbstractActor a, long time, long extension, ParsedLog log)
        {
            if (extension == 2000 && a.Prof == "Soulbeast")
            {
                if (log.PlayerListBySpec.ContainsKey("Herald") || log.PlayerListBySpec.ContainsKey("Tempest"))
                {
                    return 0;
                }
                // if not herald or tempest in squad then can only be the trait
                return a.InstID;
            }
            HashSet<long> idsToCheck = new HashSet<long>();
            if (DurationToIDs.TryGetValue(extension, out idsToCheck))
            {
                List<CastLog> cls = GetExtensionSkills(log, time, idsToCheck);
                if (cls.Count == 1)
                {
                    CastLog item = cls.First();
                    // Imbued Melodies check
                    if (extension == 2000 && log.PlayerListBySpec.TryGetValue("Tempest", out List<Player> tempests))
                    {
                        List<CombatItem> magAuraApplications = log.CombatData.GetBoonData(5684).Where(x => x.IsBuffRemove == ParseEnum.BuffRemove.None && Math.Abs(x.Time - log.FightData.ToLogSpace(time)) < 50 && x.SrcInstid != item.SrcInstId).ToList();
                        foreach (Player tempest in tempests)
                        {
                            if (magAuraApplications.FirstOrDefault(x => x.SrcInstid == tempest.InstID) != null)
                            {
                                return 0;
                            }
                        }
                    }
                    return item.SrcInstId;
                }
            }
            return 0;
        }

    }
}
