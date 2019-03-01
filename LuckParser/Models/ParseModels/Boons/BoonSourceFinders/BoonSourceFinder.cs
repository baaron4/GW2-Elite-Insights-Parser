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

        protected List<CastLog> GetExtensionSkills(ParsedLog log, HashSet<long> ids)
        {
            if (_extensionSkills == null)
            {
                List<CastLog> extensionSkills = new List<CastLog>();
                foreach (Player p in log.PlayerList)
                {
                    _extensionSkills.AddRange(p.GetCastLogs(log, log.FightData.ToFightSpace(p.FirstAware), log.FightData.ToFightSpace(p.LastAware)).Where(x => ids.Contains(x.SkillId)));
                }
            }
            return _extensionSkills;
        }

        public abstract ushort TryFindSrc(AbstractActor a, long time, long extension, ParsedLog log);

    }
}
