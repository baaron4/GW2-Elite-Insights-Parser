using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerDstFirstHitMechanic : PlayerDstHitMechanic
    {
        protected override bool Keep(AbstractHealthDamageEvent c, ParsedEvtcLog log)
        {
            return c.From != ParserHelper._unknownAgent && base.Keep(c, log) && GetFirstHit(c.From, log) == c;
        }

        private readonly Dictionary<AgentItem, AbstractHealthDamageEvent> _firstHits = new Dictionary<AgentItem, AbstractHealthDamageEvent>();

        public PlayerDstFirstHitMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerDstFirstHitMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
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
