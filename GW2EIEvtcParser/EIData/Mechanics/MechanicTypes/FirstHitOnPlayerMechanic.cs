using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class FirstHitOnPlayerMechanic : HitOnPlayerMechanic
    {
        protected override bool Keep(AbstractHealthDamageEvent c, ParsedEvtcLog log)
        {
            if (!base.Keep(c, log) || GetFirstHit(c.From, log) != c)
            {
                return false;
            }
            return true;
        }

        private readonly Dictionary<AgentItem, AbstractHealthDamageEvent> _firstHits = new Dictionary<AgentItem, AbstractHealthDamageEvent>();

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public FirstHitOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        private AbstractHealthDamageEvent GetFirstHit(AgentItem src, ParsedEvtcLog log)
        {
            if (!_firstHits.TryGetValue(src, out AbstractHealthDamageEvent evt))
            {
                AbstractHealthDamageEvent res = log.CombatData.GetDamageData(src).Where(x => x.SkillId == SkillId && x.To.Type == AgentItem.AgentType.Player && base.Keep(x, log)).FirstOrDefault();
                _firstHits[src] = res;
                return res;
            }
            return evt;
        }
    }
}
