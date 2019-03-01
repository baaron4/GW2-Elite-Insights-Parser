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

        protected List<CastLog> GetExtensionSkills(ParsedLog log, HashSet<long> ids, long time, HashSet<long> idsToKeep)
        {
            if (_extensionSkills == null)
            {
                List<CastLog> extensionSkills = new List<CastLog>();
                foreach (Player p in log.PlayerList)
                {
                    _extensionSkills.AddRange(p.GetCastLogs(log, log.FightData.ToFightSpace(p.FirstAware), log.FightData.ToFightSpace(p.LastAware)).Where(x => ids.Contains(x.SkillId) && x.EndActivation.NoInterruptEndCasting()));
                }
            }
            return _extensionSkills.Where(x => idsToKeep.Contains(x.SkillId) && x.Time <= time && time <= x.Time + x.ActualDuration + 10).ToList();
        }

        public abstract ushort TryFindSrc(AbstractActor a, long time, long extension, ParsedLog log);

    }
}
