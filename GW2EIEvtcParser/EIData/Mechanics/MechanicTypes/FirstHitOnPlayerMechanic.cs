using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class FirstHitOnPlayerMechanic : HitOnPlayerMechanic
    {
        protected override bool Keep(AbstractHealthDamageEvent c, ParsedEvtcLog log)
        {
            if (c.From == ParserHelper._unknownAgent || !base.Keep(c, log) || GetFirstHit(c.From, log) != c)
            {
                return false;
            }
            return true;
        }

        private readonly Dictionary<AgentItem, AbstractHealthDamageEvent> _firstHits = new Dictionary<AgentItem, AbstractHealthDamageEvent>();

        public FirstHitOnPlayerMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public FirstHitOnPlayerMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        private AbstractHealthDamageEvent GetFirstHit(AgentItem src, ParsedEvtcLog log)
        {
            if (!_firstHits.TryGetValue(src, out AbstractHealthDamageEvent evt))
            {
                AbstractHealthDamageEvent res = log.CombatData.GetDamageData(src).Where(x => MechanicIDs.Contains(x.SkillId) && x.To.Type == AgentItem.AgentType.Player && base.Keep(x, log)).FirstOrDefault();
                _firstHits[src] = res;
                return res;
            }
            return evt;
        }
    }
}
